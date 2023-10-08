use lazy_static::lazy_static;
use serde::Deserialize;

#[derive(Deserialize, Debug, Clone)]
pub struct Config {
    pub jwt_secret_key: String,
    pub dapr_http_port: u16,
    pub dapr_eval_metadata_endpoint: String,
}

lazy_static! {
    pub static ref CONFIG: Config = {
        dotenvy::dotenv().ok();

        envy::prefixed("CONFIG_")
            .from_env::<Config>()
            .expect("Failed to load configuration")
    };
}
