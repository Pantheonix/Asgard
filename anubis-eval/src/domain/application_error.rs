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
    #[error("Error invoking external services")]
    HttpError {
        #[from]
        source: reqwest::Error,
    },
    #[error("Error retrieving eval metadata for problem")]
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
    #[error("Error setting items in cache")]
    CacheSetError {
        key: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error getting item from cache")]
    CacheGetError {
        key: String,
        #[source]
        source: reqwest::Error,
    },
    #[error("Error serializing json")]
    JsonSerializationError(String),
    #[error("Error deserializing json")]
    JsonDeserializationError(String),
    #[error("Error authenticating user")]
    AuthError(String),
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
            ApplicationError::HttpError { .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                "Error invoking external services".to_string(),
            )
            .respond_to(request),
            ApplicationError::EvalMetadataError { problem_id, .. } => {
                rocket::response::status::Custom(
                    rocket::http::Status::InternalServerError,
                    format!("Error retrieving eval metadata for problem {}", problem_id),
                )
                .respond_to(request)
            }
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
            ApplicationError::CacheSetError { key, .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!("Error setting items in cache for key {}", key),
            )
            .respond_to(request),
            ApplicationError::CacheGetError { key, .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                format!("Error getting item from cache for key {}", key),
            )
            .respond_to(request),
            ApplicationError::JsonSerializationError { .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                "Error serializing json".to_string(),
            )
            .respond_to(request),
            ApplicationError::JsonDeserializationError { .. } => rocket::response::status::Custom(
                rocket::http::Status::InternalServerError,
                "Error deserializing json".to_string(),
            )
            .respond_to(request),
            ApplicationError::AuthError(message) => rocket::response::status::Custom(
                rocket::http::Status::Unauthorized,
                format!("Error authenticating user: {}", message),
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
