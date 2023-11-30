use crate::config::di::CONFIG;
use rocket::fairing::Fairing;

pub struct Cors;

#[rocket::async_trait]
impl Fairing for Cors {
    fn info(&self) -> rocket::fairing::Info {
        rocket::fairing::Info {
            name: "Add CORS headers to responses",
            kind: rocket::fairing::Kind::Response,
        }
    }

    async fn on_response<'r>(
        &self,
        _request: &'r rocket::Request<'_>,
        response: &mut rocket::Response<'r>,
    ) {
        let allowed_origins = CONFIG
            .allowed_origins
            .split(';')
            .map(String::from)
            .collect::<Vec<String>>();

        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Origin",
            allowed_origins.join(","),
        ));
        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Methods",
            "POST, GET, OPTIONS",
        ));
        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Headers",
            "*",
        ));
        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Credentials",
            "true",
        ));
    }
}
