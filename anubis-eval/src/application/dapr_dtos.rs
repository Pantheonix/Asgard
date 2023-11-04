use crate::domain::submission::{TestCase, TestCaseStatus};
use serde::{Deserialize, Serialize};
use std::str::FromStr;
use uuid::Uuid;

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CacheSetItemDto {
    pub key: String,
    pub value: serde_json::Value,
    pub metadata: Option<CacheMetadata>,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct CacheMetadata {
    pub ttl_in_seconds: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct GetEvalMetadataForProblemDto {
    #[serde(rename = "id")]
    pub problem_id: Uuid,
    pub name: String,
    pub proposer_id: Uuid,
    pub is_published: bool,
    pub time: f32,
    pub stack_memory: f32,
    pub total_memory: f32,
    pub tests: Vec<TestDto>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct TestDto {
    #[serde(rename = "id")]
    pub test_id: usize,
    #[serde(rename = "inputDownloadUrl")]
    pub input: String,
    #[serde(rename = "outputDownloadUrl")]
    pub output: String,
    pub score: usize,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CreateSubmissionBatchDto {
    pub submissions: Vec<CreateSubmissionTestCaseDto>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CreateSubmissionTestCaseDto {
    #[serde(skip_serializing)]
    pub testcase_id: usize,
    pub source_code: String,
    #[serde(rename = "language_id")]
    pub language: usize,
    pub stdin: String,
    pub time: f32,
    pub memory_limit: f32,
    pub stack_limit: f32,
    pub expected_output: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct TestCaseTokenDto {
    pub token: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct EvaluatedSubmissionBatchDto {
    pub submissions: Vec<EvaluatedSubmissionTestCaseDto>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct EvaluatedSubmissionTestCaseDto {
    pub token: String,
    #[serde(default)]
    pub message: Option<String>,
    #[serde(default)]
    pub compile_output: Option<String>,
    pub status: StatusDto,
    #[serde(default)]
    pub time: Option<String>,
    #[serde(default)]
    pub memory: Option<f32>,
    #[serde(default)]
    pub stdout: Option<String>,
    #[serde(default)]
    pub stderr: Option<String>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StatusDto {
    pub id: usize,
    pub description: String,
}

impl From<EvaluatedSubmissionTestCaseDto> for TestCase {
    fn from(value: EvaluatedSubmissionTestCaseDto) -> Self {
        let status: TestCaseStatus = value.status.into();
        let compile_output = match status {
            TestCaseStatus::CompilationError => value.compile_output,
            _ => None,
        };
        let time = match value.time {
            Some(time) => time.parse::<f32>().unwrap(),
            None => 0.0,
        };

        TestCase::new_for_update(
            Uuid::from_str(&value.token).unwrap(),
            value.message,
            compile_output,
            status,
            time,
            value.memory.unwrap_or(0.0),
            value.stdout,
            value.stderr,
        )
    }
}

impl From<StatusDto> for TestCaseStatus {
    fn from(value: StatusDto) -> Self {
        value.id.into()
    }
}
