use crate::application::dapr_client::DaprClient;
use diesel::Connection;
use diesel::PgConnection;
use futures::lock::Mutex;
use lazy_static::lazy_static;
use serde::Deserialize;
use std::sync::Arc;

#[derive(Deserialize, Debug, Clone)]
pub struct Config {
    pub jwt_secret_key: String,
    pub dapr_http_port: u16,
    pub dapr_eval_metadata_endpoint: String,
    pub dapr_judge_endpoint: String,
    pub dapr_get_submission_batch_endpoint: String,
    pub eval_cron_schedule: String,
    pub default_no_submissions_per_page: u16,
}

pub type Atomic<T> = Arc<Mutex<T>>;

lazy_static! {
    pub static ref CONFIG: Config = {
        dotenvy::dotenv().ok();

        envy::prefixed("CONFIG_")
            .from_env::<Config>()
            .expect("Failed to load configuration")
    };
    pub static ref REQWEST_CLIENT: reqwest::Client = reqwest::Client::new();
    pub static ref DAPR_CLIENT: Atomic<DaprClient> = {
        Atomic::new(Mutex::new(DaprClient {
            reqwest_client: reqwest::Client::new(),
        }))
    };
    pub static ref DB_CONN: Atomic<PgConnection> = {
        dotenvy::dotenv().ok();

        let database_url = std::env::var("DATABASE_URL").expect("DATABASE_URL must be set");
        Arc::new(Mutex::new(
            PgConnection::establish(&database_url).expect("Failed to create pool"),
        ))
    };
}
