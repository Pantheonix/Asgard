use crate::api::middleware::auth::JwtContext;
use crate::contracts::fps_dtos::FpsSubmissionDto;
use crate::contracts::get_submissions_dtos::GetSubmissionsDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::Submission;
use crate::infrastructure::db::Db;
use rocket::{error, get, info, Responder};
use std::str::FromStr;
use uuid::Uuid;

#[derive(Responder)]
#[response(status = 200, content_type = "json")]
pub struct GetSubmissionsResponse {
    dto: GetSubmissionsDto,
}

#[get("/submissions?<fps_dto..>")]
pub async fn get_submissions(
    fps_dto: FpsSubmissionDto,
    user_ctx: JwtContext,
    db: Db,
) -> Result<GetSubmissionsResponse, ApplicationError> {
    let user_id = Uuid::from_str(user_ctx.claims.sub.as_str()).map_err(|_| {
        ApplicationError::AuthError("Failed to parse user id from token".to_string())
    })?;

    info!("Get Submissions Request: {:?}", fps_dto);

    // Filter out submissions which should not be visible to the user
    db.run(
        move |conn| match Submission::find_all(fps_dto, &user_id, conn) {
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
        },
    )
    .await
}
