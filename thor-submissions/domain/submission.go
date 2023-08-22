package domain

import (
	"fmt"

	"github.com/google/uuid"
)

type ProgrammingLanguage string

const (
	C      ProgrammingLanguage = "C"
	Cpp    ProgrammingLanguage = "C++"
	Java   ProgrammingLanguage = "Java"
	CSharp ProgrammingLanguage = "C#"
	Go     ProgrammingLanguage = "Go"
	Rust   ProgrammingLanguage = "Rust"
	Dart   ProgrammingLanguage = "Dart"
)

type SubmissionStatus string

const (
	Pending   SubmissionStatus = "Pending"
	Evaluated SubmissionStatus = "Evaluated"
)

type TestExecutionResult struct {
	TestId        int      `json:"testId"`
	Time          int      `json:"time"`
	Memory        int      `json:"memory"`
	Grade         int      `json:"grade"`
	RuntimeErrors []string `json:"runtimeErrors"`
}

func (t TestExecutionResult) String() string {
	return fmt.Sprintf(
		"{\"testId\": \"%d\", \"time\": \"%d\", \"memory\": \"%d\", \"grade\": \"%d\", \"runtimeErrors\": \"%s\"}",
		t.TestId,
		t.Time,
		t.Memory,
		t.Grade,
		t.RuntimeErrors,
	)
}

type Submission struct {
	SubmissionId        uuid.UUID
	UserId              uuid.UUID
	ProblemId           uuid.UUID
	ProgrammingLanguage ProgrammingLanguage
	SourceCode          string
	Status              SubmissionStatus
	CompiletimeErrors   []string
	TestResults         []TestExecutionResult
	TotalGrade          int
}

func (s Submission) String() string {
	testResultsAsString := "["
	for _, testResult := range s.TestResults {
		testResultsAsString += testResult.String() + ","
	}
	testResultsAsString += "]"

	return fmt.Sprintf(
		"{\"submissionId\": \"%s\", \"userId\": \"%s\", \"problemId\": \"%s\", \"programmingLanguage\": \"%s\", \"sourceCode\": \"%s\", \"status\": \"%s\", \"compiletimeErrors\": \"%s\", \"testResults\": \"%s\", \"totalGrade\": \"%d\"}",
		s.SubmissionId,
		s.UserId,
		s.ProblemId,
		s.ProgrammingLanguage,
		s.SourceCode,
		s.Status,
		s.CompiletimeErrors,
		testResultsAsString,
		s.TotalGrade,
	)
}
