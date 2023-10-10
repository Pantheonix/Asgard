use uuid::Uuid;

#[derive(Debug, serde::Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct GetEvalMetadataForProblemDto {
    #[serde(rename = "id")]
    pub problem_id: Uuid,
    pub name: String,
    pub proposer_id: Uuid,
    pub is_published: bool,
    pub time: usize,
    pub stack_memory: usize,
    pub total_memory: usize,
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
    pub source_code: String,
    #[serde(rename = "language_id")]
    pub language: String,
    pub stdin: String,
    pub time: usize,
    pub memory_limit: usize,
    pub stack_limit: usize,
    pub expected_output: String,
}

#[derive(Debug, serde::Deserialize)]
pub struct TestCaseTokenDto {
    pub token: String,
}