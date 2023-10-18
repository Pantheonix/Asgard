use crate::application::dapr_client::DaprClient;
use crate::application::dapr_dtos::TestCaseTokenDto;
use crate::config::di::Atomic;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Submission, SubmissionStatus, TestCase, TestCaseStatus};
use diesel::PgConnection;
use futures::future::join_all;
use rocket::error;
use rocket::log::private::{debug, info};
use std::ops::DerefMut;

pub async fn evaluate_pending_submissions(
    dapr_client: Atomic<DaprClient>,
    arc_db: Atomic<PgConnection>,
) -> Result<(), ApplicationError> {
    info!("Evaluating pending submissions...");

    // DB - get all pending submissions
    let pending_submissions_tokens = {
        let arc_db = arc_db.clone();
        let mut db = arc_db.lock().await;
        
        let pending_submissions_tokens = Submission::find_by_status(SubmissionStatus::Evaluating, db.deref_mut())?;
        debug!("Pending submissions: {:?}", pending_submissions_tokens);
        
        pending_submissions_tokens
    };

    // DB - for each submission, get the pending/running test cases
    join_all(
        pending_submissions_tokens
            .iter()
            .map(|submission_token| async {
                evaluate_submission(submission_token, dapr_client.clone(), arc_db.clone()).await
            }),
    )
    .await
    .into_iter()
    .filter(|result| result.is_err())
    .for_each(|e| error!("Failed to evaluate submission: {:?}", e));

    Ok(())
}

async fn evaluate_submission(
    submission_token: &String,
    dapr_client: Atomic<DaprClient>,
    db: Atomic<PgConnection>,
) -> Result<(), ApplicationError> {
    info!("Evaluating submission: {}", submission_token);

    let mut db = db.lock().await;
    let dapr_client = dapr_client.lock().await;

    let pending_test_cases_tokens =
        TestCase::find_by_status_and_submission_id(TestCaseStatus::Pending, submission_token, db.deref_mut())?;
    let running_test_cases_tokens =
        TestCase::find_by_status_and_submission_id(TestCaseStatus::Running, submission_token, db.deref_mut())?;

    let test_cases_tokens = pending_test_cases_tokens
        .into_iter()
        .chain(running_test_cases_tokens.into_iter())
        .map(|token| TestCaseTokenDto { token })
        .collect::<Vec<_>>();
    debug!(
        "Pending test cases for submission {:?}: {:?}",
        submission_token, test_cases_tokens
    );

    // JUDGE0 - get test case results
    let test_cases_results = dapr_client.get_submission_batch(&test_cases_tokens).await?;
    debug!(
        "Test case results for submission {:?}: {:?}",
        submission_token, test_cases_results
    );

    Ok(())
}
