use crate::domain::submission::{Submission, TestCase};
use crate::infrastructure::new_submission::{NewSubmission, NewTestCase};
use crate::schema::submissions::dsl::submissions as all_submissions;
use crate::schema::submissions_testcases::dsl::submissions_testcases as all_testcases;
use diesel::{PgConnection, QueryResult, RunQueryDsl};

impl Submission {
    pub fn insert(&self, conn: &mut PgConnection) -> bool {
        // check if any of the test cases fail to insert
        let test_cases_insertion_succeeded = self
            .test_cases
            .iter()
            .filter_map(|testcase| {
                let new_testcase: NewTestCase = testcase.clone().into();
                if TestCase::insert(&new_testcase, conn) {
                    None
                } else {
                    Some(())
                }
            })
            .next()
            .is_none();

        if !test_cases_insertion_succeeded {
            return false;
        }

        let new_submission: NewSubmission = self.clone().into();
        diesel::insert_into(all_submissions)
            .values(new_submission)
            .execute(conn)
            .is_ok()
    }
}

impl TestCase {
    fn insert(testcase: &NewTestCase, conn: &mut PgConnection) -> QueryResult<usize> {
        diesel::insert_into(all_testcases)
            .values(testcase)
            .execute(conn)
    }
}
