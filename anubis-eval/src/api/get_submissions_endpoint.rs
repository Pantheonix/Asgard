use crate::application::auth::JwtContext;
use crate::application::fsp_dtos::SortDiscriminant;
use crate::domain;
use crate::domain::application_error::ApplicationError;
use crate::domain::submission::{Languages, Submission, SubmissionStatuses, Uuids};
use crate::infrastructure::db::Db;
use chrono::{DateTime, Utc};
use rocket::{get, FromForm, Responder};
use serde::Serialize;

#[derive(Responder)]
#[response(status = 200, content_type = "json")]
pub struct GetSubmissionsResponse {
    dto: GetSubmissionsDto,
}

#[get("/submissions?<fsp_dto..>")]
pub async fn get_submissions(
    fsp_dto: FspSubmissionDto,
    user_ctx: JwtContext,
    db: Db,
) -> Result<GetSubmissionsResponse, ApplicationError> {
    db.run(move |conn| match Submission::find_all(fsp_dto, conn) {
        Ok(submissions) => Ok(GetSubmissionsResponse {
            dto: submissions.into(),
        }),
        Err(e) => Err(e),
    })
    .await
}

// FSP stands for Filter, Sort, Paginate
#[derive(Debug, PartialEq, FromForm)]
pub struct FspSubmissionDto {
    pub user_id: Option<Uuids>,
    pub problem_id: Option<Uuids>,
    pub language: Option<Languages>,
    pub status: Option<SubmissionStatuses>,
    pub lt_score: Option<usize>,
    pub gt_score: Option<usize>,
    pub lt_avg_time: Option<f32>,
    pub gt_avg_time: Option<f32>,
    pub lt_avg_memory: Option<f32>,
    pub gt_avg_memory: Option<f32>,
    pub start_date: Option<domain::submission::DateTime>,
    pub end_date: Option<domain::submission::DateTime>,
    pub sort_by: Option<SortDiscriminant>,
    pub page: Option<i64>,
    pub per_page: Option<i64>,
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionsDto {
    submissions: Vec<GetSubmissionDto>,
    items: usize,
    total_pages: usize,
}

#[rocket::async_trait]
impl<'r> rocket::response::Responder<'r, 'static> for GetSubmissionsDto {
    fn respond_to(self, _: &'r rocket::Request<'_>) -> rocket::response::Result<'static> {
        let json =
            serde_json::to_string(&self).unwrap_or("Failed to serialize response".to_string());

        rocket::Response::build()
            .header(rocket::http::ContentType::JSON)
            .sized_body(json.len(), std::io::Cursor::new(json))
            .ok()
    }
}

impl From<(Vec<Submission>, usize, usize)> for GetSubmissionsDto {
    fn from((submissions, items, total_pages): (Vec<Submission>, usize, usize)) -> Self {
        Self {
            submissions: submissions
                .into_iter()
                .map(|submission| submission.into())
                .collect::<Vec<_>>(),
            items,
            total_pages,
        }
    }
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionDto {
    id: String,
    problem_id: String,
    user_id: String,
    language: String,
    status: String,
    score: usize,
    #[serde(with = "chrono::serde::ts_seconds")]
    created_at: DateTime<Utc>,
    avg_time: f32,
    avg_memory: f32,
}

impl From<Submission> for GetSubmissionDto {
    fn from(submission: Submission) -> Self {
        Self {
            id: submission.id().to_string(),
            problem_id: submission.problem_id().to_string(),
            user_id: submission.user_id().to_string(),
            language: submission.language().to_string(),
            status: submission.status().to_string(),
            score: submission.score() as usize,
            created_at: DateTime::<Utc>::from(submission.created_at()),
            avg_time: submission.avg_time().unwrap_or(0.0),
            avg_memory: submission.avg_memory().unwrap_or(0.0),
        }
    }
}
