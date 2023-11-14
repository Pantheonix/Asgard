use crate::domain::problem::Problem;
use crate::schema::problems;
use diesel::{AsChangeset, Identifiable, Insertable, Queryable, Selectable};
use uuid::Uuid;

#[derive(Debug, Clone, PartialEq, Queryable, Insertable, Identifiable, AsChangeset, Selectable)]
#[diesel(table_name = problems)]
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

impl From<ProblemModel> for Problem {
    fn from(problem: ProblemModel) -> Self {
        Problem::new(
            Uuid::parse_str(&problem.id).unwrap(),
            problem.name,
            Uuid::parse_str(&problem.proposer_id).unwrap(),
            problem.is_published,
            problem.time,
            problem.stack_memory,
            problem.total_memory,
        )
    }
}
