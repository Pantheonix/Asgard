use crate::contracts::fps_dtos::{
    FpsSubmissionDto, Languages, SortDiscriminant, SubmissionStatuses, Uuids,
};
use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use crate::domain::submission::{Submission, SubmissionStatus, TestCase, TestCaseStatus};
use crate::infrastructure::pagination::Paginate;
use crate::infrastructure::raw_queries;
use crate::infrastructure::submission_model::{SubmissionModel, TestCaseModel};
use crate::schema::problems::dsl::problems as all_problems;
use crate::schema::submissions::dsl::submissions as all_submissions;
use crate::schema::submissions_testcases::dsl::submissions_testcases as all_testcases;
use diesel::sql_types::Text;
use diesel::{sql_query, BoolExpressionMethods, ExpressionMethods, SelectableHelper};
use diesel::{PgConnection, QueryDsl, RunQueryDsl};
use rocket::error;
use std::time::SystemTime;
use uuid::Uuid;

type SubmissionsPaginated = (Vec<(Submission, Problem)>, usize, usize);

impl Submission {
    pub fn insert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        // check if submission fails to insert
        let submission: SubmissionModel = self.clone().into();

        diesel::insert_into(all_submissions)
            .values(submission)
            .execute(conn)
            .map_err(|source| ApplicationError::SubmissionSaveError {
                submission_id: self.id().to_string(),
                source,
            })?;

        // check if any of the test cases fail to insert
        self.test_cases()
            .iter()
            .map(|testcase| testcase.insert(conn))
            .collect::<Result<Vec<_>, _>>()?;

        Ok(())
    }
    
    pub fn upsert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        // check if submission fails to insert
        let submission: SubmissionModel = self.clone().into();

        diesel::insert_into(all_submissions)
            .values(&submission)
            .on_conflict(crate::schema::submissions::dsl::id)
            .do_update()
            .set(&submission)
            .execute(conn)
            .map_err(|source| ApplicationError::SubmissionSaveError {
                submission_id: self.id().to_string(),
                source,
            })?;

        // check if any of the test cases fail to insert
        self.test_cases()
            .iter()
            .map(|testcase| testcase.upsert(conn))
            .collect::<Result<Vec<_>, _>>()?;

        Ok(())
    }

    pub fn find_by_id(
        id: &String,
        conn: &mut PgConnection,
    ) -> Result<(Submission, Problem), ApplicationError> {
        all_submissions
            .find(id.to_string())
            .inner_join(all_testcases)
            .select((SubmissionModel::as_select(), TestCaseModel::as_select()))
            .load::<(SubmissionModel, TestCaseModel)>(conn)
            .map_err(|source| ApplicationError::SubmissionFindError { source })
            .map(
                |submission_and_testcases| match submission_and_testcases.is_empty() {
                    true => Err(ApplicationError::SubmissionNotFoundError {
                        submission_id: id.to_string(),
                    }),
                    false => {
                        let submission = submission_and_testcases.first().unwrap().0.clone();
                        let problem = Problem::find_by_id(&submission.problem_id, conn)?;

                        let mut testcases = submission_and_testcases
                            .into_iter()
                            .map(|(_, testcase)| testcase.into())
                            .collect::<Vec<TestCase>>();

                        testcases.sort_by_key(|a| a.testcase_id());

                        Ok((
                            Submission::new(
                                Uuid::parse_str(&submission.id).unwrap(),
                                Uuid::parse_str(&submission.user_id).unwrap(),
                                Uuid::parse_str(&submission.problem_id).unwrap(),
                                submission.language.into(),
                                submission.source_code,
                                submission.status.into(),
                                submission.score,
                                submission.created_at,
                                submission.avg_time,
                                submission.avg_memory,
                                testcases,
                            ),
                            problem,
                        ))
                    }
                },
            )?
    }

    pub fn find_by_status(
        status: SubmissionStatus,
        conn: &mut PgConnection,
    ) -> Result<Vec<String>, ApplicationError> {
        all_submissions
            .filter(crate::schema::submissions::dsl::status.eq(status.to_string()))
            .select(crate::schema::submissions::dsl::id)
            .load::<String>(conn)
            .map_err(|source| ApplicationError::SubmissionFindError { source })
    }

    pub fn find_highest_score_submissions_by_user_id(
        current_user_id: &Uuid,
        user_id: &String,
        problem_id: &Option<String>,
        conn: &mut PgConnection,
    ) -> Result<Vec<(Submission, Problem)>, ApplicationError> {
        // filter out submissions which should not be visible for current user,
        // i.e. keep only submissions for which the problem is published or
        // the user is the submitter or the user is the proposer

        // if problem_id is provided, return the submission with the highest score for that problem
        // else return a vector of submissions with the highest score for each problem

        let query_results = match problem_id {
            Some(problem_id) => {
                sql_query(raw_queries::GET_HIGHEST_SCORE_SUBMISSIONS_PER_USER_AND_PROBLEM)
                    .bind::<Text, _>(current_user_id.to_string())
                    .bind::<Text, _>(user_id.to_string())
                    .bind::<Text, _>(problem_id.to_string())
                    .load::<SubmissionModel>(conn)
            }
            None => sql_query(raw_queries::GET_HIGHEST_SCORE_SUBMISSIONS_PER_USER)
                .bind::<Text, _>(current_user_id.to_string())
                .bind::<Text, _>(user_id.to_string())
                .load::<SubmissionModel>(conn),
        };

        query_results
            .map_err(|source| ApplicationError::SubmissionFindError { source })
            .map(|submissions| {
                submissions
                    .into_iter()
                    .map(|submission| {
                        let problem = Problem::find_by_id(&submission.problem_id, conn)?;

                        Ok((submission.into(), problem))
                    })
                    .collect::<Result<Vec<_>, _>>()
            })?
    }

    pub fn find_all(
        fps_dto: FpsSubmissionDto,
        user_id: &Uuid,
        conn: &mut PgConnection,
    ) -> Result<SubmissionsPaginated, ApplicationError> {
        use crate::contracts::fps_dtos;
        use crate::schema::{problems, submissions};

        let mut query = all_submissions
            .select(SubmissionModel::as_select())
            .into_boxed();

        // filter out submissions which should not be visible for current user,
        // i.e. keep only submissions for which the problem is published or
        // the user is the submitter or the user is the proposer
        query = query.filter(
            submissions::dsl::problem_id
                .eq_any(
                    all_problems.select(problems::dsl::id).filter(
                        problems::dsl::is_published
                            .eq(true)
                            .or(problems::dsl::proposer_id.eq(user_id.to_string())),
                    ),
                )
                .or(submissions::dsl::user_id.eq(user_id.to_string())),
        );

        if let Some(Uuids { uuids: user_ids }) = fps_dto.user_id {
            let user_ids = user_ids
                .into_iter()
                .map(|user_id| user_id.to_string())
                .collect::<Vec<_>>();
            query = query.filter(submissions::dsl::user_id.eq_any(user_ids));
        }

        if let Some(Uuids { uuids: problem_ids }) = fps_dto.problem_id {
            let problem_ids = problem_ids
                .into_iter()
                .map(|problem_id| problem_id.to_string())
                .collect::<Vec<_>>();
            query = query.filter(submissions::dsl::problem_id.eq_any(problem_ids));
        }

        if let Some(Languages { languages }) = fps_dto.language {
            let languages = languages
                .into_iter()
                .map(|language| language.to_string())
                .collect::<Vec<_>>();
            query = query.filter(submissions::dsl::language.eq_any(languages));
        }

        if let Some(SubmissionStatuses { statuses }) = fps_dto.status {
            let statuses = statuses
                .into_iter()
                .map(|status| status.to_string())
                .collect::<Vec<_>>();
            query = query.filter(submissions::dsl::status.eq_any(statuses));
        }

        if let Some(lt_score) = fps_dto.lt_score {
            query = query.filter(submissions::dsl::score.lt(lt_score as i32));
        }

        if let Some(gt_score) = fps_dto.gt_score {
            query = query.filter(submissions::dsl::score.gt(gt_score as i32));
        }

        if let Some(lt_avg_time) = fps_dto.lt_avg_time {
            query = query.filter(submissions::dsl::avg_time.lt(lt_avg_time));
        }

        if let Some(gt_avg_time) = fps_dto.gt_avg_time {
            query = query.filter(submissions::dsl::avg_time.gt(gt_avg_time));
        }

        if let Some(lt_avg_memory) = fps_dto.lt_avg_memory {
            query = query.filter(submissions::dsl::avg_memory.lt(lt_avg_memory));
        }

        if let Some(gt_avg_memory) = fps_dto.gt_avg_memory {
            query = query.filter(submissions::dsl::avg_memory.gt(gt_avg_memory));
        }

        if let Some(fps_dtos::DateTime {
            date_time: start_date,
        }) = fps_dto.start_date
        {
            let start_date = SystemTime::from(start_date);
            query = query.filter(submissions::dsl::created_at.gt(start_date));
        }

        if let Some(fps_dtos::DateTime {
            date_time: end_date,
        }) = fps_dto.end_date
        {
            let end_date = SystemTime::from(end_date);
            query = query.filter(submissions::dsl::created_at.lt(end_date));
        }

        if let Some(sort_by) = fps_dto.sort_by {
            query = match sort_by {
                SortDiscriminant::ScoreAsc => query.order(submissions::dsl::score.asc()),
                SortDiscriminant::ScoreDesc => query.order(submissions::dsl::score.desc()),
                SortDiscriminant::CreatedAtAsc => query.order(submissions::dsl::created_at.asc()),
                SortDiscriminant::CreatedAtDesc => query.order(submissions::dsl::created_at.desc()),
                SortDiscriminant::AvgTimeAsc => query.order(submissions::dsl::avg_time.asc()),
                SortDiscriminant::AvgTimeDesc => query.order(submissions::dsl::avg_time.desc()),
                SortDiscriminant::AvgMemoryAsc => query.order(submissions::dsl::avg_memory.asc()),
                SortDiscriminant::AvgMemoryDesc => query.order(submissions::dsl::avg_memory.desc()),
            };
        }

        let mut query = query.paginate(fps_dto.page.unwrap_or(1));

        if let Some(per_page) = fps_dto.per_page {
            query = query.per_page(per_page);
        }

        query
            .load_and_count_pages::<SubmissionModel>(conn)
            .map_err(|source| ApplicationError::SubmissionFindError { source })
            .map(|(submissions, total_pages)| {
                let submissions = submissions
                    .into_iter()
                    .map(|submission| {
                        let problem = Problem::find_by_id(&submission.problem_id, conn)?;

                        Ok((submission.into(), problem))
                    })
                    .collect::<Result<Vec<_>, ApplicationError>>()?;
                let items = submissions.len();
                Ok((submissions, items, total_pages as usize))
            })?
    }

    pub fn update_evaluation_metadata(
        &self,
        conn: &mut PgConnection,
    ) -> Result<(), ApplicationError> {
        use crate::schema::submissions::{avg_memory, avg_time, score, status};

        let submission: SubmissionModel = self.clone().into();

        diesel::update(all_submissions.find(submission.id.to_string()))
            .set((
                status.eq(submission.status),
                score.eq(submission.score),
                avg_time.eq(submission.avg_time),
                avg_memory.eq(submission.avg_memory),
            ))
            .execute(conn)
            .map_err(|source| ApplicationError::SubmissionSaveError {
                submission_id: submission.id.to_string(),
                source,
            })?;

        Ok(())
    }

    pub fn is_problem_solved_by_user(
        user_id: &String,
        problem_id: &String,
        conn: &mut PgConnection,
    ) -> bool {
        use crate::schema::submissions::dsl::{
            problem_id as problem_id_column, status as status_column, user_id as user_id_column,
        };

        all_submissions
            .filter(user_id_column.eq(user_id.to_string()))
            .filter(problem_id_column.eq(problem_id.to_string()))
            .filter(status_column.eq(SubmissionStatus::Accepted.to_string()))
            .select(problem_id_column)
            .distinct()
            .first::<String>(conn)
            .map_or_else(
                |error| {
                    error!("Error checking if problem is solved by user: {:?}", error);
                    false
                },
                |_| true,
            )
    }
}

impl TestCase {
    fn insert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        let testcase: TestCaseModel = self.clone().into();

        diesel::insert_into(all_testcases)
            .values(testcase.clone())
            .execute(conn)
            .map_err(|source| ApplicationError::TestCaseSaveError {
                testcase_id: testcase.testcase_id.to_string(),
                submission_id: testcase.submission_id.clone(),
                source,
            })?;

        Ok(())
    }
    
    fn upsert(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        let testcase: TestCaseModel = self.clone().into();

        diesel::insert_into(all_testcases)
            .values(testcase.clone())
            .on_conflict(crate::schema::submissions_testcases::dsl::token)
            .do_update()
            .set(&testcase)
            .execute(conn)
            .map_err(|source| ApplicationError::TestCaseSaveError {
                testcase_id: testcase.testcase_id.to_string(),
                submission_id: testcase.submission_id.clone(),
                source,
            })?;

        Ok(())
    }

    pub fn find_by_submission_id(
        submission_id: &String,
        conn: &mut PgConnection,
    ) -> Result<Vec<TestCase>, ApplicationError> {
        all_testcases
            .filter(crate::schema::submissions_testcases::dsl::submission_id.eq(submission_id))
            .select(crate::schema::submissions_testcases::all_columns)
            .load::<TestCaseModel>(conn)
            .map_err(|source| ApplicationError::TestCaseFindError { source })
            .map(|testcases| {
                testcases
                    .into_iter()
                    .map(|testcase| testcase.into())
                    .collect::<Vec<_>>()
            })
    }

    pub fn find_by_status_and_submission_id(
        status: TestCaseStatus,
        submission_id: &String,
        conn: &mut PgConnection,
    ) -> Result<Vec<String>, ApplicationError> {
        all_testcases
            .filter(crate::schema::submissions_testcases::dsl::status.eq(status.to_string()))
            .filter(crate::schema::submissions_testcases::dsl::submission_id.eq(submission_id))
            .select(crate::schema::submissions_testcases::dsl::token)
            .load::<String>(conn)
            .map_err(|source| ApplicationError::TestCaseFindError { source })
    }

    fn update(&self, conn: &mut PgConnection) -> Result<(), ApplicationError> {
        use crate::schema::submissions_testcases::{
            compile_output, eval_message, memory, status, stderr, stdout, time,
        };

        let testcase: TestCaseModel = self.clone().into();

        diesel::update(all_testcases.find(testcase.token.to_string()))
            .set((
                eval_message.eq(testcase.eval_message),
                compile_output.eq(testcase.compile_output),
                status.eq(testcase.status),
                time.eq(testcase.time),
                memory.eq(testcase.memory),
                stdout.eq(testcase.stdout),
                stderr.eq(testcase.stderr),
            ))
            .execute(conn)
            .map_err(|source| ApplicationError::TestCaseSaveError {
                testcase_id: testcase.testcase_id.to_string(),
                submission_id: testcase.submission_id.to_string(),
                source,
            })?;

        Ok(())
    }

    pub fn update_testcases(
        testcases: Vec<TestCase>,
        conn: &mut PgConnection,
    ) -> Result<(), ApplicationError> {
        testcases
            .iter()
            .map(|testcase| testcase.update(conn))
            .collect::<Result<Vec<_>, _>>()?;

        Ok(())
    }
}
