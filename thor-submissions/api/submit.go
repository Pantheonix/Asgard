package api

import (
	"context"
	"fmt"
	"thor-submissions/domain"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

type SubmittedSubmissionDto struct {
	UserId              uuid.UUID                  `json:"userId"`
	ProblemId           uuid.UUID                  `json:"problemId"`
	ProgrammingLanguage domain.ProgrammingLanguage `json:"programmingLanguage"`
	SourceCode          string                     `json:"srcCode"`
}

func (s SubmittedSubmissionDto) String() string {
	return fmt.Sprintf(
		"{\"userId\": \"%s\", \"problemId\": \"%s\", \"programmingLanguage\": \"%s\", \"sourceCode\": \"%s\"}",
		s.UserId,
		s.ProblemId,
		s.ProgrammingLanguage,
		s.SourceCode,
	)
}

type QueuedSubmissionDto struct {
	SubmissionId        uuid.UUID                  `json:"submissionId"`
	UserId              uuid.UUID                  `json:"userId"`
	ProblemId           uuid.UUID                  `json:"problemId"`
	ProgrammingLanguage domain.ProgrammingLanguage `json:"programmingLanguage"`
	SourceCode          string                     `json:"srcCode"`
	Status              domain.SubmissionStatus    `json:"status"`
}

func (q QueuedSubmissionDto) String() string {
	return fmt.Sprintf(
		"{\"submissionId\": \"%s\", \"userId\": \"%s\", \"problemId\": \"%s\", \"programmingLanguage\": \"%s\", \"sourceCode\": \"%s\", \"status\": \"%s\"}",
		q.SubmissionId,
		q.UserId,
		q.ProblemId,
		q.ProgrammingLanguage,
		q.SourceCode,
		q.Status,
	)
}

func Submit(c *gin.Context, daprClient *dapr.Client) {
	var submittedSubmissionDto SubmittedSubmissionDto
	if err := c.ShouldBindJSON(&submittedSubmissionDto); err != nil {
		c.JSON(400, gin.H{"error": err.Error()})
		return
	}

	// extract to mapper
	submission := domain.Submission{
		SubmissionId:        uuid.New(),
		UserId:              submittedSubmissionDto.UserId,
		ProblemId:           submittedSubmissionDto.ProblemId,
		ProgrammingLanguage: submittedSubmissionDto.ProgrammingLanguage,
		SourceCode:          submittedSubmissionDto.SourceCode,
		Status:              domain.Pending,
	}

	// extract to mapper
	queuedSubmissionDto := QueuedSubmissionDto{
		SubmissionId:        submission.SubmissionId,
		UserId:              submission.UserId,
		ProblemId:           submission.ProblemId,
		ProgrammingLanguage: submission.ProgrammingLanguage,
		SourceCode:          submission.SourceCode,
		Status:              submission.Status,
	}

	if err := (*daprClient).PublishEvent(
		context.Background(),
		domain.PUBSUB_COMPONENT_NAME,
		domain.PUBSUB_TOPIC,
		[]byte(queuedSubmissionDto.String()),
	); err != nil {
		c.JSON(500, gin.H{"error": err.Error()})
		return
	}

	// store submission to database

	fmt.Println("Published data:", queuedSubmissionDto)
	c.JSON(200, queuedSubmissionDto)
}
