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

#[cfg(test)]
mod tests {
    use crate::api::middleware::auth::tests::encode_jwt;
    use crate::contracts::get_highest_score_submissions_dtos::GetHighestScoreSubmissionsDto;
    use crate::tests::common::{Result, ROCKET_CLIENT};
    use crate::tests::problem::tests::PROBLEMS;
    use crate::tests::user::tests::{User, UserProfile};
    use rocket::http::{Header, Status};

    #[tokio::test]
    async fn unauthenticated_user_cannot_get_submissions() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let user_profile = User::get(UserProfile::Ordinary);

        // Act
        let response = client
            .get(format!("/api/submissions/user/{}", user_profile.id))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Unauthorized,
            "Unauthenticated user cannot get highest score submissions"
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_own_submissions() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let user_profile = User::get(UserProfile::Ordinary);
        let token = encode_jwt(user_profile.clone())?;

        // Act
        let response = client
            .get(format!("/api/submissions/user/{}", user_profile.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get own highest score submissions"
        );

        let body: GetHighestScoreSubmissionsDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submissions.len(), 2);

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_submissions_for_specific_problem() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let user_profile = User::get(UserProfile::Ordinary);
        let problem = PROBLEMS.get("SumAB")?;
        let token = encode_jwt(user_profile.clone())?;

        // Act
        let response = client
            .get(format!(
                "/api/submissions/user/{}?problem_id={}",
                user_profile.id, problem.id
            ))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get highest score submissions for specific problem"
        );

        let body: GetHighestScoreSubmissionsDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submissions.len(), 1);

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_submissions_as_proposer() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let user_profile = User::get(UserProfile::Ordinary);
        let token = encode_jwt(User::get(UserProfile::Admin))?;

        // Act
        let response = client
            .get(format!("/api/submissions/user/{}", user_profile.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get highest score submissions as proposer"
        );

        let body: GetHighestScoreSubmissionsDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submissions.len(), 2);

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_submissions_only_for_published_problems_if_not_proposer(
    ) -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let user_profile = User::get(UserProfile::Ordinary);
        let token = encode_jwt(User::get(UserProfile::Proposer))?;

        // Act
        let response = client
            .get(format!("/api/submissions/user/{}", user_profile.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get highest score submissions only for published problems if not proposer"
        );

        let body: GetHighestScoreSubmissionsDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submissions.len(), 1);

        Ok(())
    }
}
