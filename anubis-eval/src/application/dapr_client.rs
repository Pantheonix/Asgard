use crate::config::di::{CONFIG, DB_CONN};
use crate::contracts::dapr_dtos::{
    CacheMetadata, CacheSetItemDto, CreateSubmissionBatchDto, EvaluatedSubmissionBatchDto,
    GetEvalMetadataForProblemDto, TestCaseTokenDto,
};
use crate::domain::application_error::ApplicationError;
use crate::domain::problem::Problem;
use rocket::request::{FromRequest, Outcome};
use rocket::{debug, info};
use serde_json::Value;
use std::ops::DerefMut;
use uuid::Uuid;

#[derive(Debug, Clone)]
pub struct DaprClient {
    pub reqwest_client: reqwest::Client,
}

impl DaprClient {
    pub async fn get_eval_metadata_for_problem(
        &self,
        problem_id: &Uuid,
    ) -> Result<GetEvalMetadataForProblemDto, ApplicationError> {
        let problem_id = problem_id.to_string();

        let url = CONFIG
            .dapr_eval_metadata_endpoint
            .to_owned()
            .replace("{problem_id}", problem_id.as_str());

        info!("Get Eval Metadata for Problem {} from cache", problem_id);

        let cache_response = self.get_item_from_cache(problem_id.as_str()).await?;

        match cache_response {
            Some(cache_response) => {
                info!(
                    "Eval Metadata for Problem {} retrieved from cache",
                    problem_id
                );
                let response =
                    serde_json::from_value::<GetEvalMetadataForProblemDto>(cache_response)
                        .map_err(|_| ApplicationError::CacheGetError {
                            key: problem_id.clone(),
                        })?;
                Ok(response)
            }
            None => {
                info!(
                    "Eval Metadata for Problem {} not found in cache",
                    problem_id
                );
                let response = self
                    .reqwest_client
                    .get(&url)
                    .send()
                    .await
                    .map_err(|e| ApplicationError::EvalMetadataError {
                        problem_id: problem_id.clone(),
                        source: e,
                    })?
                    .json::<GetEvalMetadataForProblemDto>()
                    .await
                    .map_err(|e| ApplicationError::EvalMetadataError {
                        problem_id: problem_id.clone(),
                        source: e,
                    })?;

                let cache_set_item = CacheSetItemDto {
                    key: problem_id.clone(),
                    value: serde_json::to_value(&response).map_err(|_| {
                        ApplicationError::CacheSetError {
                            key: problem_id.clone(),
                        }
                    })?,
                    metadata: Some(CacheMetadata {
                        ttl_in_seconds: CONFIG.default_cache_ttl_seconds.to_string(),
                    }),
                };

                info!("Set Eval Metadata for Problem {} in cache", problem_id);
                self.set_items_in_cache(vec![cache_set_item]).await?;

                let problem: Problem = response.clone().into();
                let db = DB_CONN.clone();
                let mut db = db.lock().await;

                info!(
                    "Upserting Eval Metadata for Problem {} in database",
                    problem_id
                );
                problem.upsert(db.deref_mut())?;

                Ok(response)
            }
        }
    }

    pub async fn get_input_and_output_for_test(
        &self,
        test_id: usize,
        problem_id: Uuid,
        (input_url, output_url): (String, String),
    ) -> Result<(String, String), ApplicationError> {
        let input_key = format!("{}-{}-input", problem_id, test_id);

        info!("Get Input for Test {} from cache", test_id);

        let cache_response = self.get_item_from_cache(input_key.as_str()).await?;
        let input = match cache_response {
            Some(input) => serde_json::from_value::<String>(input).map_err(|_| {
                ApplicationError::CacheGetError {
                    key: input_key.clone(),
                }
            })?,
            None => {
                let input = self
                    .reqwest_client
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

                let cache_set_item = CacheSetItemDto {
                    key: input_key,
                    value: Value::String(input.clone()),
                    metadata: Some(CacheMetadata {
                        ttl_in_seconds: CONFIG.default_cache_ttl_seconds.to_string(),
                    }),
                };
                self.set_items_in_cache(vec![cache_set_item]).await?;

                input
            }
        };

        let output_key = format!("{}-{}-output", problem_id, test_id);

        info!("Get Output for Test {} from cache", test_id);

        let cache_response = self.get_item_from_cache(output_key.as_str()).await?;
        let output = match cache_response {
            Some(output) => serde_json::from_value::<String>(output).map_err(|_| {
                ApplicationError::CacheGetError {
                    key: output_key.clone(),
                }
            })?,
            None => {
                let output = self
                    .reqwest_client
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

                let cache_set_item = CacheSetItemDto {
                    key: output_key,
                    value: Value::String(output.clone()),
                    metadata: Some(CacheMetadata {
                        ttl_in_seconds: CONFIG.default_cache_ttl_seconds.to_string(),
                    }),
                };
                self.set_items_in_cache(vec![cache_set_item]).await?;

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
            .reqwest_client
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
            .reqwest_client
            .get(&url)
            .send()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?
            .json::<EvaluatedSubmissionBatchDto>()
            .await
            .map_err(|e| ApplicationError::SubmissionEvaluationError { source: e })?;

        Ok(response)
    }

    async fn get_item_from_cache(&self, key: &str) -> Result<Option<Value>, ApplicationError> {
        let url = CONFIG.dapr_state_store_get_endpoint.to_owned();
        let url = url.replace("{key}", key);

        let response = self.reqwest_client.get(&url).send().await.map_err(|_| {
            ApplicationError::CacheGetError {
                key: key.to_string(),
            }
        })?;

        let response = match response.status() {
            reqwest::StatusCode::OK => {
                let response = response.json::<Value>().await.map_err(|_| {
                    ApplicationError::CacheGetError {
                        key: key.to_string(),
                    }
                })?;
                Some(response)
            }
            _ => None,
        };

        debug!("Cache response: {:?}", response);

        Ok(response)
    }

    async fn set_items_in_cache(
        &self,
        items: Vec<CacheSetItemDto>,
    ) -> Result<(), ApplicationError> {
        let url = CONFIG.dapr_state_store_post_endpoint.to_owned();

        debug!("Cache request: {:?}", items);

        let response = self
            .reqwest_client
            .post(&url)
            .json(&items)
            .send()
            .await
            .map_err(|_| ApplicationError::CacheSetError {
                key: items
                    .iter()
                    .map(|item| item.key.clone())
                    .collect::<Vec<String>>()
                    .join(","),
            })?;

        let status = response.status();
        debug!("Cache response status: {:?}", status);

        Ok(())
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
