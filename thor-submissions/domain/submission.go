package domain

import (
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

type Submission struct {
	SubmissionId        uuid.UUID           `json:"submissionId"`
	UserId              uuid.UUID           `json:"userId"`
	ProblemId           uuid.UUID           `json:"problemId"`
	ProgrammingLanguage ProgrammingLanguage `json:"programmingLanguage"`
	SourceCode          string              `json:"srcCode"`
	Status              SubmissionStatus    `json:"status"`
	Time                int                 `json:"time"`
	Memory              int                 `json:"memory"`
	Grade               int                 `json:"grade"`
	Errors              []string            `json:"errors"`
}
