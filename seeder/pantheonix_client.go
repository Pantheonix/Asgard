package main

import (
	"bytes"
	"encoding/json"
	"fmt"
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

type ProblemDto struct {
	Id string `json:"id"`
}

func (c *PantheonixClient) CreateProblem(token *BearerToken, problemId int) error {
	problem := c.config.Problems.Data[problemId]

	reqBodyJson, err := os.ReadFile(problem.CreateReqPath)
	if err != nil {
		return fmt.Errorf("failed to parse create problem request file content for problem %d: %v", problemId, err)
	}

	bodyReader := bytes.NewReader(reqBodyJson)
	req, err := http.NewRequest(http.MethodPost, c.Endpoint(c.config.Problems.Endpoints.Create), bodyReader)

	if err != nil {
		return fmt.Errorf("failed to create problem with id %d: %v", problemId, err)
	}
	req.Header.Set("Content-Type", "application/json")
	req.AddCookie(token.Cookie)

	res, err := c.Do(req)
	if err != nil {
		return fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}
	defer res.Body.Close()

	if res.StatusCode != http.StatusOK {
		return fmt.Errorf("failed to create problem: %s", res.Status)
	}

	problemDto := &ProblemDto{}
	resBody, err := io.ReadAll(res.Body)
	if err != nil {
		return fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}

	if err := json.Unmarshal(resBody, problemDto); err != nil {
		return fmt.Errorf("failed to create problem with id %d: %s", problemId, err)
	}

	// Upload tests
	tests := c.config.Problems.Data[problemId].Tests
	for testId := range len(tests) {
		if err := c.CreateTest(token, problemDto.Id, problemId, testId); err != nil {
			return err
		}
	}

	log.Printf("Successfully created problem with id %d and guid %s\n", problemId, problemDto.Id)

	return nil
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
