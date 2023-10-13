use rocket::{Build, Rocket};
use rocket_sync_db_pools::database;

#[database("anubis-submissions")]
pub struct Db(diesel::PgConnection);

pub async fn run_migrations(rocket: Rocket<Build>) -> Rocket<Build> {
    use diesel_migrations::{embed_migrations, EmbeddedMigrations, MigrationHarness};

    const MIGRATIONS: EmbeddedMigrations = embed_migrations!("migrations");

    Db::get_one(&rocket)
        .await
        .expect("database connection missing")
        .run(|conn| {
            conn.run_pending_migrations(MIGRATIONS)
                .expect("diesel migrations missing");
        })
        .await;

    rocket
}
