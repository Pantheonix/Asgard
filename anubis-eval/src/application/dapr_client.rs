use crate::application::dapr_dtos::{
    CreateSubmissionBatchDto, EvaluatedSubmissionBatchDto, GetEvalMetadataForProblemDto,
    TestCaseTokenDto,
};
use crate::config::di::CONFIG;
use crate::domain::application_error::ApplicationError;
use rocket::request::{FromRequest, Outcome};
use uuid::Uuid;

#[derive(Debug)]
pub struct DaprClient {
    reqwest_client: reqwest::Client,
}

impl DaprClient {
    pub async fn get_eval_metadata_for_problem(
        &self,
        problem_id: &Uuid,
    ) -> Result<GetEvalMetadataForProblemDto, reqwest::Error> {
        let problem_id = problem_id.to_string();

        let url = CONFIG
            .dapr_eval_metadata_endpoint
            .to_owned()
            .replace("{problem_id}", problem_id.as_str());

        let response = self
            .reqwest_client
            .get(&url)
            .send()
            .await?
            .json::<GetEvalMetadataForProblemDto>()
            .await?;

        Ok(response)
    }

    pub async fn get_input_and_output_for_test(
        (input_url, output_url): (String, String),
    ) -> Result<(String, String), reqwest::Error> {
        let input = reqwest::get(input_url).await?.text().await?;
        let output = reqwest::get(output_url).await?.text().await?;

        Ok((input, output))
    }

    pub async fn create_submission_batch(
        &self,
        submission_batch: &CreateSubmissionBatchDto,
    ) -> Result<Vec<TestCaseTokenDto>, reqwest::Error> {
        let url = CONFIG.dapr_judge_endpoint.to_owned();

        let response = self
            .reqwest_client
            .post(&url)
            .json(&submission_batch)
            .send()
            .await?
            .json::<Vec<TestCaseTokenDto>>()
            .await?;

        Ok(response)
    }

    pub async fn get_submission_batch(
        &self,
        submission_tokens: &Vec<TestCaseTokenDto>,
    ) -> Result<EvaluatedSubmissionBatchDto, reqwest::Error> {
        let url = CONFIG.dapr_get_submission_batch_endpoint.to_owned();
        let tokens = submission_tokens
            .iter()
            .map(|token_dto| token_dto.token.clone())
            .collect::<Vec<String>>()
            .join(",");

        let url = url.replace("{tokens}", tokens.as_str());

        let response = self
            .reqwest_client
            .get(&url)
            .send()
            .await?
            .json::<EvaluatedSubmissionBatchDto>()
            .await?;

        Ok(response)
    }
}

#[rocket::async_trait]
impl<'r> FromRequest<'r> for DaprClient {
    type Error = ApplicationError;

    async fn from_request(req: &'r rocket::Request<'_>) -> Outcome<Self, Self::Error> {
        let reqwest_client = req.rocket().state::<reqwest::Client>();

        match reqwest_client {
            None => {
                let response = String::from("Error getting reqwest client");
                Outcome::Failure((
                    rocket::http::Status::InternalServerError,
                    ApplicationError::Unknown(response),
                ))
            }
            Some(reqwest_client) => Outcome::Success(DaprClient {
                reqwest_client: reqwest_client.clone(),
            }),
        }
    }
}
