use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use crate::domain::test::Test;
use crate::infrastructure::problem_model::{ProblemModel, TestModel};
use crate::schema::problems;
use crate::schema::problems::dsl::problems as all_problems;
use crate::schema::tests;
use crate::schema::tests::dsl::tests as all_tests;
use diesel::{PgConnection, QueryDsl, RunQueryDsl, SelectableHelper};
use uuid::Uuid;

impl Problem {
    pub fn upsert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        let model: ProblemModel = self.clone().into();

        diesel::insert_into(all_problems)
            .values(&model)
            .on_conflict(problems::id)
            .do_update()
            .set(&model)
            .execute(conn)
            .map_err(|e| ApplicationError::ProblemSaveError {
                problem_id: self.id().to_string(),
                source: e,
            })?;

        self.tests()
            .iter()
            .map(|test| test.upsert(conn))
            .collect::<Result<Vec<()>, ApplicationError>>()?;

        Ok(())
    }

    pub fn find_by_id(
        problem_id: &String,
        conn: &mut PgConnection,
    ) -> Result<Problem, ApplicationError> {
        all_problems
            .find(problem_id)
            .inner_join(all_tests)
            .select((ProblemModel::as_select(), TestModel::as_select()))
            .load::<(ProblemModel, TestModel)>(conn)
            .map_err(|source| ApplicationError::ProblemFindError { source })
            .map(|problem_and_tests| match problem_and_tests.is_empty() {
                true => Err(ApplicationError::ProblemNotFoundError {
                    problem_id: problem_id.to_string(),
                }),
                false => {
                    let problem = problem_and_tests.first().unwrap().0.clone();
                    let tests = problem_and_tests
                        .into_iter()
                        .map(|(_, test)| test.into())
                        .collect::<Vec<_>>();

                    Ok(Problem::new(
                        Uuid::parse_str(&problem.id).unwrap(),
                        problem.name,
                        Uuid::parse_str(&problem.proposer_id).unwrap(),
                        problem.is_published,
                        problem.time,
                        problem.stack_memory,
                        problem.total_memory,
                        tests,
                    ))
                }
            })?
    }
}

impl Test {
    pub fn upsert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        let model: TestModel = self.clone().into();

        diesel::insert_into(all_tests)
            .values(&model)
            .on_conflict(tests::id)
            .do_update()
            .set(&model)
            .execute(conn)
            .map_err(|e| ApplicationError::TestSaveError {
                problem_id: self.problem_id().to_string(),
                test_id: self.id().to_string(),
                source: e,
            })?;

        Ok(())
    }
}
