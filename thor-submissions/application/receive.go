package application

import (
	"fmt"
	"thor-submissions/domain"

	cloudevents "github.com/cloudevents/sdk-go/v2"
	dapr "github.com/dapr/go-sdk/client"
	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

type ReceivedSubmissionDto struct {
	SubmissionId        uuid.UUID                    `json:"submissionId"`
	UserId              uuid.UUID                    `json:"userId"`
	ProblemId           uuid.UUID                    `json:"problemId"`
	ProgrammingLanguage domain.ProgrammingLanguage   `json:"programmingLanguage"`
	SourceCode          string                       `json:"srcCode"`
	Status              domain.SubmissionStatus      `json:"status"`
	CompiletimeErrors   []string                     `json:"compiletimeErrors"`
	TestResults         []domain.TestExecutionResult `json:"testResults"`
	TotalGrade          int                          `json:"totalGrade"`
}

func (r ReceivedSubmissionDto) String() string {
	testResultsAsString := "["
	for _, testResult := range r.TestResults {
		testResultsAsString += testResult.String() + ","
	}
	testResultsAsString += "]"

	return fmt.Sprintf(
		"{\"submissionId\": \"%s\", \"userId\": \"%s\", \"problemId\": \"%s\", \"programmingLanguage\": \"%s\", \"sourceCode\": \"%s\", \"status\": \"%s\", \"compiletimeErrors\": \"%s\", \"testResults\": \"%s\", \"totalGrade\": \"%d\"}",
		r.SubmissionId,
		r.UserId,
		r.ProblemId,
		r.ProgrammingLanguage,
		r.SourceCode,
		r.Status,
		r.CompiletimeErrors,
		testResultsAsString,
		r.TotalGrade,
	)
}

func Receive(c *gin.Context, daprClient *dapr.Client) {
	// bind cloud event to dto
	event, err := cloudevents.NewEventFromHTTPRequest(c.Request)
	if err != nil {
		fmt.Println("Error: ", err)
		c.JSON(400, gin.H{"error": err.Error()})
		return
	}

	var receivedSubmissionDto ReceivedSubmissionDto
	if err := event.DataAs(&receivedSubmissionDto); err != nil {
		fmt.Println("Error: ", err)
		c.JSON(400, gin.H{"error": err.Error()})
		return
	}

	// extract to mapper
	submission := domain.Submission{
		SubmissionId:        receivedSubmissionDto.SubmissionId,
		UserId:              receivedSubmissionDto.UserId,
		ProblemId:           receivedSubmissionDto.ProblemId,
		ProgrammingLanguage: receivedSubmissionDto.ProgrammingLanguage,
		SourceCode:          receivedSubmissionDto.SourceCode,
		Status:              receivedSubmissionDto.Status,
		CompiletimeErrors:   receivedSubmissionDto.CompiletimeErrors,
		TestResults:         receivedSubmissionDto.TestResults,
		TotalGrade:          receivedSubmissionDto.TotalGrade,
	}

	// update submission from db

	fmt.Println("Received submission: ", submission.String())
	c.JSON(200, gin.H{"submission": submission})
	return
}
