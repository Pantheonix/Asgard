use rocket::serde::json::{json, Value};
use rocket::{catch, error};

#[catch(401)]
pub fn unauthorized_catcher(req: &rocket::Request) -> Value {
    error!("Unauthorized request: {:?}", req);
    json!({
        "error": "Access token has expired or is invalid"
    })
}
