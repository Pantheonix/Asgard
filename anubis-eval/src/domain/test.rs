use crate::contracts::dapr_dtos::TestDto;
use uuid::Uuid;

#[derive(Debug, Clone)]
pub struct Test {
    id: i32,
    problem_id: Uuid,
    score: i32,
    input_url: String,
    output_url: String,
}

impl Test {
    pub fn new(
        id: i32,
        problem_id: Uuid,
        score: i32,
        input_url: String,
        output_url: String,
    ) -> Self {
        Self {
            id,
            score,
            problem_id,
            input_url,
            output_url,
        }
    }

    pub fn id(&self) -> i32 {
        self.id
    }

    pub fn problem_id(&self) -> &Uuid {
        &self.problem_id
    }

    pub fn score(&self) -> i32 {
        self.score
    }

    pub fn input_url(&self) -> &String {
        &self.input_url
    }

    pub fn output_url(&self) -> &String {
        &self.output_url
    }
}

impl From<(TestDto, String)> for Test {
    fn from(value: (TestDto, String)) -> Self {
        let (value, problem_id) = value;

        Self {
            id: value.test_id as i32,
            problem_id: Uuid::parse_str(&problem_id).unwrap(),
            score: value.score as i32,
            input_url: value.input_url,
            output_url: value.output_url,
        }
    }
}
