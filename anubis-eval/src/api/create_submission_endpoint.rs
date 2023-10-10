use crate::application::auth::JwtContext;
use crate::application::dapr_client::DaprClient;
use crate::application::dapr_dtos::{CreateSubmissionBatchDto, CreateSubmissionTestCaseDto};
use crate::domain::network_response::NetworkResponse;
use rocket::futures::future::join_all;
use rocket::serde::json::Json;
use rocket::{post, Responder};
use rocket_validation::{Validate, Validated};
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

// #[derive(Responder)]
// #[response(status = 201, content_type = "json")]
// pub struct CreateSubmissionResponse {
//     id: String,
// }

#[post("/submission", format = "json", data = "<submission>")]
pub async fn create_submission(
    submission: Validated<Json<CreateSubmissionRequest>>,
    user_ctx: JwtContext,
    dapr_client: DaprClient,
) -> NetworkResponse {
    let submission = submission.into_inner();
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).unwrap();

    // ENKI - Get Eval Metadata for Problem
    let eval_metadata = dapr_client
        .get_eval_metadata_for_problem(&submission.problem_id)
        .await
        .unwrap();

    // FIREBASE - Get Problem Test Contents
    let test_cases = join_all(eval_metadata.tests.iter().map(|test| async {
        let (input, output) =
            DaprClient::get_input_and_output_for_test((test.input.clone(), test.output.clone()))
                .await
                .unwrap();

        CreateSubmissionTestCaseDto {
            source_code: submission.source_code.to_string(),
            language: submission.language.parse().unwrap(),
            stdin: input,
            time: eval_metadata.time,
            memory_limit: eval_metadata.total_memory * 1000,
            stack_limit: eval_metadata.stack_memory * 1000,
            expected_output: output,
        }
    }))
    .await;

    let submission_batch = CreateSubmissionBatchDto {
        submissions: test_cases,
    };

    // JUDGE0 - Create Submission Batch
    let submission_tokens = dapr_client
        .create_submission_batch(&submission_batch)
        .await
        .unwrap();
    println!("{:?}", submission_tokens);

    // JUDGE0 - Get Submission Results

    // POSTGRES - Create Submission
    let submission_id = Uuid::new_v4().to_string();

    NetworkResponse::Created(submission_id)
}
