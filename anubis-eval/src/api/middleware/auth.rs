use crate::config::di::CONFIG;
use crate::domain::application_error::ApplicationError;
use jsonwebtoken::errors::{Error, ErrorKind};
use jsonwebtoken::{decode, Algorithm, DecodingKey, Validation};
use rocket::http::Status;
use rocket::request::{FromRequest, Outcome};
use rocket::serde::{Deserialize, Serialize};
use rocket::Request;

#[derive(Debug)]
pub struct JwtContext {
    pub claims: Claims,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Claims {
    pub sub: String,
    exp: usize,
}

#[rocket::async_trait]
impl<'r> FromRequest<'r> for JwtContext {
    type Error = ApplicationError;

    async fn from_request(req: &'r Request<'_>) -> Outcome<Self, Self::Error> {
        fn is_valid(key: &str) -> Result<Claims, Error> {
            Ok(decode_jwt(String::from(key))?)
        }

        match req.headers().get_one("authorization") {
            None => {
                let response = String::from("Error validating JWT token - No token provided");
                Outcome::Error((Status::Unauthorized, ApplicationError::AuthError(response)))
            }
            Some(key) => match is_valid(key) {
                Ok(claims) => Outcome::Success(JwtContext { claims }),
                Err(err) => match &err.kind() {
                    ErrorKind::ExpiredSignature => {
                        let response = String::from("Error validating JWT token - Expired Token");
                        Outcome::Error((
                            Status::Unauthorized,
                            ApplicationError::AuthError(response),
                        ))
                    }
                    ErrorKind::InvalidToken => {
                        let response = String::from("Error validating JWT token - Invalid Token");
                        Outcome::Error((
                            Status::Unauthorized,
                            ApplicationError::AuthError(response),
                        ))
                    }
                    _ => {
                        let response = format!("Error validating JWT token - {}", err);
                        Outcome::Error((
                            Status::Unauthorized,
                            ApplicationError::AuthError(response),
                        ))
                    }
                },
            },
        }
    }
}

fn decode_jwt(token: String) -> Result<Claims, ErrorKind> {
    let secret = CONFIG.clone().jwt_secret_key;
    let token = token.trim_start_matches("Bearer").trim();

    match decode::<Claims>(
        token,
        &DecodingKey::from_secret(secret.as_bytes()),
        &Validation::new(Algorithm::HS256),
    ) {
        Ok(token) => Ok(token.claims),
        Err(err) => Err(err.kind().to_owned()),
    }
}

#[cfg(test)]
pub mod tests {
    use crate::config::di::CONFIG;
    use crate::tests::user::tests::User;
    use rocket::serde::{Deserialize, Serialize};

    #[derive(Debug, Deserialize, Serialize)]
    pub struct Claims {
        sub: String,
        email: String,
        role: Vec<String>,
        exp: usize,
    }

    pub fn encode_jwt(user: User) -> Result<String, Box<dyn std::error::Error>> {
        let secret = CONFIG.clone().jwt_secret_key;
        let claims = Claims {
            sub: user.id.to_string(),
            email: user.email,
            role: user.role.iter().map(|r| r.to_string()).collect(),
            exp: (chrono::Utc::now() + chrono::Duration::hours(1)).timestamp() as usize,
        };

        jsonwebtoken::encode(
            &jsonwebtoken::Header::default(),
            &claims,
            &jsonwebtoken::EncodingKey::from_secret(secret.as_ref()),
        )
        .map_err(|e| e.into())
    }
}
