use crate::api::middleware::auth::JwtContext;
use crate::contracts::get_highest_score_submissions_dtos::GetHighestScoreSubmissionsDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::Submission;
use crate::infrastructure::db::Db;
use rocket::{error, get, info, Responder};
use std::str::FromStr;
use uuid::Uuid;

#[derive(Responder)]
#[response(status = 200, content_type = "json")]
pub struct GetSubmissionsResponse {
    dto: GetHighestScoreSubmissionsDto,
}

#[get("/submissions/user/<user_id>?<problem_id>")]
pub async fn get_highest_score_submissions(
    user_id: String,
    problem_id: Option<String>,
    user_ctx: JwtContext,
    db: Db,
) -> Result<GetSubmissionsResponse, ApplicationError> {
    let current_user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).map_err(|_| {
        ApplicationError::AuthError("Failed to parse user id from token".to_string())
    })?;

    // Filter out submissions which should not be visible to the user
    db.run(move |conn| {
        match Submission::find_highest_score_submissions_by_user_id(
            &current_user_id,
            &user_id,
            &problem_id,
            conn,
        ) {
            Ok(submissions) => {
                info!("Submissions retrieved: {:?}", submissions);
                Ok(GetSubmissionsResponse {
                    dto: submissions.into(),
                })
            }
            Err(e) => {
                error!("Failed to get submissions: {:?}", e);
                Err(e)
            }
        }
    })
    .await
}
