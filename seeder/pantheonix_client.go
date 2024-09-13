package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"strings"
)

type PantheonixClient struct {
	config Fixtures
	*http.Client
}

func NewPantheonixClient(config Fixtures) PantheonixClient {
	return PantheonixClient{config, &http.Client{}}
}

func (c *PantheonixClient) Endpoint(endpoint string) string {
	return c.config.BaseUrl + endpoint
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

func (t *BearerToken) SetAccessToken(token string, cookie *http.Cookie) {
	t.AccessToken = token
	t.Cookie = cookie
}

func (c *PantheonixClient) Login(user UserData) (*BearerToken, error) {
	// Send a login request
	bodyJson, err := json.Marshal(user)
	if err != nil {
		log.Fatalf("Failed to marshal user: %s", err)
	}

	bodyReader := bytes.NewReader(bodyJson)
	resp, err := c.Post(c.Endpoint(c.config.Users.Endpoints.Login), "application/json", bodyReader)
	defer resp.Body.Close()

	if err != nil {
		log.Fatalf("Failed to send login request: %s", err)
	}

	token := NewBearerToken()
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
