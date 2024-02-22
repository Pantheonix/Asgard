use crate::application::auth::JwtContext;
use crate::application::dapr_client::DaprClient;
use crate::config::di::CONFIG;
use crate::contracts::create_submission_dtos::CreateSubmissionResponseDto;
use crate::contracts::dapr_dtos::{CreateSubmissionBatchDto, CreateSubmissionTestCaseDto};
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Language, Submission, TestCase, TestCaseStatus};
use crate::infrastructure::db::Db;
use rocket::futures::future::join_all;
use rocket::serde::json::Json;
use rocket::{debug, error, info, post, Responder};
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

#[derive(Responder)]
#[response(status = 201, content_type = "json")]
pub struct CreateSubmissionResponse {
    dto: CreateSubmissionResponseDto,
}

#[post("/submissions", format = "json", data = "<submission>")]
pub async fn create_submission(
    submission: Validated<Json<CreateSubmissionRequest>>,
    user_ctx: JwtContext,
    dapr_client: DaprClient,
    db: Db,
) -> Result<CreateSubmissionResponse, ApplicationError> {
    info!("Create Submission Request: {:?}", submission);

    let submission = submission.into_inner();
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).map_err(|_| {
        ApplicationError::AuthError("Failed to parse user id from token".to_string())
    })?;
    let language: Language = submission.language.clone().into();

    // ENKI - Get Eval Metadata for Problem
    let eval_metadata = dapr_client
        .get_eval_metadata_for_problem(&submission.problem_id)
        .await?;

    info!("Eval Metadata retrieved: {:?}", eval_metadata);

    // Check if the submission is allowed to be sent for the problem
    debug!(
        "is_published: {}, proposer_id: {}, user_id: {}",
        eval_metadata.is_published, eval_metadata.proposer_id, user_id
    );

    if !eval_metadata.is_published && eval_metadata.proposer_id != user_id {
        error!("Cannot submit for unpublished problem");
        return Err(ApplicationError::CannotSubmitForUnpublishedProblemError);
    }

    // FIREBASE - Get Problem Test Contents
    let mut test_cases = join_all(eval_metadata.tests.iter().map(|test| async {
        let (input, output) = dapr_client
            .get_input_and_output_for_test(
                test.test_id,
                eval_metadata.problem_id,
                (test.input.clone(), test.output.clone()),
            )
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
    .await
    .into_iter()
    .collect::<Result<Vec<_>, ApplicationError>>()?;

    test_cases.sort_by(|a, b| a.testcase_id.cmp(&b.testcase_id));

    let submission_batch = CreateSubmissionBatchDto {
        submissions: test_cases,
    };

    info!("Creating Submission Batch");

    // JUDGE0 - Create Submission Batch
    let submission_tokens = join_all(
        submission_batch
            .submissions
            .chunks(CONFIG.eval_batch_size as usize)
            .map(|submission_batch| async {
                dapr_client
                    .create_submission_batch(&CreateSubmissionBatchDto {
                        submissions: submission_batch.to_vec(),
                    })
                    .await
            }),
    )
    .await
    .into_iter()
    .collect::<Result<Vec<_>, ApplicationError>>()?
    .into_iter()
    .flatten()
    .collect::<Vec<_>>();

    info!(
        "Submission Tokens received from Judge0 Evaluator: {:?}",
        submission_tokens
    );

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

    info!("Saving Submission to database: {:?}", submission.id());

    db.run(move |conn| match submission.insert(conn) {
        Ok(_) => {
            info!("Submission saved to database");
            Ok(CreateSubmissionResponse {
                dto: CreateSubmissionResponseDto {
                    id: submission.id().to_string(),
                },
            })
        }
        Err(e) => {
            error!("Error saving submission to database: {:?}", e);
            Err(e)
        }
    })
    .await
}
