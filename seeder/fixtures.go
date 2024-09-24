package main

import (
	"fmt"
	"gopkg.in/yaml.v3"
	"os"
)

type Fixtures struct {
	BaseUrl  string          `yaml:"base_url"`
	Users    UserFixtures    `yaml:"users"`
	Problems ProblemFixtures `yaml:"problems"`
}

type UserFixtures struct {
	Endpoints struct {
		Register string `yaml:"register"`
		Login    string `yaml:"login"`
	}
	Data []*UserData `yaml:"data,flow"`
}

type UserData struct {
	Email    string `yaml:"email" json:"email"`
	Password string `yaml:"password" json:"password"`
}

type ProblemFixtures struct {
	Endpoints struct {
		Create           string `yaml:"create"`
		Update           string `yaml:"update"`
		CreateTest       string `yaml:"create_test"`
		CreateSubmission string `yaml:"create_submission"`
	}
	Data []*ProblemData `yaml:"data,flow"`
}

type ProblemData struct {
	CreateReqPath string            `yaml:"create_req_path"`
	Tests         []*TestData       `yaml:"tests,flow"`
	Submissions   []*SubmissionData `yaml:"submissions,flow"`
}

type TestData struct {
	TestZipPath string `yaml:"test_zip_path"`
	Score       int    `yaml:"score"`
}

type SubmissionData struct {
	SourceCodePath string `yaml:"source_code_path"`
	Language       string `yaml:"language"`
}

// LoadFixtures loads fixtures from a YAML file
func LoadFixtures(filename string) (*Fixtures, error) {
	fixtures := &Fixtures{}

	file, err := os.Open(filename)
	defer file.Close()

	if err != nil {
		return nil, fmt.Errorf("failed to open fixtures file: %s", err)
	}

	content, err := os.ReadFile(filename)
	if err := yaml.Unmarshal(content, fixtures); err != nil {
		return nil, fmt.Errorf("failed to decode fixtures: %s", err)
	}

	return fixtures, nil
}
