use crate::domain::submission::{
    IoType, ProgrammingLanguage, Submission, SubmissionStatus, TestExecutionResult,
};
use reqwest::Url;
use serde::{Deserialize, Serialize};
use std::fmt::Display;

#[derive(Debug, Deserialize, Serialize, Clone, PartialEq, Eq, Hash)]
pub struct QueuedSubmissionDto {
    #[serde(rename = "submissionId")]
    pub submission_id: uuid::Uuid,

    #[serde(rename = "userId")]
    pub user_id: uuid::Uuid,

    #[serde(rename = "problemId")]
    pub problem_id: uuid::Uuid,

    #[serde(rename = "programmingLanguage")]
    pub programming_language: ProgrammingLanguage,

    #[serde(rename = "srcCode")]
    pub source_code: String,

    #[serde(rename = "status")]
    pub status: SubmissionStatus,
}

#[derive(Debug, Deserialize, Serialize, Clone, PartialEq, Eq, Hash)]
pub struct SubmissionEvaluationMetadataDto {
    #[serde(rename = "id")]
    pub problem_id: uuid::Uuid,

    #[serde(rename = "name")]
    pub problem_name: String,

    #[serde(rename = "time")]
    pub time_limit: i32,

    #[serde(rename = "totalMemory")]
    pub total_memory_limit: i32,

    #[serde(rename = "stackMemory")]
    pub stack_memory_limit: i32,

    #[serde(rename = "ioType")]
    pub io_type: IoType,

    #[serde(rename = "tests")]
    pub tests: Vec<TestDto>,
}

#[derive(Debug, Deserialize, Serialize, Clone, PartialEq, Eq, Hash)]
pub struct TestDto {
    #[serde(rename = "id")]
    pub test_id: i32,

    #[serde(rename = "score")]
    pub score: i32,

    #[serde(rename = "inputDownloadUrl")]
    pub input_url: String,

    #[serde(rename = "outputDownloadUrl")]
    pub output_url: String,
}

#[derive(Debug, Deserialize, Serialize, Clone, PartialEq, Eq, Hash)]
pub struct EvaluatedSubmissionDto {
    #[serde(rename = "submissionId")]
    pub submission_id: uuid::Uuid,

    #[serde(rename = "userId")]
    pub user_id: uuid::Uuid,

    #[serde(rename = "problemId")]
    pub problem_id: uuid::Uuid,

    #[serde(rename = "programmingLanguage")]
    pub programming_language: ProgrammingLanguage,

    #[serde(rename = "srcCode")]
    pub source_code: String,

    #[serde(rename = "status")]
    pub status: SubmissionStatus,

    #[serde(rename = "compiletimeErrors")]
    pub compiletime_errors: Option<Vec<String>>,

    #[serde(rename = "testResults")]
    pub test_results: Vec<TestExecutionResultDto>,

    #[serde(rename = "totalGrade")]
    pub total_grade: i32,
}

#[derive(Debug, Deserialize, Serialize, Clone, PartialEq, Eq, Hash)]
pub struct TestExecutionResultDto {
    #[serde(rename = "testId")]
    pub test_id: i32,

    #[serde(rename = "time")]
    pub time: i32,

    #[serde(rename = "memory")]
    pub memory: i32,

    #[serde(rename = "grade")]
    pub grade: i32,

    #[serde(rename = "runtimeErrors")]
    pub runtime_errors: Option<Vec<String>>,
}

impl Display for QueuedSubmissionDto {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{{ submission_id: {}, user_id: {}, problem_id: {}, programming_language: {:?}, source_code: {}, status: {:?} }}",
            self.submission_id, self.user_id, self.problem_id, self.programming_language, self.source_code, self.status
        )
    }
}

impl Display for TestDto {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{{ test_id: {}, score: {}, input_url: {}, output_url: {} }}",
            self.test_id, self.score, self.input_url, self.output_url
        )
    }
}

impl Display for SubmissionEvaluationMetadataDto {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{{ problem_id: {}, problem_name: {}, time_limit: {}, total_memory_limit: {}, stack_memory_limit: {}, io_type: {:?}, tests: {:?} }}",
            self.problem_id, self.problem_name, self.time_limit, self.total_memory_limit, self.stack_memory_limit, self.io_type, self.tests
        )
    }
}

impl Display for TestExecutionResultDto {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{{ test_id: {}, time: {}, memory: {}, grade: {}, runtime_errors: {:?} }}",
            self.test_id, self.time, self.memory, self.grade, self.runtime_errors
        )
    }
}

impl Display for EvaluatedSubmissionDto {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{{ submission_id: {}, user_id: {}, problem_id: {}, programming_language: {:?}, source_code: {}, status: {:?}, compiletime_errors: {:?}, test_results: {:?}, total_grade: {} }}",
            self.submission_id, self.user_id, self.problem_id, self.programming_language, self.source_code, self.status, self.compiletime_errors, self.test_results, self.total_grade
        )
    }
}

pub async fn evaluate(event: cloudevents::Event) -> crate::api::Result<impl warp::Reply> {
    match event.data() {
        Some(cloudevents::Data::Json(payload_json)) => {
            // map the payload to a Submission
            let res: Result<QueuedSubmissionDto, serde_json::Error> =
                serde_json::from_value(payload_json.clone());

            match res {
                Ok(queued_submission_dto) => {
                    // println!("submissionEvaluationDto: {:?}", queued_submission_dto);

                    let submission_eval_metadata_dto = reqwest::get(
                        Url::parse(
                            format!(
                                // use dapr sidecar to get the problem metadata
                                "http://dapr-app-id:enki-problems@localhost:35003/api/app/problem/{}/eval-metadata",
                                queued_submission_dto.problem_id
                            )
                            .as_str(),
                        )
                        .map_err(|e| {
                            println!("Error: {:?}", e);
                            warp::reject()
                        })?,
                    )
                    .await
                    .map_err(|e| {
                        println!("Error: {:?}", e);
                        warp::reject()
                    })?
                    .json::<SubmissionEvaluationMetadataDto>()
                    .await
                    .map_err(|e| {
                        println!("Error: {:?}", e);
                        warp::reject()
                    })?;

                    // println!("Submission eval metadata: {}", submission_eval_metadata_dto);

                    // map dtos against submission domain entity
                    let submission = Submission {
                        submission_id: queued_submission_dto.submission_id,
                        user_id: queued_submission_dto.user_id,
                        problem_id: queued_submission_dto.problem_id,
                        programming_language: queued_submission_dto.programming_language,
                        source_code: queued_submission_dto.source_code,
                        status: queued_submission_dto.status,
                        time_limit: submission_eval_metadata_dto.time_limit,
                        total_memory_limit: submission_eval_metadata_dto.total_memory_limit,
                        stack_memory_limit: submission_eval_metadata_dto.stack_memory_limit,
                        io_type: submission_eval_metadata_dto.io_type,
                        tests: submission_eval_metadata_dto
                            .tests
                            .iter()
                            .map(|test_dto| crate::domain::submission::Test {
                                test_id: test_dto.test_id,
                                score: test_dto.score,
                                input_url: test_dto.input_url.clone(),
                                output_url: test_dto.output_url.clone(),
                            })
                            .collect(),
                        compiletime_errors: None,
                        test_results: vec![TestExecutionResult {
                            test_id: 1,
                            time: 1,
                            memory: 4,
                            grade: 20,
                            runtime_errors: None,
                        }],
                        total_grade: 0,
                    };

                    let evaluated_submission_dto = EvaluatedSubmissionDto {
                        submission_id: submission.submission_id,
                        user_id: submission.user_id,
                        problem_id: submission.problem_id,
                        programming_language: submission.programming_language,
                        source_code: submission.source_code,
                        status: SubmissionStatus::Evaluated,
                        compiletime_errors: submission.compiletime_errors,
                        test_results: submission
                            .test_results
                            .iter()
                            .map(|test_result| TestExecutionResultDto {
                                test_id: test_result.test_id,
                                time: test_result.time,
                                memory: test_result.memory,
                                grade: test_result.grade,
                                runtime_errors: test_result.runtime_errors.clone(),
                            })
                            .collect(),
                        total_grade: submission.total_grade,
                    };

                    // publish to dapr pubsub evaluated-submissions-topic
                    // let pub_evaluated_submission_event = cloudevents::EventBuilderV10::new()
                    //     .id(uuid::Uuid::new_v4().to_string().as_str())
                    //     .ty("evaluated-submission")
                    //     .source("http://localhost:35003/v1.0/publish/redis-pubsub/evaluated-submissions-topic")
                    //     .data("application/json", json!(evaluated_submission_dto))
                    //     .build()
                    //     .unwrap();

                    let client = reqwest::Client::new();
                    let pub_res = client
                        .post(
                            Url::parse(
                                "http://localhost:35003/v1.0/publish/redis-pubsub/evaluated-submissions-topic"
                            )
                            .map_err(|e| {
                                println!("Error: {:?}", e);
                                warp::reject()
                            })?,
                        )
                        .json(&evaluated_submission_dto)
                        // .map_err(|e| {
                        //     println!("Error: {:?}", e);
                        //     warp::reject()
                        // })?
                        .send()
                        .await
                        .map_err(|e| {
                            println!("Error: {:?}", e);
                            warp::reject()
                        })?;

                    match pub_res.status() {
                        reqwest::StatusCode::OK => {
                            println!("Published evaluated submission to dapr pubsub evaluated-submissions-topic");
                            Ok(warp::reply::with_status(
                                warp::reply::json(&evaluated_submission_dto),
                                warp::http::StatusCode::OK,
                            ))
                        }
                        _ => {
                            println!("Error: {:?}", pub_res);
                            Err(warp::reject())
                        }
                    }
                }
                Err(e) => {
                    println!("Error: {:?}", e);
                    Err(warp::reject())
                }
            }
        }
        _ => Err(warp::reject()),
    }
}
