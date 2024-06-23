use rocket::local::asynchronous::Client;
use crate::rocket;

pub async fn setup_rocket() -> Result<Client, Box<dyn std::error::Error + 'static>> {
    dotenvy::from_filename(".env.template").ok();

    let client = Client::tracked(rocket().await).await?;

    Ok(client)
}
