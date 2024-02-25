use crate::contracts::problem_eval_metadata_upserted_dtos::EvalMetadataForProblemDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use crate::infrastructure::db::Db;
use cloudevents::Event;
use rocket::serde::json::Json;
use rocket::{info, post};

#[post(
    "/problem-eval-metadata-upserted",
    format = "application/cloudevents+json",
    data = "<event>"
)]
pub async fn handle_problem_eval_metadata_upserted_event(
    event: Json<Event>,
    db: Db,
) -> Result<(), ApplicationError> {
    let event = event.into_inner();

    let eval_metadata: EvalMetadataForProblemDto = match event.data() {
        Some(cloudevents::Data::Json(data)) => {
            serde_json::from_value(data.clone()).map_err(|e| ApplicationError::JsonParseError {
                value: event.to_string(),
                source: e,
            })
        }
        _ => Err(ApplicationError::CloudEventParseError {
            value: event.to_string(),
            source: anyhow::anyhow!("No data found in problem eval metadata upserted event"),
        }),
    }?;

    info!(
        "Problem Eval Metadata upserted event received: {:?}",
        eval_metadata
    );

    let problem: Problem = eval_metadata.clone().into();

    db.run(move |conn| match problem.upsert(conn) {
        Ok(_) => {
            info!(
                "Persisting problem eval metadata for problem: {:?}",
                problem.id()
            );

            Ok(())
        }
        Err(e) => Err(e),
    })
    .await
}
