use crate::application::auth::JwtContext;
use crate::application::dapr_client::DaprClient;
use crate::contracts::get_submission_dtos::GetSubmissionWithTestCasesDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::Submission;
use crate::infrastructure::db::Db;
use rocket::{error, get, info, Responder};
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
