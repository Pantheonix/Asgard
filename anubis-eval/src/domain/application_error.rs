use thiserror::Error;

#[derive(Error, Debug)]
pub enum ApplicationError {
    #[error("Error saving submission {submission_id:?} to database")]
    SubmissionSaveError {
        submission_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error saving testcase {testcase_id:?} for submission {submission_id:?} to database")]
    TestCaseSaveError {
        testcase_id: String,
        submission_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error retrieving eval metadata for problem {problem_id:?}")]
    EvalMetadataRetrievalError {
        problem_id: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error retrieving test contents for testcase {testcase_id:?}")]
    TestContentRetrievalError {
        testcase_id: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error submitting testcases for evaluation for submission {submission_id:?}")]
    TestSubmissionError {
        submission_id: String,
        #[source]
        source: reqwest::Error,
    },
}
