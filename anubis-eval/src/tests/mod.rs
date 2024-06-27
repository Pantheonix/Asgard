mod problem;
pub mod user;
mod submission;

#[cfg(test)]
pub mod common {
    use std::ops::DerefMut;
    use std::sync::Arc;

    use async_once::AsyncOnce;
    use diesel::PgConnection;
    use lazy_static::lazy_static;
    use rocket::local::asynchronous::Client;

    use crate::config::di::{Atomic, DB_CONN};
    use crate::config::logger::tests::init_logger;
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

        Ok(())
    }

    async fn seed_problems(conn: Atomic<PgConnection>) -> Result<()> {
        let mut db = conn.lock().await;

        let raw_test1= TESTS.get("SumAB_Test1")?.to_owned();
        let raw_test2 = TESTS.get("SumAB_Test2")?.to_owned();

        let raw_problem = PROBLEMS.get("SumAB")?.to_owned();
        let problem: Problem = (raw_problem, vec![raw_test1, raw_test2]).into();

        problem.upsert(db.deref_mut())?;

        let raw_problem = PROBLEMS.get("DiffAB")?.to_owned();
        let problem: Problem = (raw_problem, vec![]).into(); 

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
        
        Ok(())
    }
}