use crate::config::di::CONFIG;
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
    ) -> Result<serde_json::Value, reqwest::Error> {
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
            .json::<serde_json::Value>()
            .await?;

        Ok(response)
    }
}

#[rocket::async_trait]
impl<'r> FromRequest<'r> for DaprClient {
    type Error = ();

    async fn from_request(
        req: &'r rocket::Request<'_>,
    ) -> rocket::request::Outcome<Self, Self::Error> {
        let reqwest_client = req
            .rocket()
            .state::<reqwest::Client>()
            .expect("Expected reqwest client to be managed by rocket");

        Outcome::Success(DaprClient {
            reqwest_client: reqwest_client.clone(),
        })
    }
}
