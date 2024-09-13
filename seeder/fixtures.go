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
	Data []UserData `yaml:",flow"`
}

type UserData struct {
	Email    string `yaml:"email" json:"email"`
	Password string `yaml:"password" json:"password"`
}

type ProblemFixtures struct {
	Endpoints struct {
		Create string `yaml:"create"`
	}
	Data []ProblemData `yaml:",flow"`
}

type ProblemData struct {
}

type SubmissionFixtures struct {
	Endpoints struct {
		Create string `yaml:"create"`
	}
	Data []SubmissionData `yaml:",flow"`
}

type SubmissionData struct {
}

// LoadFixtures loads fixtures from a YAML file
func LoadFixtures(filename string) Fixtures {
	file, err := os.Open(filename)

	if err != nil {
		log.Fatalf("Failed to open fixtures file: %s", err)
	}
	defer file.Close()

	content, err := os.ReadFile(filename)

	fixtures := Fixtures{}
	if err := yaml.Unmarshal(content, &fixtures); err != nil {
		log.Fatalf("Failed to decode fixtures: %s", err)
	}

	return fixtures
}
