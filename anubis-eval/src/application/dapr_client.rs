use crate::config::di::{Atomic, CONFIG};
use crate::contracts::dapr_dtos::{
    CreateSubmissionBatchDto, EvaluatedSubmissionBatchDto, StateStoreSetItemDto, TestCaseTokenDto,
};
use crate::contracts::problem_eval_metadata_upserted_dtos::EvalMetadataForProblemDto;
use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use diesel::PgConnection;
use rocket::request::{FromRequest, Outcome};
use rocket::{debug, error, info, warn};
use serde_json::Value;
use std::ops::DerefMut;
use uuid::Uuid;

#[derive(Clone)]
pub struct DaprClient {
    pub http_client: reqwest::Client,
    pub db_conn: Atomic<PgConnection>,
}

impl DaprClient {
    pub async fn get_eval_metadata_for_problem(
        &self,
        problem_id: &Uuid,
    ) -> Result<EvalMetadataForProblemDto, ApplicationError> {
        let problem_id = problem_id.to_string();

        let url = CONFIG
            .dapr_eval_metadata_endpoint
            .to_owned()
            .replace("{problem_id}", problem_id.as_str());

        info!("Get Eval Metadata for Problem {} from db", problem_id);

        let eval_metadata = {
            let mut db = self.db_conn.lock().await;
            Problem::find_by_id(&problem_id, db.deref_mut())
        };

        match eval_metadata {
            Ok(eval_metadata) => {
                info!("Eval Metadata for Problem {} found in db", problem_id);
                Ok(eval_metadata.into())
            }
            Err(ApplicationError::ProblemFindError { .. })
            | Err(ApplicationError::ProblemNotFoundError { .. }) => {
                warn!(
                    "Eval Metadata for Problem {} not found in db. Fetching from Enki",
                    problem_id
                );
                let response = self
                    .http_client
                    .get(&url)
                    .send()
                    .await
                    .map_err(|e| ApplicationError::EvalMetadataError {
                        problem_id: problem_id.clone(),
                        source: e,
                    })?
                    .json::<EvalMetadataForProblemDto>()
                    .await
                    .map_err(|e| ApplicationError::EvalMetadataError {
                        problem_id: problem_id.clone(),
                        source: e,
                    })?;

                let mut db = self.db_conn.lock().await;
                let problem = Problem::from(response.clone());
                problem.upsert(db.deref_mut())?;

                Ok(response)
            }
            Err(e) => Err(e),
        }
    }

    pub async fn get_input_and_output_for_test(
        &self,
        test_id: usize,
        problem_id: Uuid,
        (input_url, output_url): (String, String),
    ) -> Result<(String, String), ApplicationError> {
        let input_key = format!("{}-{}-input", problem_id, test_id);

        info!("Get Input for Test {} from state store", test_id);

        let state_store_response = self.get_item_from_state_store(input_key.as_str()).await?;
        let input = match state_store_response {
            Some(input) => {
                info!("Input for Test {} found in state store", test_id);

                serde_json::from_value::<String>(input).map_err(|_| {
                    ApplicationError::StateStoreGetError {
                        key: input_key.clone(),
                    }
                })?
            },
            None => {
                let input = self
                    .http_client
                    .get(input_url.clone())
                    .send()
                    .await
                    .map_err(|e| ApplicationError::TestInputOutputError {
                        problem_id: problem_id.to_string(),
                        test_id: test_id.to_string(),
                        source: e,
                    })?
                    .text()
                    .await
                    .map_err(|e| ApplicationError::TestInputOutputError {
                        problem_id: problem_id.to_string(),
                        test_id: test_id.to_string(),
                        source: e,
                    })?;

                let state_store_set_item = StateStoreSetItemDto {
                    key: input_key,
                    value: Value::String(input.clone()),
                };
                self.set_items_in_state_store(vec![state_store_set_item])
                    .await?;

                input
            }
        };

        let output_key = format!("{}-{}-output", problem_id, test_id);

        info!("Get Output for Test {} from state store", test_id);

        let state_store_response = self.get_item_from_state_store(output_key.as_str()).await?;
        let output = match state_store_response {
            Some(output) => {
                info!("Output for Test {} found in state store", test_id);

                serde_json::from_value::<String>(output).map_err(|_| {
                    ApplicationError::StateStoreGetError {
                        key: output_key.clone(),
                    }
                })?
            },
            None => {
                let output = self
                    .http_client
                    .get(output_url.clone())
                    .send()
                    .await
                    .map_err(|e| ApplicationError::TestInputOutputError {
                        problem_id: problem_id.to_string(),
                        test_id: test_id.to_string(),
                        source: e,
                    })?
                    .text()
                    .await
                    .map_err(|e| ApplicationError::TestInputOutputError {
                        problem_id: problem_id.to_string(),
                        test_id: test_id.to_string(),
                        source: e,
                    })?;

                let state_store_set_item = StateStoreSetItemDto {
                    key: output_key,
                    value: Value::String(output.clone()),
                };
                self.set_items_in_state_store(vec![state_store_set_item])
                    .await?;

                output
            }
        };

        Ok((input, output))
    }

    pub async fn create_submission_batch(
        &self,
        submission_batch: &CreateSubmissionBatchDto,
    ) -> Result<Vec<TestCaseTokenDto>, ApplicationError> {
        let url = CONFIG.dapr_judge_endpoint.to_owned();

        let response = self
            .http_client
            .post(&url)
            .json(&submission_batch)
            .send()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?
            .json::<Vec<TestCaseTokenDto>>()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?;

        Ok(response)
    }

    pub async fn get_submission_batch(
        &self,
        submission_tokens: &[TestCaseTokenDto],
    ) -> Result<EvaluatedSubmissionBatchDto, ApplicationError> {
        let url = CONFIG.dapr_get_submission_batch_endpoint.to_owned();
        let tokens = submission_tokens
            .iter()
            .map(|token_dto| token_dto.token.clone())
            .collect::<Vec<String>>()
            .join(",");

        let url = url.replace("{tokens}", tokens.as_str());

        let response = self
            .http_client
            .get(&url)
            .send()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?
            .json::<EvaluatedSubmissionBatchDto>()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?;

        Ok(response)
    }

    async fn get_item_from_state_store(
        &self,
        key: &str,
    ) -> Result<Option<Value>, ApplicationError> {
        let url = CONFIG.dapr_state_store_get_endpoint.to_owned();
        let url = url.replace("{key}", key);

        let response = self.http_client.get(&url).send().await.map_err(|e| {
            error!("Error getting item from state store: {:?}", e);
            
            ApplicationError::StateStoreGetError {
                key: key.to_string(),
            }
        })?;

        let response = match response.status() {
            reqwest::StatusCode::OK => {
                let response = response.json::<Value>().await.map_err(|_| {
                    ApplicationError::StateStoreGetError {
                        key: key.to_string(),
                    }
                })?;
                Some(response)
            }
            _ => None,
        };

        debug!("State store response: {:?}", response);

        Ok(response)
    }

    async fn set_items_in_state_store(
        &self,
        items: Vec<StateStoreSetItemDto>,
    ) -> Result<(), ApplicationError> {
        let url = CONFIG.dapr_state_store_post_endpoint.to_owned();

        debug!("State store request: {:?}", items);

        let response = self
            .http_client
            .post(&url)
            .json(&items)
            .send()
            .await
            .map_err(|_| ApplicationError::StateStoreSetError {
                key: items
                    .iter()
                    .map(|item| item.key.clone())
                    .collect::<Vec<String>>()
                    .join(","),
            })?;

        let status = response.status();
        debug!("State store response status: {:?}", status);

        Ok(())
    }
}

#[rocket::async_trait]
impl<'r> FromRequest<'r> for DaprClient {
    type Error = ApplicationError;

    async fn from_request(req: &'r rocket::Request<'_>) -> Outcome<Self, Self::Error> {
        let http_client = req.rocket().state::<reqwest::Client>();
        let db_conn = req.rocket().state::<Atomic<PgConnection>>();

        match (http_client, db_conn) {
            (Some(http_client), Some(db_conn)) => Outcome::Success(DaprClient {
                http_client: http_client.clone(),
                db_conn: db_conn.clone(),
            }),
            _ => {
                let response = String::from("Error initializing http client and/or db connection");
                Outcome::Failure((
                    rocket::http::Status::InternalServerError,
                    ApplicationError::Unknown(response),
                ))
            }
        }
    }
}
