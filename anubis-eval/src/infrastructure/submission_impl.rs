use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Submission, TestCase};
use crate::infrastructure::new_submission::{NewSubmission, NewTestCase};
use crate::schema::submissions::dsl::submissions as all_submissions;
use crate::schema::submissions_testcases::dsl::submissions_testcases as all_testcases;
use diesel::{PgConnection, RunQueryDsl};

impl Submission {
    pub fn insert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        // check if submission fails to insert
        let new_submission: NewSubmission = self.clone().into();
        diesel::insert_into(all_submissions)
            .values(new_submission)
            .execute(conn)
            .map_err(|source| ApplicationError::SubmissionSaveError {
                submission_id: self.id.to_string(),
                source,
            })?;

        // check if any of the test cases fail to insert
        self.test_cases
            .iter()
            .map(|testcase| {
                let testcase: NewTestCase = testcase.clone().into();
                TestCase::insert(testcase, conn)
            })
            .collect::<Result<Vec<_>, _>>()?;

        Ok(())
    }
}

impl TestCase {
    fn insert(testcase: NewTestCase, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        diesel::insert_into(all_testcases)
            .values(testcase.clone())
            .execute(conn)
            .map_err(|source| ApplicationError::TestCaseSaveError {
                testcase_id: testcase.testcase_id.to_string(),
                submission_id: testcase.submission_id.clone(),
                source,
            })?;

        Ok(())
    }
}
