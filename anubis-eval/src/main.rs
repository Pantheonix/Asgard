use crate::config::di::{CONFIG, DAPR_CLIENT, DB_CONN, REQWEST_CLIENT};
use crate::config::logger::init_logger;
use crate::infrastructure::db::{run_migrations, Db};
use rocket::fairing::AdHoc;
use rocket::log::private::info;
use rocket::{catchers, error, launch, routes};
use tokio_cron_scheduler::{Job, JobScheduler};

mod api;
mod application;
mod config;
mod domain;
mod infrastructure;
mod schema;

#[launch]
async fn rocket() -> _ {
    init_logger();
    info!("Starting rocket...");

    let config = CONFIG.clone();
    let scheduler = JobScheduler::new()
        .await
        .expect("Failed to create scheduler");
    scheduler
        .add(
            Job::new_async(config.eval_cron_schedule.as_str(), |_, _| {
                Box::pin(async {
                    let response = api::evaluate_job::evaluate_pending_submissions(
                        DAPR_CLIENT.clone(),
                        DB_CONN.clone(),
                    )
                    .await;

                    match response {
                        Ok(_) => info!("Successfully evaluated pending submissions"),
                        Err(e) => error!("Failed to evaluate pending submissions: {:?}", e),
                    }
                })
            })
            .expect("Failed to add evaluation job"),
        )
        .await
        .unwrap();
    scheduler.start().await.unwrap();

    rocket::build()
        .manage(REQWEST_CLIENT.clone())
        .attach(Db::fairing())
        .attach(AdHoc::on_ignite("Diesel Migrations", run_migrations))
        .mount(
            "/api",
            routes![
                api::health_check_endpoint::health_check,
                api::create_submission_endpoint::create_submission,
                api::get_submission_endpoint::get_submission,
                api::get_submissions_endpoint::get_submissions,
            ],
        )
        .register(
            "/api",
            catchers![
                rocket_validation::validation_catcher,
                api::catchers::unauthorized_catcher,
            ],
        )
}
