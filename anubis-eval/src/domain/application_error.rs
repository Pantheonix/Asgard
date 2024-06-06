use rocket::response::Responder;
use rocket::serde::json::{json, Value};
use rocket::{error, Request};
use thiserror::Error;

#[derive(Error, Debug)]
pub enum ApplicationError {
    #[error("Error saving submission {submission_id:?} to database")]
    SubmissionSaveError {
        submission_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error finding submissions")]
    SubmissionFindError {
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error finding submission {submission_id:?}")]
    SubmissionNotFoundError { submission_id: String },
    #[error("Error saving testcase {testcase_id:?} for submission {submission_id:?} to database")]
    TestCaseSaveError {
        testcase_id: String,
        submission_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error finding testcases")]
    TestCaseFindError {
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error saving problem eval metadata")]
    ProblemSaveError {
        problem_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error finding problems")]
    ProblemFindError {
        #[source]
        source: diesel::result::Error,
    },
    #[error("Error finding problem {problem_id:?}")]
    ProblemNotFoundError { problem_id: String },
    #[error("Error saving test {test_id:?} for problem {problem_id:?} to database")]
    TestSaveError {
        test_id: String,
        problem_id: String,
        #[source]
        source: diesel::result::Error,
    },
    #[error("Submission evaluation service failed")]
    SubmissionEvaluationError {
        #[from]
        source: reqwest::Error,
    },
    #[error("Error retrieving eval metadata for problem {problem_id:?}")]
    EvalMetadataError {
        problem_id: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error retrieving input and output for test {test_id:?} of problem {problem_id:?}")]
    TestInputOutputError {
        problem_id: String,
        test_id: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error getting item from state store for key {key:?}")]
    StateStoreGetError { key: String },
    #[error("Error setting item in state store for key {key:?}")]
    StateStoreSetError { key: String },
    #[error("Error authenticating user")]
    AuthError(String),
    #[error(
        "Sending submissions for unpublished problems is not allowed unless you are the proposer"
    )]
    CannotSubmitForUnpublishedProblemError,
    #[error(
        "Viewing submissions for unpublished problems is not allowed unless you are the proposer"
    )]
    CannotViewSubmissionsForUnpublishedProblemError,
    #[error("Failed to parse json: {value:?}")]
    JsonParseError {
        value: String,
        #[source]
        source: serde_json::Error,
    },
    #[error("Failed to parse cloud event: {value:?}")]
    CloudEventParseError {
        value: String,
        #[source]
        source: anyhow::Error,
    },
    #[error("Unknown error")]
    Unknown(String),
}

impl<'r, 'o: 'r> Responder<'r, 'o> for ApplicationError {
    fn respond_to(self, request: &'r Request<'_>) -> rocket::response::Result<'o> {
        error!("Application Error: {:?}", self);
        let error_json = json!({"error": self.to_string()});

        match self {
            ApplicationError::SubmissionSaveError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::SubmissionNotFoundError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::NotFound,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::SubmissionFindError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::TestCaseSaveError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::TestCaseFindError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::ProblemNotFoundError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::NotFound,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::ProblemFindError { .. } => rocket::response::status::Custom::<Value>(
                rocket::http::Status::InternalServerError,
                error_json,
            )
            .respond_to(request),
            ApplicationError::ProblemSaveError { .. } => rocket::response::status::Custom::<Value>(
                rocket::http::Status::InternalServerError,
                error_json,
            )
            .respond_to(request),
            ApplicationError::TestSaveError { .. } => rocket::response::status::Custom::<Value>(
                rocket::http::Status::InternalServerError,
                error_json,
            )
            .respond_to(request),
            ApplicationError::SubmissionEvaluationError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::EvalMetadataError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::TestInputOutputError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::StateStoreGetError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::StateStoreSetError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::AuthError(_) => rocket::response::status::Custom::<Value>(
                rocket::http::Status::Unauthorized,
                error_json,
            )
            .respond_to(request),
            ApplicationError::CannotSubmitForUnpublishedProblemError => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::Forbidden,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::CannotViewSubmissionsForUnpublishedProblemError => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::Forbidden,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::JsonParseError { .. } => rocket::response::status::Custom::<Value>(
                rocket::http::Status::InternalServerError,
                error_json,
            )
            .respond_to(request),
            ApplicationError::CloudEventParseError { .. } => {
                rocket::response::status::Custom::<Value>(
                    rocket::http::Status::InternalServerError,
                    error_json,
                )
                .respond_to(request)
            }
            ApplicationError::Unknown(_) => rocket::response::status::Custom::<Value>(
                rocket::http::Status::InternalServerError,
                error_json,
            )
            .respond_to(request),
        }
    }
}
