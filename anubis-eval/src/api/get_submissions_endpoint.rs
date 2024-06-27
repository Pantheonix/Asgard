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

#[cfg(test)]
mod tests {
    use crate::api::middleware::auth::tests::encode_jwt;
    use crate::contracts::get_submissions_dtos::GetSubmissionsDto;
    use crate::tests::common::{Result, ROCKET_CLIENT};
    use crate::tests::user::tests::{User, UserProfile};
    use rocket::http::{Header, Status};
    use serial_test::serial;

    #[tokio::test]
    #[serial]
    async fn unauthenticated_user_cannot_get_submissions() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();

        // Act
        let response = client.get("/api/submissions").dispatch().await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Unauthorized,
            "Unauthenticated user cannot get submissions"
        );

        Ok(())
    }

    #[tokio::test]
    #[serial]
    async fn authenticated_user_can_get_submissions() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Ordinary))?;

        // Act
        let response = client
            .get("/api/submissions")
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Unauthenticated user cannot get submissions"
        );

        let body: GetSubmissionsDto = serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.items, 3);
        assert_eq!(body.total_pages, 1);
        assert_eq!(body.submissions.len(), 3);

        Ok(())
    }
}
