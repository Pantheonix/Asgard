use crate::application::auth::JwtContext;
use rocket::serde::json::Json;
use rocket::{post, Responder};
use rocket_validation::{Validate, Validated};
use std::str::FromStr;
use uuid::Uuid;
use crate::application::dapr_client::DaprClient;
use crate::domain::network_response::NetworkResponse;

#[derive(Debug, serde::Deserialize, Validate)]
#[serde(crate = "rocket::serde")]
pub struct CreateSubmissionRequest<'r> {
    problem_id: Uuid,
    language: &'r str,
    #[validate(length(min = 1, max = 10000))]
    source_code: &'r str,
}

// #[derive(Responder)]
// #[response(status = 201, content_type = "json")]
// pub struct CreateSubmissionResponse {
//     id: String,
// }

#[post("/submission", format = "json", data = "<submission>")]
pub async fn create_submission(
    user_ctx: JwtContext,
    dapr_client: DaprClient,
    submission: Validated<Json<CreateSubmissionRequest<'_>>>,
) -> NetworkResponse {
    let submission = submission.into_inner();
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).unwrap();

    let eval_metadata = dapr_client
        .get_eval_metadata_for_problem(&submission.problem_id)
        .await
        .unwrap();
    println!("eval_metadata: {:?}", eval_metadata);

    let submission_id = Uuid::new_v4().to_string();

    NetworkResponse::Created(submission_id)
}
