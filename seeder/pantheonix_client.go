package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
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
		log.Printf("Failed to marshal user: %s", err)
		return token, err
	}

	bodyReader := bytes.NewReader(bodyJson)
	resp, err := c.Post(c.Endpoint(c.config.Users.Endpoints.Login), "application/json", bodyReader)
	defer resp.Body.Close()

	if err != nil {
		log.Printf("Failed to send login request: %s", err)
		return token, err
	}

	if resp.StatusCode != http.StatusOK {
		return token, fmt.Errorf("failed to login: %s", resp.Status)
	}

	// Extract the bearer token from the response
	cookies := resp.Cookies()

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
	reqBodyJson, err := os.ReadFile(c.config.Problems.Data[problemId].CreateReqPath)
	if err != nil {
		log.Printf("Failed to parse create problem request file content for problem %d: %v", problemId, err)
		return err
	}

	bodyReader := bytes.NewReader(reqBodyJson)
	req, err := http.NewRequest(http.MethodPost, c.Endpoint(c.config.Problems.Endpoints.Create), bodyReader)

	if err != nil {
		log.Printf("Failed to create problem with id %d: %v", problemId, err)
		return err
	}
	req.Header.Set("Content-Type", "application/json")
	req.AddCookie(token.Cookie)

	resp, err := c.Do(req)
	if err != nil {
		log.Printf("Failed to create problem with id %d: %v", problemId, err)
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("failed to create problem: %s", resp.Status)
	}

	problemDto := &ProblemDto{}
	respBody, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Printf("Failed to read create problem response body: %s", err)
		return err
	}

	if err := json.Unmarshal(respBody, problemDto); err != nil {
		log.Printf("Failed to deserialize create problem response body: %s", err)
		return err
	}

	fmt.Println(problemDto.Id)

	return nil
}

func (c *PantheonixClient) CreateTest(token *BearerToken, testId int) error {
	return nil
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
