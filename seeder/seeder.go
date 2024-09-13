package main

import (
	"log"
)

type Seeder struct {
	client *PantheonixClient
}

func NewSeeder(client *PantheonixClient) *Seeder {
	return &Seeder{client}
}

func (s *Seeder) SeedUsers() {
	// Seed users
}

func (s *Seeder) SeedProblems() error {
	admin := s.client.config.Users.Data[0]
	token, err := s.client.Login(admin)

	if err != nil {
		log.Printf("Login failed: %s", err)
		return err
	}

	problems := s.client.config.Problems.Data
	for i := range len(problems) {
		if err := s.client.CreateProblem(token, i); err != nil {
			return err
		}
	}

	log.Println("Successfully created problems and tests")

	return nil
}

func (s *Seeder) SeedSubmissions() {
	// Seed submissions
}
