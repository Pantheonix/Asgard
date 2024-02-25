use rocket::response::Responder;
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
    #[error("Error retrieving input and output for test")]
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

        match self {
            ApplicationError::SubmissionSaveError { submission_id, .. } => {
                rocket::response::status::Custom(
                    rocket::http::Status::BadRequest,
                    format!("Error saving submission {} to database", submission_id),
                )
                .respond_to(request)
            }
            ApplicationError::SubmissionNotFoundError { submission_id } => {
                rocket::response::status::Custom(
                    rocket::http::Status::NotFound,
                    format!("Submission {} not found", submission_id),
                )
                .respond_to(request)
            }
            ApplicationError::SubmissionFindError { .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                "Error finding submissions".to_string(),
            )
            .respond_to(request),
            ApplicationError::TestCaseSaveError {
                testcase_id,
                submission_id,
                ..
            } => rocket::response::status::Custom(
                rocket::http::Status::BadRequest,
                format!(
                    "Error saving testcase {} for submission {} to database",
                    testcase_id, submission_id
                ),
            )
            .respond_to(request),
            ApplicationError::TestCaseFindError { .. } => rocket::response::status::Custom(
                rocket::http::Status::NotFound,
                "Error finding testcases".to_string(),
            )
            .respond_to(request),
            ApplicationError::ProblemNotFoundError { problem_id, .. } =>
                rocket::response::status::Custom(
                    rocket::http::Status::NotFound,
                    format!("Problem {} not found", problem_id),
                )
                .respond_to(request),
            ApplicationError::ProblemFindError { .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                "Error finding problems".to_string(),
            )
            .respond_to(request),
            ApplicationError::ProblemSaveError { problem_id, .. } =>
                rocket::response::status::Custom(
                    rocket::http::Status::BadRequest,
                    format!("Error saving problem eval metadata for problem {}", problem_id),
                )
                .respond_to(request),
            ApplicationError::TestSaveError { problem_id, test_id, .. } =>
                rocket::response::status::Custom(
                    rocket::http::Status::BadRequest,
                    format!("Error saving test {} for problem {}", test_id, problem_id),
                )
                .respond_to(request),
            ApplicationError::SubmissionEvaluationError { .. } =>
                rocket::response::status::Custom(
                    rocket::http::Status::InternalServerError,
                    "Submission evaluation service failed".to_string(),
                )
                .respond_to(request),
            ApplicationError::EvalMetadataError { problem_id, .. } =>
                rocket::response::status::Custom(
                    rocket::http::Status::InternalServerError,
                    format!("Error retrieving eval metadata for problem {}", problem_id),
                )
                .respond_to(request),
            ApplicationError::TestInputOutputError {
                problem_id,
                test_id,
                ..
            } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!(
                    "Error retrieving input and output for test {} of problem {}",
                    test_id, problem_id
                ),
            )
            .respond_to(request),
            ApplicationError::StateStoreGetError { key, .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!("Error getting item from state store for key {}", key),
            )
            .respond_to(request),
            ApplicationError::StateStoreSetError { key, .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!("Error setting item in state store for key {}", key),
            )
            .respond_to(request),
            ApplicationError::AuthError(message) => rocket::response::status::Custom(
                rocket::http::Status::Unauthorized,
                format!("Error authenticating user: {}", message),
            )
            .respond_to(request),
            ApplicationError::CannotSubmitForUnpublishedProblemError => rocket::response::status::Custom(
                rocket::http::Status::Forbidden,
                "Sending submissions for unpublished problems is not allowed unless you are the proposer".to_string(),
            )
            .respond_to(request),
            ApplicationError::CannotViewSubmissionsForUnpublishedProblemError => rocket::response::status::Custom(
                rocket::http::Status::Forbidden,
                "Viewing submissions for unpublished problems is not allowed unless you are the proposer".to_string(),
            )
            .respond_to(request),
            ApplicationError::JsonParseError { .. } => rocket::response::status::Custom(
                rocket::http::Status::BadRequest,
                "Failed to parse json".to_string(),
            )
            .respond_to(request),
            ApplicationError::CloudEventParseError { .. } => rocket::response::status::Custom(
                rocket::http::Status::BadRequest,
                "Failed to parse cloud event".to_string(),
            )
            .respond_to(request),
            ApplicationError::Unknown(message) => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!("Unknown error: {}", message),
            )
            .respond_to(request),
        }
    }
}
