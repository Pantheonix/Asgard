use std::time::SystemTime;
use diesel::{Insertable, Queryable};
use uuid::Uuid;
use crate::domain::submission::{Submission, TestCase};
use crate::schema::submissions;
use crate::schema::submissions_testcases;

#[derive(Queryable, Insertable)]
#[table_name = "submissions"]
pub(crate) struct NewSubmission {
    id: String,
    user_id: String,
    problem_id: String,
    language: String,
    source_code: String,
    status: String,
    score: i32,
    created_at: SystemTime,
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

#[derive(Queryable, Insertable)]
#[table_name = "submissions_testcases"]
pub(crate) struct NewTestCase {
    token: String,
    submission_id: String,
    testcase_id: i32,
    status: String,
    time: f32,
    memory: f32,
    score: i32,
    eval_message: Option<String>,
    stdout: Option<String>,
    stderr: Option<String>,
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
            eval_message: testcase.eval_message,
            stdout: testcase.stdout,
            stderr: testcase.stderr,
        }
    }
}
