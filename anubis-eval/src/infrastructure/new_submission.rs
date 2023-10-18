use crate::domain::submission::{Submission, TestCase};
use crate::schema::submissions;
use crate::schema::submissions_testcases;
use diesel::{Insertable, Queryable};
use std::time::SystemTime;
use uuid::Uuid;

#[derive(Debug, Clone, Queryable, Insertable)]
#[table_name = "submissions"]
pub(in crate::infrastructure) struct NewSubmission {
    pub(in crate::infrastructure) id: String,
    pub(in crate::infrastructure) user_id: String,
    pub(in crate::infrastructure) problem_id: String,
    pub(in crate::infrastructure) language: String,
    pub(in crate::infrastructure) source_code: String,
    pub(in crate::infrastructure) status: String,
    pub(in crate::infrastructure) score: i32,
    pub(in crate::infrastructure) created_at: SystemTime,
}

impl From<Submission> for NewSubmission {
    fn from(submission: Submission) -> Self {
        Self {
            id: submission.id.to_string(),
            user_id: submission.user_id.to_string(),
            problem_id: submission.problem_id.to_string(),
            language: submission.language.to_string(),
            source_code: submission.source_code.to_string(),
            status: submission.status.to_string(),
            score: submission.score,
            created_at: submission.created_at,
        }
    }
}

impl From<NewSubmission> for Submission {
    fn from(submission: NewSubmission) -> Self {
        Self {
            id: Uuid::parse_str(&submission.id).unwrap(),
            user_id: Uuid::parse_str(&submission.user_id).unwrap(),
            problem_id: Uuid::parse_str(&submission.problem_id).unwrap(),
            language: submission.language.parse().unwrap(),
            source_code: submission.source_code,
            status: submission.status.parse().unwrap(),
            score: submission.score,
            created_at: submission.created_at,
            test_cases: vec![],
        }
    }
}

#[derive(Debug, Clone, Queryable, Insertable)]
#[table_name = "submissions_testcases"]
pub(in crate::infrastructure) struct NewTestCase {
    pub(in crate::infrastructure) token: String,
    pub(in crate::infrastructure) submission_id: String,
    pub(in crate::infrastructure) testcase_id: i32,
    pub(in crate::infrastructure) status: String,
    pub(in crate::infrastructure) time: f32,
    pub(in crate::infrastructure) memory: f32,
    pub(in crate::infrastructure) score: i32,
    pub(in crate::infrastructure) expected_score: i32,
    pub(in crate::infrastructure) eval_message: Option<String>,
    pub(in crate::infrastructure) stdout: Option<String>,
    pub(in crate::infrastructure) stderr: Option<String>,
}

impl From<TestCase> for NewTestCase {
    fn from(testcase: TestCase) -> Self {
        Self {
            token: testcase.token.to_string(),
            submission_id: testcase.submission_id.to_string(),
            testcase_id: testcase.testcase_id,
            status: testcase.status.to_string(),
            time: testcase.time,
            memory: testcase.memory,
            score: testcase.score,
            expected_score: testcase.score,
            eval_message: testcase.eval_message,
            stdout: testcase.stdout,
            stderr: testcase.stderr,
        }
    }
}

impl From<NewTestCase> for TestCase {
    fn from(testcase: NewTestCase) -> Self {
        Self {
            token: Uuid::parse_str(&testcase.token).unwrap(),
            submission_id: Uuid::parse_str(&testcase.submission_id).unwrap(),
            testcase_id: testcase.testcase_id,
            status: testcase.status.parse().unwrap(),
            time: testcase.time,
            memory: testcase.memory,
            score: testcase.score,
            expected_score: testcase.expected_score,
            eval_message: testcase.eval_message,
            stdout: testcase.stdout,
            stderr: testcase.stderr,
        }
    }
}
