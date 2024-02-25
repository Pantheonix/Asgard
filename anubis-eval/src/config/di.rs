use crate::application::dapr_client::DaprClient;
use diesel::Connection;
use diesel::PgConnection;
use lazy_static::lazy_static;
use serde::Deserialize;
use std::sync::Arc;
use tokio::sync::Mutex;

#[derive(Deserialize, Debug, Clone)]
pub struct Config {
    pub jwt_secret_key: String,
    pub dapr_http_port: u16,
    pub dapr_eval_metadata_endpoint: String,
    pub dapr_judge_endpoint: String,
    pub dapr_get_submission_batch_endpoint: String,
    pub dapr_state_store_get_endpoint: String,
    pub dapr_state_store_post_endpoint: String,
    pub eval_cron_schedule: String,
    pub default_no_submissions_per_page: u16,
    pub eval_batch_size: u16,
    pub allowed_origins: String,
}

pub type Atomic<T> = Arc<Mutex<T>>;

lazy_static! {
    pub static ref CONFIG: Config = {
        dotenvy::dotenv().ok();

        envy::prefixed("CONFIG_")
            .from_env::<Config>()
            .expect("Failed to load configuration")
    };
    pub static ref HTTP_CLIENT: reqwest::Client = reqwest::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .danger_accept_invalid_certs(true)
        .build()
        .expect("Failed to create http client");
    pub static ref DAPR_CLIENT: Atomic<DaprClient> = {
        Atomic::new(Mutex::new(DaprClient {
            http_client: reqwest::Client::new(),
            db_conn: DB_CONN.clone(),
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
