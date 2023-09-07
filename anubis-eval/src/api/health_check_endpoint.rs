use rocket::get;

#[get("/health_check")]
pub fn health_check() -> &'static str {
    "Healthy"
}