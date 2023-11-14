use rocket::{catch, error};

#[catch(401)]
pub fn unauthorized_catcher(req: &rocket::Request) -> &'static str {
    error!("Unauthorized request: {:?}", req);
    "Unauthorized user access due to missing or invalid JWT token"
}
