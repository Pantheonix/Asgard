use crate::domain::submission::{Submission, TestCase};
use crate::infrastructure::new_submission::{NewSubmission, NewTestCase};
use crate::schema::submissions::dsl::submissions as all_submissions;
use crate::schema::submissions_testcases::dsl::submissions_testcases as all_testcases;
use diesel::{PgConnection, RunQueryDsl};

impl Submission {
    pub fn insert(&self, conn: &mut PgConnection) -> bool {
        self.test_cases.iter().for_each(|testcase| {
            let testcase: NewTestCase = testcase.clone().into();
            TestCase::insert(&testcase, conn);
        });

        let new_submission: NewSubmission = self.clone().into();
        diesel::insert_into(all_submissions)
            .values(new_submission)
            .execute(conn)
            .is_ok()
    }
}

impl TestCase {
    fn insert(testcase: &NewTestCase, conn: &mut PgConnection) -> bool {
        diesel::insert_into(all_testcases)
            .values(testcase)
            .execute(conn)
            .is_ok()
    }
}
