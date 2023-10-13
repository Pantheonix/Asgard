use serde::{Deserialize, Deserializer};
use uuid::Uuid;

#[derive(Debug, serde::Deserialize)]
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

#[derive(Debug, serde::Deserialize)]
pub struct TestDto {
    #[serde(rename = "id")]
    pub test_id: usize,
    #[serde(rename = "inputDownloadUrl")]
    pub input: String,
    #[serde(rename = "outputDownloadUrl")]
    pub output: String,
    pub score: usize,
}

#[derive(Debug, serde::Serialize)]
pub struct CreateSubmissionBatchDto {
    pub submissions: Vec<CreateSubmissionTestCaseDto>,
}

#[derive(Debug, serde::Serialize)]
pub struct CreateSubmissionTestCaseDto {
    #[serde(skip_serializing)]
    pub testcase_id: usize,
    pub source_code: String,
    #[serde(rename = "language_id")]
    pub language: String,
    pub stdin: String,
    pub time: f32,
    pub memory_limit: f32,
    pub stack_limit: f32,
    pub expected_output: String,
}

#[derive(Debug, serde::Deserialize)]
pub struct TestCaseTokenDto {
    pub token: String,
}

#[derive(Debug, serde::Deserialize)]
pub struct EvaluatedSubmissionBatchDto {
    pub submissions: Vec<EvaluatedSubmissionTestCaseDto>,
}

#[derive(Debug, serde::Deserialize)]
pub struct EvaluatedSubmissionTestCaseDto {
    pub token: String,
    pub message: Option<String>,
    pub status: StatusDto,
    #[serde(default, deserialize_with = "string_to_f32")]
    pub time: f32,
    #[serde(default)]
    pub memory: f32,
    pub stdout: Option<String>,
    pub stderr: Option<String>,
}

fn string_to_f32<'de, D>(deserializer: D) -> Result<f32, D::Error>
where
    D: serde::Deserializer<'de>,
{
    let s = String::deserialize(deserializer)?;
    Ok(s.parse::<f32>().unwrap())
}

#[derive(Debug, serde::Deserialize)]
pub struct StatusDto {
    pub id: usize,
    pub description: String,
}
