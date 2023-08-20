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

type Submission struct {
	SubmissionId        uuid.UUID           `json:"submissionId"`
	UserId              uuid.UUID           `json:"userId"`
	ProblemId           uuid.UUID           `json:"problemId"`
	ProgrammingLanguage ProgrammingLanguage `json:"programmingLanguage"`
	SourceCode          string              `json:"srcCode"`
}

func (s Submission) String() string {
	return fmt.Sprintf(
		"{submissionId: %s, userId: %s, problemId: %s, programmingLanguage: %s, srcCode: %s}",
		s.SubmissionId,
		s.UserId,
		s.ProblemId,
		s.ProgrammingLanguage,
		s.SourceCode,
	)
}
