use crate::application::auth::JWT;
use crate::domain::network_response::NetworkResponse;
use rocket::get;

#[get("/submission")]
pub fn create_submission(key: JWT) -> NetworkResponse {
    NetworkResponse::Created("Super secure endpoint".to_string())
}
