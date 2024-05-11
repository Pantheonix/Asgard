use crate::config::di::CONFIG;
use rocket::fairing::Fairing;
use rocket::http::{Method, Status};

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
        request: &'r rocket::Request<'_>,
        response: &mut rocket::Response<'r>,
    ) {
        let allowed_origins = CONFIG
            .allowed_origins
            .split(';')
            .map(String::from)
            .collect::<Vec<String>>();

        if request.method() == Method::Options {
            response.set_status(Status::NoContent);
            response.set_header(rocket::http::Header::new(
                "Access-Control-Allow-Methods",
                "POST, GET, OPTIONS",
            ));
            response.set_header(rocket::http::Header::new(
                "Access-Control-Allow-Headers",
                "Access-Control-Allow-Headers, \
                       Origin,Accept, X-Requested-With, \
                       Content-Type, \
                       Access-Control-Request-Method, \
                       Access-Control-Request-Headers",
            ));
            response.remove_header("Content-Type");
        }
        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Origin",
            allowed_origins.join(","),
        ));
        response.set_header(rocket::http::Header::new(
            "Access-Control-Allow-Credentials",
            "true",
        ));
    }
}
