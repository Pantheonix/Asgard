use crate::domain::submission::{Submission, TestCase};
use crate::schema::submissions;
use crate::schema::submissions_testcases;
use diesel::{AsChangeset, Associations, Identifiable, Insertable, Queryable, Selectable};
use std::time::SystemTime;
use uuid::Uuid;

#[derive(Debug, Clone, PartialEq, Queryable, Insertable, Identifiable, AsChangeset, Selectable)]
#[diesel(table_name = submissions)]
pub(in crate::infrastructure) struct SubmissionModel {
    pub(in crate::infrastructure) language: String,
    pub(in crate::infrastructure) source_code: String,
    pub(in crate::infrastructure) status: String,
    pub(in crate::infrastructure) score: i32,
    pub(in crate::infrastructure) created_at: SystemTime,
    pub(in crate::infrastructure) id: String,
    pub(in crate::infrastructure) user_id: String,
    pub(in crate::infrastructure) problem_id: String,
    pub(in crate::infrastructure) avg_time: Option<f32>,
    pub(in crate::infrastructure) avg_memory: Option<f32>,
}

impl From<Submission> for SubmissionModel {
    fn from(submission: Submission) -> Self {
        Self {
            id: submission.id().to_string(),
            user_id: submission.user_id().to_string(),
            problem_id: submission.problem_id().to_string(),
            language: submission.language().to_string(),
            source_code: submission.source_code().to_string(),
            status: submission.status().to_string(),
            score: submission.score(),
            created_at: submission.created_at(),
            avg_time: submission.avg_time(),
            avg_memory: submission.avg_memory(),
        }
    }
}

impl From<SubmissionModel> for Submission {
    fn from(submission: SubmissionModel) -> Self {
        Submission::new(
            Uuid::parse_str(&submission.id).unwrap(),
            Uuid::parse_str(&submission.user_id).unwrap(),
            Uuid::parse_str(&submission.problem_id).unwrap(),
            submission.language.into(),
            submission.source_code,
            submission.status.into(),
            submission.score,
            submission.created_at,
            submission.avg_time,
            submission.avg_memory,
        )
    }
}

#[derive(Debug, Clone, PartialEq, Queryable, Insertable, Identifiable, AsChangeset, Selectable, Associations)]
#[diesel(belongs_to(SubmissionModel, foreign_key = submission_id))]
#[diesel(table_name = submissions_testcases)]
#[diesel(primary_key(token))]
pub(in crate::infrastructure) struct TestCaseModel {
    pub(in crate::infrastructure) status: String,
    pub(in crate::infrastructure) time: f32,
    pub(in crate::infrastructure) memory: f32,
    pub(in crate::infrastructure) eval_message: Option<String>,
    pub(in crate::infrastructure) compile_output: Option<String>,
    pub(in crate::infrastructure) stdout: Option<String>,
    pub(in crate::infrastructure) stderr: Option<String>,
    pub(in crate::infrastructure) token: String,
    pub(in crate::infrastructure) submission_id: String,
    pub(in crate::infrastructure) testcase_id: i32,
    pub(in crate::infrastructure) expected_score: i32,
}

impl From<TestCase> for TestCaseModel {
    fn from(testcase: TestCase) -> Self {
        Self {
            token: testcase.token().to_string(),
            submission_id: testcase.submission_id().to_string(),
            testcase_id: testcase.testcase_id(),
            status: testcase.status().to_string(),
            time: testcase.time(),
            memory: testcase.memory(),
            expected_score: testcase.expected_score(),
            eval_message: testcase.eval_message(),
            compile_output: testcase.compile_output(),
            stdout: testcase.stdout(),
            stderr: testcase.stderr(),
        }
    }
}

impl From<TestCaseModel> for TestCase {
    fn from(testcase: TestCaseModel) -> Self {
        TestCase::new(
            Uuid::parse_str(&testcase.token).unwrap(),
            Uuid::parse_str(&testcase.submission_id).unwrap(),
            testcase.testcase_id,
            testcase.status.into(),
            testcase.time,
            testcase.memory,
            testcase.expected_score,
            testcase.eval_message,
            testcase.compile_output,
            testcase.stdout,
            testcase.stderr,
        )
    }
}
