use crate::application::auth::JwtContext;
use crate::application::dapr_client::DaprClient;
use crate::application::dapr_dtos::{CreateSubmissionBatchDto, CreateSubmissionTestCaseDto};
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Language, Submission, TestCase, TestCaseStatus};
use crate::infrastructure::db::Db;
use rocket::futures::future::join_all;
use rocket::serde::json::Json;
use rocket::{post, Responder};
use rocket_validation::{Validate, Validated};
use serde::Serialize;
use std::str::FromStr;
use uuid::Uuid;

#[derive(Debug, serde::Deserialize, Validate)]
#[serde(crate = "rocket::serde")]
pub struct CreateSubmissionRequest {
    problem_id: Uuid,
    language: String,
    #[validate(length(min = 1, max = 10000))]
    source_code: String,
}

#[derive(Responder)]
#[response(status = 201, content_type = "json")]
pub struct CreateSubmissionResponse {
    dto: CreateSubmissionResponseDto,
}

#[post("/submission", format = "json", data = "<submission>")]
pub async fn create_submission(
    submission: Validated<Json<CreateSubmissionRequest>>,
    user_ctx: JwtContext,
    dapr_client: DaprClient,
    db: Db,
) -> Result<CreateSubmissionResponse, ApplicationError> {
    let submission = submission.into_inner();
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).unwrap();
    let language: Language = submission.language.clone().into();

    // ENKI - Get Eval Metadata for Problem
    let eval_metadata = dapr_client
        .get_eval_metadata_for_problem(&submission.problem_id)
        .await?;

    // FIREBASE - Get Problem Test Contents
    let test_cases = join_all(eval_metadata.tests.iter().map(|test| async {
        let (input, output) =
            DaprClient::get_input_and_output_for_test((test.input.clone(), test.output.clone()))
                .await?;

        Ok(CreateSubmissionTestCaseDto {
            testcase_id: test.test_id,
            source_code: submission.source_code.to_string(),
            language: language.clone().into(),
            stdin: input,
            time: eval_metadata.time,
            memory_limit: eval_metadata.total_memory * 1000_f32,
            stack_limit: eval_metadata.stack_memory * 1000_f32,
            expected_output: output,
        })
    }))
    .await;

    let mut test_cases = test_cases
        .into_iter()
        .map(|test| test.map_err(|e| ApplicationError::HttpError { source: e }))
        .collect::<Result<Vec<_>, _>>()?;

    test_cases.sort_by(|a, b| a.testcase_id.cmp(&b.testcase_id));

    let submission_batch = CreateSubmissionBatchDto {
        submissions: test_cases,
    };

    // JUDGE0 - Create Submission Batch
    let submission_tokens = dapr_client
        .create_submission_batch(&submission_batch)
        .await?;

    // POSTGRES - Create Submission and mark it as pending alongside its test cases
    let submission_id = Uuid::new_v4();
    let test_cases = submission_tokens
        .iter()
        .zip(eval_metadata.tests.iter())
        .map(|(token_dto, test_dto)| {
            TestCase::new(
                Uuid::from_str(&token_dto.token).unwrap(),
                submission_id,
                test_dto.test_id as i32,
                TestCaseStatus::Pending,
                0_f32,
                0_f32,
                test_dto.score as i32,
                None,
                None,
                None,
                None,
            )
        })
        .collect::<Vec<TestCase>>();

    let submission = Submission::new_in_pending(
        submission_id,
        user_id,
        submission.problem_id,
        language,
        submission.source_code.clone(),
        test_cases,
    );

    db.run(move |conn| match submission.insert(conn) {
        Ok(_) => Ok(CreateSubmissionResponse {
            dto: CreateSubmissionResponseDto {
                id: submission.id().to_string(),
            },
        }),
        Err(e) => Err(e),
    })
    .await
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct CreateSubmissionResponseDto {
    id: String,
}

#[rocket::async_trait]
impl<'r> rocket::response::Responder<'r, 'static> for CreateSubmissionResponseDto {
    fn respond_to(self, _: &'r rocket::Request<'_>) -> rocket::response::Result<'static> {
        let json =
            serde_json::to_string(&self).unwrap_or("Failed to serialize response".to_string());

        rocket::Response::build()
            .header(rocket::http::ContentType::JSON)
            .sized_body(json.len(), std::io::Cursor::new(json))
            .ok()
    }
}
