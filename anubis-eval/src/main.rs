use crate::config::logger::init_logger;
use rocket::log::private::info;
use rocket::{catchers, launch, routes};

mod api;
mod application;
mod config;
mod domain;
mod infrastructure;
mod schema;

#[launch]
fn rocket() -> _ {
    init_logger();
    info!("Starting rocket...");

    let reqwest_client = reqwest::Client::new();

    rocket::build()
        .manage(reqwest_client)
        .mount(
            "/api",
            routes![
                api::health_check_endpoint::health_check,
                api::create_submission_endpoint::create_submission,
            ],
        )
        .register("/", catchers![rocket_validation::validation_catcher,])
}
