use crate::domain::submission::Submission;
use rocket::serde::Serialize;
use crate::domain::problem::Problem;

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetHighestScoreSubmissionsDto {
    submissions: Vec<GetHighestScoreSubmissionDto>,
}

#[rocket::async_trait]
impl<'r> rocket::response::Responder<'r, 'static> for GetHighestScoreSubmissionsDto {
    fn respond_to(self, _: &'r rocket::Request<'_>) -> rocket::response::Result<'static> {
        let json =
            serde_json::to_string(&self).unwrap_or("Failed to serialize response".to_string());

        rocket::Response::build()
            .header(rocket::http::ContentType::JSON)
            .sized_body(json.len(), std::io::Cursor::new(json))
            .ok()
    }
}

impl From<Vec<(Submission, Problem)>> for GetHighestScoreSubmissionsDto {
    fn from(submissions: Vec<(Submission, Problem)>) -> Self {
        Self {
            submissions: submissions
                .into_iter()
                .map(|submission| submission.into())
                .collect::<Vec<_>>(),
        }
    }
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetHighestScoreSubmissionDto {
    id: String,
    problem_id: String,
    problem_name: String,
    score: usize,
}

impl From<(Submission, Problem)> for GetHighestScoreSubmissionDto {
    fn from(submission: (Submission, Problem)) -> Self {
        let (submission, problem) = (submission.0, submission.1);

        Self {
            id: submission.id().to_string(),
            problem_id: submission.problem_id().to_string(),
            problem_name: problem.name().to_string(),
            score: submission.score() as usize,
        }
    }
}
