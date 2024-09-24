use crate::infrastructure::db::Db;
use diesel::RunQueryDsl;
use rocket::get;

#[get("/health_check")]
pub async fn health_check(db: Db) -> &'static str {
    db.run(
        move |conn| match diesel::sql_query("SELECT 1").execute(conn) {
            Ok(_) => "Healthy!",
            Err(_) => "Unhealthy!",
        },
    )
    .await
}

#[cfg(test)]
mod tests {
    use crate::tests::common::Result;
    use crate::tests::common::ROCKET_CLIENT;
    use rocket::http::Status;

    #[tokio::test]
    async fn test_health_check() -> Result<()> {
        let client = ROCKET_CLIENT.get().await.clone();

        let response = client.get("/api/health_check").dispatch().await;

        assert_eq!(response.status(), Status::Ok);
        assert_eq!(response.into_string().await.unwrap(), "Healthy!");

        Ok(())
    }
}
