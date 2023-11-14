use crate::application::auth::JwtContext;
use crate::application::dapr_client::DaprClient;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::Submission;
use crate::infrastructure::db::Db;
use chrono::{DateTime, Utc};
use rocket::{error, get, info, Responder};
use serde::Serialize;
use std::str::FromStr;
use tokio::runtime::Handle;
use uuid::Uuid;

#[derive(Responder)]
#[response(status = 200, content_type = "json")]
pub struct GetSubmissionResponse {
    dto: GetSubmissionWithTestCasesDto,
}

#[get("/submissions/<submission_id>")]
pub async fn get_submission(
    submission_id: String,
    user_ctx: JwtContext,
    dapr_client: DaprClient,
    db: Db,
) -> Result<GetSubmissionResponse, ApplicationError> {
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).map_err(|_| {
        ApplicationError::AuthError("Failed to parse user id from token".to_string())
    })?;

    info!("Get Submission Request: {:?}", submission_id);

    db.run(move |conn| {
        match Submission::find_by_id(&submission_id, conn) {
            Ok(submission) => {
                // Check if the user is allowed to view the submission
                let handle = Handle::current();
                let _ = handle.enter();

                let eval_metadata = futures::executor::block_on(
                    dapr_client.get_eval_metadata_for_problem(&submission.problem_id()),
                )?;

                info!("Eval Metadata retrieved: {:?}", eval_metadata);

                if !eval_metadata.is_published
                    && eval_metadata.proposer_id != user_id
                    && submission.user_id() != user_id
                {
                    error!("Cannot view submission for unpublished problem");
                    return Err(ApplicationError::CannotViewSubmissionsForUnpublishedProblemError);
                }

                // Remove the source code from the submission if the user is not allowed to view it
                let submission = match submission.user_is_allowed_to_view_source_code(
                    &user_id,
                    &eval_metadata.proposer_id,
                    conn,
                ) {
                    false => {
                        info!("User is not allowed to view source code for submission");
                        submission.without_source_code()
                    }
                    true => {
                        info!("User is allowed to view source code for submission");
                        submission
                    }
                };

                info!("Submission retrieved: {:?}", submission);
                Ok(GetSubmissionResponse {
                    dto: submission.into(),
                })
            }
            Err(e) => {
                error!("Error retrieving submission: {:?}", e);
                Err(e)
            }
        }
    })
    .await
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionWithTestCasesDto {
    #[serde(flatten)]
    submission: GetSubmissionDto,
    test_cases: Vec<GetSubmissionTestCaseDto>,
}

#[rocket::async_trait]
impl<'r> rocket::response::Responder<'r, 'static> for GetSubmissionWithTestCasesDto {
    fn respond_to(self, _: &'r rocket::Request<'_>) -> rocket::response::Result<'static> {
        let json =
            serde_json::to_string(&self).unwrap_or("Failed to serialize response".to_string());

        rocket::Response::build()
            .header(rocket::http::ContentType::JSON)
            .sized_body(json.len(), std::io::Cursor::new(json))
            .ok()
    }
}

impl From<Submission> for GetSubmissionWithTestCasesDto {
    fn from(submission: Submission) -> Self {
        Self {
            submission: GetSubmissionDto {
                id: submission.id().to_string(),
                problem_id: submission.problem_id().to_string(),
                user_id: submission.user_id().to_string(),
                language: submission.language().to_string(),
                source_code: match submission.source_code().to_string().is_empty() {
                    true => None,
                    false => Some(submission.source_code().to_string()),
                },
                status: submission.status().to_string(),
                score: submission.score() as usize,
                created_at: DateTime::<Utc>::from(submission.created_at()),
                avg_time: submission.avg_time().unwrap_or(0.0),
                avg_memory: submission.avg_memory().unwrap_or(0.0),
            },
            test_cases: submission
                .test_cases()
                .into_iter()
                .map(|test_case| GetSubmissionTestCaseDto {
                    id: test_case.testcase_id() as usize,
                    status: test_case.status().to_string(),
                    time: test_case.time(),
                    memory: test_case.memory(),
                    expected_score: test_case.expected_score() as usize,
                    eval_message: test_case.eval_message().unwrap_or("".to_string()),
                    compile_output: test_case.compile_output().unwrap_or("".to_string()),
                    stdout: test_case.stdout().unwrap_or("".to_string()),
                    stderr: test_case.stderr().unwrap_or("".to_string()),
                })
                .collect(),
        }
    }
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionDto {
    id: String,
    problem_id: String,
    user_id: String,
    language: String,
    source_code: Option<String>,
    status: String,
    score: usize,
    #[serde(with = "chrono::serde::ts_seconds")]
    created_at: DateTime<Utc>,
    avg_time: f32,
    avg_memory: f32,
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionTestCaseDto {
    id: usize,
    status: String,
    time: f32,
    memory: f32,
    expected_score: usize,
    eval_message: String,
    compile_output: String,
    stdout: String,
    stderr: String,
}
