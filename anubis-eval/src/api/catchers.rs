use rocket::serde::json::{json, Value};
use rocket::{catch, error};

#[catch(401)]
pub fn unauthorized_catcher(req: &rocket::Request) -> Value {
    error!("Unauthorized request: {:?}", req);
    json!({
        "error": "Access token has expired or is invalid"
    })
}

#[catch(404)]
pub fn not_found_catcher(req: &rocket::Request) -> Value {
    error!("Not found: {:?}", req);
    json!({
        "error": "Resource not found"
    })
}

#[catch(500)]
pub fn internal_error_catcher(req: &rocket::Request) -> Value {
    error!("Internal error: {:?}", req);
    json!({
        "error": "Internal server error"
    })
}
