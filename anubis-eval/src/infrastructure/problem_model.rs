use crate::domain::problem::Problem;
use crate::domain::test::Test;
use crate::schema::{problems, tests};
use diesel::{AsChangeset, Associations, Identifiable, Insertable, Queryable, Selectable};
use uuid::Uuid;

#[derive(Debug, Clone, PartialEq, Queryable, Insertable, Identifiable, AsChangeset, Selectable)]
#[diesel(table_name = problems)]
#[diesel(check_for_backend(diesel::pg::Pg))]
pub(in crate::infrastructure) struct ProblemModel {
    pub(in crate::infrastructure) id: String,
    pub(in crate::infrastructure) name: String,
    pub(in crate::infrastructure) proposer_id: String,
    pub(in crate::infrastructure) is_published: bool,
    pub(in crate::infrastructure) time: Option<f32>,
    pub(in crate::infrastructure) stack_memory: Option<f32>,
    pub(in crate::infrastructure) total_memory: Option<f32>,
}

impl From<Problem> for ProblemModel {
    fn from(problem: Problem) -> Self {
        Self {
            id: problem.id().to_string(),
            name: problem.name().to_string(),
            proposer_id: problem.proposer_id().to_string(),
            is_published: problem.is_published(),
            time: problem.time(),
            stack_memory: problem.stack_memory(),
            total_memory: problem.total_memory(),
        }
    }
}

#[derive(
    Debug,
    Clone,
    PartialEq,
    Queryable,
    Insertable,
    Identifiable,
    AsChangeset,
    Selectable,
    Associations,
)]
#[diesel(belongs_to(ProblemModel, foreign_key = problem_id))]
#[diesel(table_name = tests)]
#[diesel(check_for_backend(diesel::pg::Pg))]
pub(in crate::infrastructure) struct TestModel {
    pub(in crate::infrastructure) id: i32,
    pub(in crate::infrastructure) problem_id: String,
    pub(in crate::infrastructure) score: i32,
    pub(in crate::infrastructure) input_url: String,
    pub(in crate::infrastructure) output_url: String,
}

impl From<Test> for TestModel {
    fn from(test: Test) -> Self {
        Self {
            id: test.id(),
            problem_id: test.problem_id().to_string(),
            score: test.score(),
            input_url: test.input_url().to_string(),
            output_url: test.output_url().to_string(),
        }
    }
}

impl From<TestModel> for Test {
    fn from(test: TestModel) -> Self {
        Test::new(
            test.id,
            Uuid::parse_str(&test.problem_id).unwrap(),
            test.score,
            test.input_url,
            test.output_url,
        )
    }
}
