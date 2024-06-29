pub mod problem;
pub mod submission;
pub mod user;

#[cfg(test)]
pub mod common {
    use std::ops::DerefMut;
    use std::sync::Arc;

    use crate::application::dapr_client::DaprClient;
    use async_once::AsyncOnce;
    use diesel::PgConnection;
    use lazy_static::lazy_static;
    use rocket::info;
    use rocket::local::asynchronous::Client;
    use serde_json::Value;

    use crate::config::di::{Atomic, DAPR_CLIENT, DB_CONN};
    use crate::config::logger::tests::init_logger;
    use crate::contracts::dapr_dtos::StateStoreSetItemDto;
    use crate::domain::problem::Problem;
    use crate::domain::submission::Submission;
    use crate::rocket;
    use crate::tests::problem::tests::{PROBLEMS, TESTS};
    use crate::tests::submission::tests::{SUBMISSIONS, TEST_CASES};

    pub type Result<T> = std::result::Result<T, Box<dyn std::error::Error + 'static>>;

    lazy_static! {
        pub static ref ROCKET_CLIENT: AsyncOnce<Arc<Client>> = AsyncOnce::new(async {
            let client = setup_rocket().await.expect("Failed to setup rocket client");
            Arc::new(client)
        });
    }

    #[ctor::ctor]
    fn setup() {
        dotenvy::from_filename(".env.template").expect("Failed to load env vars");
        init_logger();
    }

    async fn setup_rocket() -> Result<Client> {
        let client = Client::tracked(rocket().await).await?;
        seed().await?;

        Ok(client)
    }

    async fn seed() -> Result<()> {
        let conn = DB_CONN.clone();
        seed_problems(conn.clone()).await?;
        seed_submissions(conn.clone()).await?;

        let dapr_client = DAPR_CLIENT.clone();
        seed_tests_cache(dapr_client.clone()).await?;

        Ok(())
    }

    async fn seed_problems(conn: Atomic<PgConnection>) -> Result<()> {
        let mut db = conn.lock().await;

        let raw_test1 = TESTS.get("SumAB_Test1")?.to_owned();
        let raw_test2 = TESTS.get("SumAB_Test2")?.to_owned();

        let raw_problem = PROBLEMS.get("SumAB")?.to_owned();
        let problem: Problem = (raw_problem, vec![raw_test1, raw_test2]).into();

        problem.upsert(db.deref_mut())?;

        let raw_test1 = TESTS.get("DiffAB_Test1")?.to_owned();

        let raw_problem = PROBLEMS.get("DiffAB")?.to_owned();
        let problem: Problem = (raw_problem, vec![raw_test1]).into();

        problem.upsert(db.deref_mut())?;

        Ok(())
    }

    async fn seed_submissions(conn: Atomic<PgConnection>) -> Result<()> {
        let mut db = conn.lock().await;

        let raw_testcase1 = TEST_CASES.get("Submission1_TestCase1")?.to_owned();
        let raw_testcase2 = TEST_CASES.get("Submission1_TestCase2")?.to_owned();

        let raw_submission = SUBMISSIONS.get("Ordinary_SumAB_Submission1")?.to_owned();
        let submission: Submission = (raw_submission, vec![raw_testcase1, raw_testcase2]).into();

        submission.upsert(db.deref_mut())?;

        let raw_testcase1 = TEST_CASES.get("Submission2_TestCase1")?.to_owned();
        let raw_testcase2 = TEST_CASES.get("Submission2_TestCase2")?.to_owned();

        let raw_submission = SUBMISSIONS.get("Proposer_SumAB_Submission2")?.to_owned();
        let submission: Submission = (raw_submission, vec![raw_testcase1, raw_testcase2]).into();

        submission.upsert(db.deref_mut())?;

        let raw_testcase1 = TEST_CASES.get("Submission3_TestCase1")?.to_owned();
        let raw_testcase2 = TEST_CASES.get("Submission3_TestCase2")?.to_owned();

        let raw_submission = SUBMISSIONS.get("Admin_SumAB_Submission3")?.to_owned();
        let submission: Submission = (raw_submission, vec![raw_testcase1, raw_testcase2]).into();

        submission.upsert(db.deref_mut())?;

        let raw_testcase1 = TEST_CASES.get("Submission4_TestCase1")?.to_owned();

        let raw_submission = SUBMISSIONS.get("Admin_DiffAB_Submission4")?.to_owned();
        let submission: Submission = (raw_submission, vec![raw_testcase1]).into();

        submission.upsert(db.deref_mut())?;

        let raw_testcase1 = TEST_CASES.get("Submission5_TestCase1")?.to_owned();

        let raw_submission = SUBMISSIONS.get("Ordinary_DiffAB_Submission5")?.to_owned();
        let submission: Submission = (raw_submission, vec![raw_testcase1]).into();

        submission.upsert(db.deref_mut())?;

        Ok(())
    }

    async fn seed_tests_cache(dapr_client: Atomic<DaprClient>) -> Result<()> {
        let dapr_client = dapr_client.lock().await;

        let problem1_id = PROBLEMS.get("SumAB")?.id.clone();
        dapr_client
            .save_test_for_problem(1, problem1_id.clone(), ("1 2".to_string(), "3".to_string()))
            .await?;
        dapr_client
            .save_test_for_problem(2, problem1_id.clone(), ("3 4".to_string(), "7".to_string()))
            .await?;

        let problem2_id = PROBLEMS.get("DiffAB")?.id.clone();
        dapr_client
            .save_test_for_problem(
                1,
                problem2_id.clone(),
                ("1 2".to_string(), "-1".to_string()),
            )
            .await?;

        Ok(())
    }

    impl DaprClient {
        async fn save_test_for_problem(
            &self,
            test_id: usize,
            problem_id: String,
            (input, output): (String, String),
        ) -> Result<()> {
            let input_key = format!("{}-{}-input", problem_id, test_id);
            info!("Saving test input: {}", input_key);
            let input_item = StateStoreSetItemDto {
                key: input_key.clone(),
                value: Value::String(input.clone()),
            };

            let output_key = format!("{}-{}-output", problem_id, test_id);
            let output_item = StateStoreSetItemDto {
                key: output_key,
                value: Value::String(output.clone()),
            };

            self.set_items_in_state_store(vec![input_item, output_item])
                .await?;

            let val = self
                .get_item_from_state_store(input_key.clone().as_str())
                .await?;
            info!("Value: {:?}", val);

            Ok(())
        }
    }
}
