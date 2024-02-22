use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use crate::infrastructure::problem_model::ProblemModel;
use crate::schema::problems::dsl::problems as all_problems;
use crate::schema::problems::id;
use diesel::{PgConnection, QueryDsl, RunQueryDsl};

impl Problem {
    pub fn upsert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        let model: ProblemModel = self.clone().into();

        diesel::insert_into(all_problems)
            .values(&model)
            .on_conflict(id)
            .do_update()
            .set(&model)
            .execute(conn)
            .map_err(|e| ApplicationError::ProblemSaveError {
                problem_id: self.id().to_string(),
                source: e,
            })?;

        Ok(())
    }

    pub fn find_by_id(problem_id: &String, conn: &mut PgConnection) -> Result<Problem, ApplicationError> {
        all_problems
            .find(problem_id)
            .first(conn)
            .map(|model: ProblemModel| model.into())
            .map_err(|e| ApplicationError::ProblemFindError {
                problem_id: problem_id.to_string(),
                source: e,
            })
    }
}
