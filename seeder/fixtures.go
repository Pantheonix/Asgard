package main

import (
	"gopkg.in/yaml.v3"
	"log"
	"os"
)

type Fixtures struct {
	BaseUrl     string             `yaml:"base_url"`
	Users       UserFixtures       `yaml:"users"`
	Problems    ProblemFixtures    `yaml:"problems"`
	Submissions SubmissionFixtures `yaml:"submissions"`
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
		Create     string `yaml:"create"`
		CreateTest string `yaml:"create_test"`
	}
	Data []*ProblemData `yaml:"data,flow"`
}

type ProblemData struct {
	CreateReqPath string      `yaml:"create_req_path"`
	Tests         []*TestData `yaml:"tests,flow"`
}

type TestData struct {
	TestZipPath string `yaml:"test_zip_path"`
	Score       int    `yaml:"score"`
}

type SubmissionFixtures struct {
	Endpoints struct {
		Create string `yaml:"create"`
	}
	Data []*SubmissionData `yaml:"data,flow"`
}

type SubmissionData struct {
}

// LoadFixtures loads fixtures from a YAML file
func LoadFixtures(filename string) *Fixtures {
	file, err := os.Open(filename)
	defer file.Close()

	if err != nil {
		log.Printf("Failed to open fixtures file: %s", err)
	}

	content, err := os.ReadFile(filename)

	fixtures := &Fixtures{}
	if err := yaml.Unmarshal(content, fixtures); err != nil {
		log.Printf("Failed to decode fixtures: %s", err)
	}

	return fixtures
}
