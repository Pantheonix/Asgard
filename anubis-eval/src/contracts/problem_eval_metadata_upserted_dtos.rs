use crate::contracts::dapr_dtos::TestDto;
use crate::domain::problem::Problem;
use rocket::serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct EvalMetadataForProblemDto {
    #[serde(rename = "Id")]
    pub problem_id: Uuid,
    #[serde(rename = "Name")]
    pub name: String,
    #[serde(rename = "ProposerId")]
    pub proposer_id: Uuid,
    #[serde(rename = "IsPublished")]
    pub is_published: bool,
    #[serde(rename = "Time")]
    pub time: f32,
    #[serde(rename = "StackMemory")]
    pub stack_memory: f32,
    #[serde(rename = "TotalMemory")]
    pub total_memory: f32,
    #[serde(rename = "Tests")]
    pub tests: Vec<TestDto>,
}

impl From<Problem> for EvalMetadataForProblemDto {
    fn from(problem: Problem) -> Self {
        Self {
            problem_id: *problem.id(),
            name: problem.name().to_string(),
            proposer_id: *problem.proposer_id(),
            is_published: problem.is_published(),
            time: problem.time().unwrap_or(0.0),
            stack_memory: problem.stack_memory().unwrap_or(0.0),
            total_memory: problem.total_memory().unwrap_or(0.0),
            tests: problem
                .tests()
                .into_iter()
                .map(|test| TestDto {
                    test_id: test.id() as usize,
                    input_url: test.input_url().to_string(),
                    output_url: test.output_url().to_string(),
                    score: test.score() as usize,
                })
                .collect(),
        }
    }
}
