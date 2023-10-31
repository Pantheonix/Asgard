use crate::api::get_submissions_endpoint::FspSubmissionDto;
use crate::application::fsp_dtos::SortDiscriminant;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Submission, SubmissionStatus, TestCase, TestCaseStatus};
use crate::infrastructure::pagination::Paginate;
use crate::infrastructure::submission_model::{SubmissionModel, TestCaseModel};
use crate::schema::submissions::dsl::submissions as all_submissions;
use crate::schema::submissions_testcases::dsl::submissions_testcases as all_testcases;
use diesel::{ExpressionMethods, SelectableHelper};
use diesel::{PgConnection, QueryDsl, RunQueryDsl};
use uuid::Uuid;

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

    pub fn find_by_id(
        id: &String,
        conn: &mut PgConnection,
    ) -> Result<Submission, ApplicationError> {
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
                        let submission = submission_and_testcases[0].0.clone();
                        let testcases = submission_and_testcases
                            .into_iter()
                            .map(|(_, testcase)| testcase.into())
                            .collect::<Vec<_>>();

                        Ok(Submission::new(
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

    pub fn find_all(
        fsp_dto: FspSubmissionDto,
        conn: &mut PgConnection,
    ) -> Result<(Vec<Submission>, usize), ApplicationError> {
        let mut query = all_submissions
            .select(SubmissionModel::as_select())
            .into_boxed();

        if let Some(sort_by) = fsp_dto.sort_by {
            query = match sort_by {
                SortDiscriminant::ScoreAsc => {
                    query.order(crate::schema::submissions::dsl::score.asc())
                }
                SortDiscriminant::ScoreDesc => {
                    query.order(crate::schema::submissions::dsl::score.desc())
                }
                SortDiscriminant::CreatedAtAsc => {
                    query.order(crate::schema::submissions::dsl::created_at.asc())
                }
                SortDiscriminant::CreatedAtDesc => {
                    query.order(crate::schema::submissions::dsl::created_at.desc())
                }
                SortDiscriminant::AvgTimeAsc => {
                    query.order(crate::schema::submissions::dsl::avg_time.asc())
                }
                SortDiscriminant::AvgTimeDesc => {
                    query.order(crate::schema::submissions::dsl::avg_time.desc())
                }
                SortDiscriminant::AvgMemoryAsc => {
                    query.order(crate::schema::submissions::dsl::avg_memory.asc())
                }
                SortDiscriminant::AvgMemoryDesc => {
                    query.order(crate::schema::submissions::dsl::avg_memory.desc())
                }
            };
        }

        let mut query = query.paginate(fsp_dto.page.unwrap_or(1));

        if let Some(per_page) = fsp_dto.per_page {
            query = query.per_page(per_page);
        }

        query
            .load_and_count_pages::<SubmissionModel>(conn)
            .map_err(|source| ApplicationError::SubmissionFindError { source })
            .map(|(submissions, total_pages)| {
                (
                    submissions
                        .into_iter()
                        .map(|submission| submission.into())
                        .collect::<Vec<_>>(),
                    total_pages as usize,
                )
            })
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
