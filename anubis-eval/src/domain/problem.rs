use crate::contracts::problem_eval_metadata_upserted_dtos::EvalMetadataForProblemDto;
use crate::domain::test::Test;
use uuid::Uuid;

#[derive(Debug, Clone)]
pub struct Problem {
    id: Uuid,
    name: String,
    proposer_id: Uuid,
    is_published: bool,
    time: Option<f32>,
    stack_memory: Option<f32>,
    total_memory: Option<f32>,
    tests: Vec<Test>,
}

impl Problem {
    pub fn new(
        id: Uuid,
        name: String,
        proposer_id: Uuid,
        is_published: bool,
        time: Option<f32>,
        stack_memory: Option<f32>,
        total_memory: Option<f32>,
        tests: Vec<Test>,
    ) -> Self {
        Self {
            id,
            name,
            proposer_id,
            is_published,
            time,
            stack_memory,
            total_memory,
            tests,
        }
    }

    pub fn id(&self) -> &Uuid {
        &self.id
    }

    pub fn name(&self) -> &String {
        &self.name
    }

    pub fn proposer_id(&self) -> &Uuid {
        &self.proposer_id
    }

    pub fn is_published(&self) -> bool {
        self.is_published
    }

    pub fn time(&self) -> Option<f32> {
        self.time
    }

    pub fn stack_memory(&self) -> Option<f32> {
        self.stack_memory
    }

    pub fn total_memory(&self) -> Option<f32> {
        self.total_memory
    }

    pub fn tests(&self) -> &Vec<Test> {
        &self.tests
    }
}

impl From<EvalMetadataForProblemDto> for Problem {
    fn from(dto: EvalMetadataForProblemDto) -> Self {
        Self {
            id: dto.problem_id,
            name: dto.name,
            proposer_id: dto.proposer_id,
            is_published: dto.is_published,
            time: Option::from(dto.time),
            stack_memory: Option::from(dto.stack_memory),
            total_memory: Option::from(dto.total_memory),
            tests: dto
                .tests
                .into_iter()
                .map(|test| (test, dto.problem_id.to_string()).into())
                .collect(),
        }
    }
}
