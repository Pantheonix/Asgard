use crate::domain::network_response::NetworkResponse;
use dotenvy::dotenv;
use jsonwebtoken::errors::{Error, ErrorKind};
use jsonwebtoken::{decode, Algorithm, DecodingKey, Validation};
use rocket::http::Status;
use rocket::request::{FromRequest, Outcome};
use rocket::serde::{Deserialize, Serialize};
use rocket::Request;
use std::env;

#[derive(Debug, Deserialize, Serialize)]
pub struct Claims {
    pub sub: String,
    exp: usize,
}

#[derive(Debug)]
pub struct JwtGuard {
    pub claims: Claims,
}

#[rocket::async_trait]
impl<'r> FromRequest<'r> for JwtGuard {
    type Error = NetworkResponse;

    async fn from_request(req: &'r Request<'_>) -> Outcome<Self, NetworkResponse> {
        fn is_valid(key: &str) -> Result<Claims, Error> {
            Ok(decode_jwt(String::from(key))?)
        }

        match req.headers().get_one("authorization") {
            None => {
                let response = String::from("Error validating JWT token - No token provided");
                Outcome::Failure((Status::BadRequest, NetworkResponse::BadRequest(response)))
            }
            Some(key) => match is_valid(key) {
                Ok(claims) => Outcome::Success(JwtGuard { claims }),
                Err(err) => match &err.kind() {
                    ErrorKind::ExpiredSignature => {
                        let response = String::from("Error validating JWT token - Expired Token");
                        Outcome::Failure((
                            Status::BadRequest,
                            NetworkResponse::BadRequest(response),
                        ))
                    }
                    ErrorKind::InvalidToken => {
                        let response = String::from("Error validating JWT token - Invalid Token");
                        Outcome::Failure((
                            Status::BadRequest,
                            NetworkResponse::BadRequest(response),
                        ))
                    }
                    _ => {
                        let response =
                            String::from(format!("Error validating JWT token - {}", err));
                        Outcome::Failure((
                            Status::BadRequest,
                            NetworkResponse::BadRequest(response),
                        ))
                    }
                },
            },
        }
    }
}

fn decode_jwt(token: String) -> Result<Claims, ErrorKind> {
    dotenv().ok();

    let secret = env::var("JWT_SECRET_KEY").expect("JWT_SECRET_KEY must be set.");
    let token = token.trim_start_matches("Bearer").trim();

    match decode::<Claims>(
        &token,
        &DecodingKey::from_secret(secret.as_bytes()),
        &Validation::new(Algorithm::HS256),
    ) {
        Ok(token) => Ok(token.claims),
        Err(err) => Err(err.kind().to_owned()),
    }
}
