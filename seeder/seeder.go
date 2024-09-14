package main

import (
	"context"
	"fmt"
	"golang.org/x/sync/errgroup"
	"log"
	"os"
)

type Seeder struct {
	client *PantheonixClient
}

func NewSeeder(client *PantheonixClient) *Seeder {
	return &Seeder{client}
}

func (s *Seeder) SeedUsers() error {
	return nil
}

func (s *Seeder) SeedProblems(ctx context.Context) error {
	admin := s.client.config.Users.Data[0]
	token, err := s.client.Login(admin)

	if err != nil {
		return err
	}

	g, ctx := errgroup.WithContext(ctx)
	problems := s.client.config.Problems.Data
	for i, problem := range problems {
		g.Go(func() error {
			problemDto, err := s.client.CreateProblem(ctx, token, i)
			if err != nil {
				return err
			}

			if err := s.SeedSubmissions(ctx, token, problemDto, problem); err != nil {
				return err
			}

			return nil
		})
	}

	if err := g.Wait(); err != nil {
		return err
	}

	log.Println("Successfully created problems and tests")

	return nil
}

func (s *Seeder) SeedSubmissions(ctx context.Context, token *BearerToken, problemDto *ProblemDto, problemData *ProblemData) error {
	g, ctx := errgroup.WithContext(ctx)
	for i, submission := range problemData.Submissions {
		g.Go(func() error {
			sourceCode, err := os.ReadFile(submission.SourceCodePath)
			if err != nil {
				return fmt.Errorf("failed to read source code for submission %d: %v", i, err)
			}

			submissionDto := &SubmissionDto{
				ProblemId:  problemDto.Id,
				Language:   submission.Language,
				SourceCode: string(sourceCode),
			}
			if err := s.client.CreateSubmission(token, submissionDto); err != nil {
				return err
			}

			return nil
		})
	}

	if err := g.Wait(); err != nil {
		return err
	}

	log.Printf("Successfully created submissions for problem %s\n", problemDto.Id)

	return nil
}
