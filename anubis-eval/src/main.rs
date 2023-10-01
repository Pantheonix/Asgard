use crate::config::logger::init_logger;
use rocket::log::private::info;
use rocket::{launch, routes};

mod api;
mod application;
mod config;
mod domain;
mod infrastructure;
mod schema;

#[launch]
fn rocket() -> _ {
    // init_logger();
    info!("Starting rocket...");

    rocket::build().mount(
        "/api",
        routes![
            api::health_check_endpoint::health_check,
            api::create_submission_endpoint::create_submission,
        ],
    )
}
