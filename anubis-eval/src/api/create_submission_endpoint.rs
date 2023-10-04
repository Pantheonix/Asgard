use crate::application::auth::JwtGuard;
use crate::domain::network_response::NetworkResponse;
use rocket::get;

#[get("/submission")]
pub fn create_submission(user_ctx: JwtGuard) -> NetworkResponse {
    NetworkResponse::Created("Super secure endpoint".to_string())
}
