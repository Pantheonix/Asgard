use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use crate::infrastructure::problem_model::ProblemModel;
use crate::schema::problems::dsl::problems as all_problems;
use crate::schema::problems::id;
use diesel::{PgConnection, RunQueryDsl};

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
}
