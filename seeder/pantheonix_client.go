package main

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"golang.org/x/sync/errgroup"
	"io"
	"log"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
	"strings"
	"time"
)

type PantheonixClient struct {
	config *Fixtures
	*http.Client
}

func NewPantheonixClient(config *Fixtures) *PantheonixClient {
	return &PantheonixClient{config, &http.Client{Timeout: 30 * time.Second}}
}

func (c *PantheonixClient) Endpoint(endpoint string) string {
	return c.config.BaseUrl + endpoint
}

func (t *BearerToken) SetAccessToken(token string, cookie *http.Cookie) {
	t.AccessToken = token
	t.Cookie = cookie
}

func (c *PantheonixClient) Login(user *UserData) (*BearerToken, error) {
	token := NewBearerToken()

	// Send a login request
	bodyJson, err := json.Marshal(user)
	if err != nil {
		return token, fmt.Errorf("failed to serialize user: %s", err)
	}

	bodyReader := bytes.NewReader(bodyJson)
	res, err := c.Post(c.Endpoint(c.config.Users.Endpoints.Login), "application/json", bodyReader)
	defer res.Body.Close()

	if err != nil {
		return token, fmt.Errorf("failed to login: %s", err)
	}

	if res.StatusCode != http.StatusOK {
		return token, fmt.Errorf("failed to login: %s", res.Status)
	}

	// Extract the bearer token from the response
	cookies := res.Cookies()

	for _, cookie := range cookies {
		if strings.Contains(cookie.Name, "AccessToken") {
			token.SetAccessToken(cookie.Value, cookie)
			break
		}
	}

	return token, nil
}

func (c *PantheonixClient) CreateProblem(ctx context.Context, token *BearerToken, problemId int) (*ProblemDto, error) {
	problem := c.config.Problems.Data[problemId]

	reqBodyJson, err := os.ReadFile(problem.CreateReqPath)
	if err != nil {
		return nil, fmt.Errorf("failed to parse create problem request file content for problem %d: %v", problemId, err)
	}

	bodyReader := bytes.NewReader(reqBodyJson)
	req, err := http.NewRequest(http.MethodPost, c.Endpoint(c.config.Problems.Endpoints.Create), bodyReader)

	if err != nil {
		return nil, fmt.Errorf("failed to create problem with id %d: %v", problemId, err)
	}
	req.Header.Set("Content-Type", "application/json")
	req.AddCookie(token.Cookie)

	res, err := c.Do(req)
	if err != nil {
		return nil, fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}
	defer res.Body.Close()

	if res.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("failed to create problem: %s", res.Status)
	}

	problemDto := &ProblemDto{}
	resBody, err := io.ReadAll(res.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}

	if err := json.Unmarshal(resBody, problemDto); err != nil {
		return nil, fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}

	// Upload tests
	g, ctx := errgroup.WithContext(ctx)
	tests := c.config.Problems.Data[problemId].Tests
	for testId := range len(tests) {
		g.Go(func() error {
			if err := c.CreateTest(token, problemDto.Id, problemId, testId); err != nil {
				return err
			}
			return nil
		})
	}

	if err := g.Wait(); err != nil {
		return nil, err
	}

	// Publish problem
	if err := c.PublishProblem(token, problemDto.Id); err != nil {
		return nil, err
	}

	log.Printf("Successfully created problem with id %d and guid %s\n", problemId, problemDto.Id)

	return problemDto, nil
}

func (c *PantheonixClient) CreateTest(token *BearerToken, problemGuid string, problemId, testId int) error {
	test := c.config.Problems.Data[problemId].Tests[testId]

	extraParams := map[string]string{
		"score": fmt.Sprintf("%d", test.Score),
	}
	createTestEndpoint := c.Endpoint(c.config.Problems.Endpoints.CreateTest)
	createTestEndpoint = strings.Replace(createTestEndpoint, "{problem_id}", problemGuid, 1)

	req, err := newFileUploadRequest(createTestEndpoint, test.TestZipPath, "archiveFile", extraParams)
	if err != nil {
		return fmt.Errorf("failed to compose create request for test with id %d for problem %d: %s", testId, problemId, err)
	}
	req.AddCookie(token.Cookie)

	res, err := c.Do(req)
	if err != nil {
		return fmt.Errorf("failed to send create request test with id %d for problem %d: %s", testId, problemId, err)
	}
	defer res.Body.Close()

	if res.StatusCode != http.StatusOK {
		return fmt.Errorf("failed to create test with id %d for problem %d: %s", testId, problemId, res.Status)
	}

	log.Printf("Successfully created test with id %d\n", testId)

	return nil
}

func (c *PantheonixClient) PublishProblem(token *BearerToken, problemGuid string) error {
	publishProblemEndpoint := c.Endpoint(c.config.Problems.Endpoints.Update)
	publishProblemEndpoint = strings.Replace(publishProblemEndpoint, "{problem_id}", problemGuid, 1)

	bodyJson, err := json.Marshal(map[string]bool{"isPublished": true})
	if err != nil {
		return fmt.Errorf("failed to serialize publish request for problem %s: %s", problemGuid, err)
	}

	body := bytes.NewReader(bodyJson)
	req, err := http.NewRequest(http.MethodPut, publishProblemEndpoint, body)
	if err != nil {
		return fmt.Errorf("failed to compose publish request for problem %s: %s", problemGuid, err)
	}
	req.Header.Set("Content-Type", "application/json")
	req.AddCookie(token.Cookie)

	res, err := c.Do(req)
	if err != nil {
		return fmt.Errorf("failed to send publish request for problem %s: %s", problemGuid, err)
	}
	defer res.Body.Close()

	if res.StatusCode != http.StatusOK {
		return fmt.Errorf("failed to publish problem %s: %s", problemGuid, res.Status)
	}

	log.Printf("Successfully published problem %s\n", problemGuid)

	return nil
}

func (c *PantheonixClient) CreateSubmission(token *BearerToken, submissionDto *SubmissionDto) error {
	createSubmissionEndpoint := c.Endpoint(c.config.Problems.Endpoints.CreateSubmission)

	bodyJson, err := json.Marshal(submissionDto)
	if err != nil {
		return fmt.Errorf("failed to serialize submission: %s", err)
	}

	body := bytes.NewReader(bodyJson)
	req, err := http.NewRequest(http.MethodPost, createSubmissionEndpoint, body)
	if err != nil {
		return fmt.Errorf("failed to compose submission request: %s", err)
	}
	req.Header.Set("Content-Type", "application/json")
	req.AddCookie(token.Cookie)

	res, err := c.Do(req)
	if err != nil {
		return fmt.Errorf("failed to send submission request: %s", err)
	}
	defer res.Body.Close()

	if res.StatusCode != http.StatusCreated {
		return fmt.Errorf("failed to submit: %s", res.Status)
	}

	log.Printf("Successfully submitted solution for problem %s\n", submissionDto.ProblemId)

	return nil
}

func newFileUploadRequest(uri string, filePath string, testFileParamName string, params map[string]string) (*http.Request, error) {
	fileReader, err := os.Open(filePath)
	if err != nil {
		return nil, err
	}
	defer fileReader.Close()

	body := &bytes.Buffer{}
	writer := multipart.NewWriter(body)
	part, err := writer.CreateFormFile(testFileParamName, filepath.Base(filePath))
	if err != nil {
		return nil, err
	}

	_, err = io.Copy(part, fileReader)
	for key, val := range params {
		err = writer.WriteField(key, val)
		if err != nil {
			return nil, err
		}
	}

	err = writer.Close()
	if err != nil {
		return nil, err
	}

	req, err := http.NewRequest(http.MethodPost, uri, body)
	if err != nil {
		return nil, err
	}
	req.Header.Set("Content-Type", writer.FormDataContentType())

	return req, nil
}

type ProblemDto struct {
	Id string `json:"id"`
}

type SubmissionDto struct {
	ProblemId  string `json:"problem_id"`
	Language   string `json:"language"`
	SourceCode string `json:"source_code"`
}

type BearerToken struct {
	AccessToken string
	Cookie      *http.Cookie
}

func NewBearerToken() *BearerToken {
	return &BearerToken{
		AccessToken: "",
		Cookie:      nil,
	}
}
