use crate::config::logger::init_logger;
use rocket::log::private::info;
use rocket::{launch, routes};

mod api;
mod application;
mod config;
mod domain;
mod infrastructure;

#[launch]
fn rocket() -> _ {
    // init_logger();
    info!("Starting rocket...");

    rocket::build().mount("/", routes![api::health_check_endpoint::health_check,])
}
