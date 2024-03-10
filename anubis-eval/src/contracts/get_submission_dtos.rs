use crate::domain::submission::Submission;
use chrono::{DateTime, Utc};
use rocket::serde::Serialize;
use crate::domain::problem::Problem;

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionWithTestCasesDto {
    #[serde(flatten)]
    submission: GetSubmissionDto,
    test_cases: Vec<GetSubmissionTestCaseDto>,
}

#[rocket::async_trait]
impl<'r> rocket::response::Responder<'r, 'static> for GetSubmissionWithTestCasesDto {
    fn respond_to(self, _: &'r rocket::Request<'_>) -> rocket::response::Result<'static> {
        let json =
            serde_json::to_string(&self).unwrap_or("Failed to serialize response".to_string());

        rocket::Response::build()
            .header(rocket::http::ContentType::JSON)
            .sized_body(json.len(), std::io::Cursor::new(json))
            .ok()
    }
}

impl From<(Submission, Problem)> for GetSubmissionWithTestCasesDto {
    fn from((submission, problem): (Submission, Problem)) -> Self {
        Self {
            submission: GetSubmissionDto {
                id: submission.id().to_string(),
                problem_id: submission.problem_id().to_string(),
                problem_name: problem.name().to_string(),
                user_id: submission.user_id().to_string(),
                language: submission.language().to_string(),
                source_code: match submission.source_code().to_string().is_empty() {
                    true => None,
                    false => Some(submission.source_code().to_string()),
                },
                status: submission.status().to_string(),
                score: submission.score() as usize,
                created_at: DateTime::<Utc>::from(submission.created_at()),
                avg_time: submission.avg_time().unwrap_or(0.0),
                avg_memory: submission.avg_memory().unwrap_or(0.0),
            },
            test_cases: submission
                .test_cases()
                .into_iter()
                .map(|test_case| GetSubmissionTestCaseDto {
                    id: test_case.testcase_id() as usize,
                    status: test_case.status().to_string(),
                    time: test_case.time(),
                    memory: test_case.memory(),
                    expected_score: test_case.expected_score() as usize,
                    eval_message: test_case.eval_message().unwrap_or("".to_string()),
                    compile_output: test_case.compile_output().unwrap_or("".to_string()),
                    stdout: test_case.stdout().unwrap_or("".to_string()),
                    stderr: test_case.stderr().unwrap_or("".to_string()),
                })
                .collect(),
        }
    }
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionDto {
    id: String,
    problem_id: String,
    problem_name: String,
    user_id: String,
    language: String,
    source_code: Option<String>,
    status: String,
    score: usize,
    #[serde(with = "chrono::serde::ts_seconds")]
    created_at: DateTime<Utc>,
    avg_time: f32,
    avg_memory: f32,
}

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
pub struct GetSubmissionTestCaseDto {
    id: usize,
    status: String,
    time: f32,
    memory: f32,
    expected_score: usize,
    eval_message: String,
    compile_output: String,
    stdout: String,
    stderr: String,
}
