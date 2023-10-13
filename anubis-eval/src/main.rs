use crate::config::logger::init_logger;
use crate::infrastructure::db::{run_migrations, Db};
use rocket::fairing::AdHoc;
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
        .attach(Db::fairing())
        .attach(AdHoc::on_ignite("Diesel Migrations", run_migrations))
        .mount(
            "/api",
            routes![
                api::health_check_endpoint::health_check,
                api::create_submission_endpoint::create_submission,
            ],
        )
        .register("/", catchers![rocket_validation::validation_catcher,])
}
