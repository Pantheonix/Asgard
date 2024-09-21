use crate::api::middleware::auth::JwtContext;
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
            Ok((submission, problem)) => {
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
                    dto: (submission, problem).into(),
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

#[cfg(test)]
mod tests {
    use crate::api::middleware::auth::tests::encode_jwt;
    use crate::contracts::get_submission_dtos::GetSubmissionWithTestCasesDto;
    use crate::tests::common::{Result, ROCKET_CLIENT};
    use crate::tests::submission::tests::SUBMISSIONS;
    use crate::tests::user::tests::{User, UserProfile};
    use rocket::http::{Header, Status};
    use uuid::Uuid;

    #[tokio::test]
    async fn unauthenticated_user_cannot_get_submission() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let submission = SUBMISSIONS.get("Ordinary_SumAB_Submission1")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Unauthorized,
            "Unauthenticated user cannot get submission"
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_own_submission() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Ordinary))?;
        let submission = SUBMISSIONS.get("Ordinary_SumAB_Submission1")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get own submission"
        );

        let body: GetSubmissionWithTestCasesDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submission.id, submission.id);
        assert_eq!(
            body.submission.source_code,
            Some(submission.source_code.clone())
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_other_user_submission_without_source_code_for_still_unsolved_problem(
    ) -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Admin))?;
        let submission = SUBMISSIONS.get("Ordinary_SumAB_Submission1")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get other user's submission without source code for still unsolved problem"
        );

        let body: GetSubmissionWithTestCasesDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submission.id, submission.id);
        assert_eq!(body.submission.source_code, None);

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_other_user_submission_with_source_code_for_already_solved_problem(
    ) -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Proposer))?;
        let submission = SUBMISSIONS.get("Ordinary_SumAB_Submission1")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get other user's submission with source code for already solved problem"
        );

        let body: GetSubmissionWithTestCasesDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submission.id, submission.id);
        assert_eq!(
            body.submission.source_code,
            Some(submission.source_code.clone())
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_can_get_own_submission_for_prior_published_problem() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Ordinary))?;
        let submission = SUBMISSIONS.get("Ordinary_DiffAB_Submission5")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Ok,
            "Authenticated user can get own submission for prior published problem"
        );

        let body: GetSubmissionWithTestCasesDto =
            serde_json::from_str(&response.into_string().await.unwrap())?;
        assert_eq!(body.submission.id, submission.id);
        assert_eq!(
            body.submission.source_code,
            Some(submission.source_code.clone())
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_cannot_get_non_existent_submission() -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Ordinary))?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", Uuid::new_v4().to_string()))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::NotFound,
            "Authenticated user cannot get non-existent submission"
        );

        Ok(())
    }

    #[tokio::test]
    async fn authenticated_user_cannot_get_other_user_submission_for_unpublished_problem(
    ) -> Result<()> {
        // Arrange
        let client = ROCKET_CLIENT.get().await.clone();
        let token = encode_jwt(User::get(UserProfile::Proposer))?;
        let submission = SUBMISSIONS.get("Admin_DiffAB_Submission4")?;

        // Act
        let response = client
            .get(format!("/api/submissions/{}", submission.id))
            .header(Header::new("Authorization", format!("Bearer {}", token)))
            .dispatch()
            .await;

        // Assert
        assert_eq!(
            response.status(),
            Status::Forbidden,
            "Authenticated user cannot get other user's submission for unpublished problem"
        );

        Ok(())
    }
}
