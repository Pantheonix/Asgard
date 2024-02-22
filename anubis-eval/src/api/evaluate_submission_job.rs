use crate::application::dapr_client::DaprClient;
use crate::config::di::Atomic;
use crate::contracts::dapr_dtos::TestCaseTokenDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Submission, SubmissionStatus, TestCase, TestCaseStatus};
use diesel::PgConnection;
use futures::future::join_all;
use rocket::error;
use rocket::log::private::{debug, info};
use std::ops::DerefMut;
use std::str::FromStr;
use uuid::Uuid;

pub async fn evaluate_pending_submissions(
    dapr_client: Atomic<DaprClient>,
    arc_db: Atomic<PgConnection>,
) -> Result<(), ApplicationError> {
    info!("Evaluating pending submissions...");

    // DB - get all pending submissions
    let pending_submissions_ids = {
        let arc_db = arc_db.clone();
        let mut db = arc_db.lock().await;

        let pending_submissions_ids =
            Submission::find_by_status(SubmissionStatus::Evaluating, db.deref_mut())?;
        debug!("Pending submissions: {:?}", pending_submissions_ids);

        pending_submissions_ids
    };

    // DB - for each submission, get the pending/running test cases
    join_all(pending_submissions_ids.iter().map(|submission_id| async {
        evaluate_submission(submission_id, dapr_client.clone(), arc_db.clone()).await
    }))
    .await
    .into_iter()
    .filter(|result| result.is_err())
    .for_each(|e| error!("Failed to evaluate submission: {:?}", e));

    Ok(())
}

async fn evaluate_submission(
    submission_id: &String,
    dapr_client: Atomic<DaprClient>,
    db: Atomic<PgConnection>,
) -> Result<(), ApplicationError> {
    info!("Evaluating submission: {}", submission_id);

    let mut db = db.lock().await;
    let dapr_client = dapr_client.lock().await;

    let pending_test_cases_tokens = TestCase::find_by_status_and_submission_id(
        TestCaseStatus::Pending,
        submission_id,
        db.deref_mut(),
    )?;
    let running_test_cases_tokens = TestCase::find_by_status_and_submission_id(
        TestCaseStatus::Running,
        submission_id,
        db.deref_mut(),
    )?;

    let pending_test_cases_tokens = pending_test_cases_tokens
        .into_iter()
        .chain(running_test_cases_tokens.into_iter())
        .map(|token| TestCaseTokenDto { token })
        .collect::<Vec<_>>();
    debug!(
        "Pending test cases for submission {:?}: {:?}",
        submission_id, pending_test_cases_tokens
    );

    if pending_test_cases_tokens.is_empty() {
        // decide if submission is accepted or rejected
        let testcases = TestCase::find_by_submission_id(submission_id, db.deref_mut())?;

        let submission_status = if testcases
            .iter()
            .all(|testcase| testcase.status() == TestCaseStatus::Accepted)
        {
            SubmissionStatus::Accepted
        } else {
            SubmissionStatus::Rejected
        };

        let submission_score = testcases
            .iter()
            .map(|testcase| match testcase.status() {
                TestCaseStatus::Accepted => testcase.expected_score(),
                _ => 0,
            })
            .sum::<i32>();

        let submission_avg_time = testcases
            .iter()
            .map(|testcase| testcase.time())
            .sum::<f32>()
            / testcases.len() as f32;

        let submission_avg_memory = testcases
            .iter()
            .map(|testcase| testcase.memory())
            .sum::<f32>()
            / testcases.len() as f32;

        let (submission_avg_time, submission_avg_memory) = match submission_status {
            SubmissionStatus::Accepted => (Some(submission_avg_time), Some(submission_avg_memory)),
            _ => (None, None),
        };

        let submission = Submission::new_with_metadata(
            Uuid::from_str(submission_id).unwrap(),
            submission_status,
            submission_score,
            submission_avg_time,
            submission_avg_memory,
        );

        // DB - update submission
        submission.update_evaluation_metadata(db.deref_mut())?;
    } else {
        // JUDGE0 - get test case results
        let test_cases_results = dapr_client
            .get_submission_batch(&pending_test_cases_tokens)
            .await?;
        debug!(
            "Test case results for submission {:?}: {:?}",
            submission_id, test_cases_results
        );

        // DB - construct submission and test cases from DTOs and update them
        let test_cases = test_cases_results
            .submissions
            .into_iter()
            .map(|submission_testcase| submission_testcase.into())
            .collect::<Vec<TestCase>>();
        TestCase::update_testcases(test_cases, db.deref_mut())?;
    }

    Ok(())
}
