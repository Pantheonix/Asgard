use crate::domain::problem::Problem;
use crate::domain::submission::Submission;
use chrono::{DateTime, Utc};
use rocket::serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionsDto {
    pub submissions: Vec<GetSubmissionDto>,
    pub items: usize,
    pub total_pages: usize,
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

impl From<(Vec<(Submission, Problem)>, usize, usize)> for GetSubmissionsDto {
    fn from((submissions, items, total_pages): (Vec<(Submission, Problem)>, usize, usize)) -> Self {
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

impl From<Vec<(Submission, Problem)>> for GetSubmissionsDto {
    fn from(submissions: Vec<(Submission, Problem)>) -> Self {
        Self {
            submissions: submissions
                .clone()
                .into_iter()
                .map(|submission| submission.into())
                .collect(),
            items: submissions.len(),
            total_pages: 1,
        }
    }
}

#[derive(Serialize, Deserialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionDto {
    id: String,
    problem_id: String,
    problem_name: String,
    is_published: bool,
    user_id: String,
    language: String,
    status: String,
    score: usize,
    #[serde(with = "chrono::serde::ts_seconds")]
    created_at: DateTime<Utc>,
    avg_time: f32,
    avg_memory: f32,
}

impl From<(Submission, Problem)> for GetSubmissionDto {
    fn from((submission, problem): (Submission, Problem)) -> Self {
        Self {
            id: submission.id().to_string(),
            problem_id: submission.problem_id().to_string(),
            problem_name: problem.name().to_string(),
            is_published: problem.is_published(),
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
