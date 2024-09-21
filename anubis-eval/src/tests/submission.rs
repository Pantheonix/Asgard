#[cfg(test)]
pub mod tests {
    use crate::domain;
    use cder::{Dict, StructLoader};
    use lazy_static::lazy_static;
    use serde::Deserialize;
    use std::time::SystemTime;

    #[derive(Debug, Clone, Deserialize)]
    pub struct Submission {
        pub id: String,
        pub user_id: String,
        pub problem_id: String,
        pub language: String,
        pub source_code: String,
        pub status: String,
        pub score: i32,
        pub created_at: String,
        pub avg_time: f32,
        pub avg_memory: f32,
    }

    impl From<(Submission, Vec<TestCase>)> for domain::submission::Submission {
        fn from((submission, test_cases): (Submission, Vec<TestCase>)) -> Self {
            domain::submission::Submission::new(
                submission.id.parse().unwrap(),
                submission.user_id.parse().unwrap(),
                submission.problem_id.parse().unwrap(),
                submission.language.into(),
                submission.source_code,
                submission.status.into(),
                submission.score,
                SystemTime::now(),
                Some(submission.avg_time),
                Some(submission.avg_memory),
                test_cases
                    .into_iter()
                    .map(|test_case| test_case.into())
                    .collect(),
            )
        }
    }

    #[derive(Debug, Clone, Deserialize)]
    pub struct TestCase {
        pub token: String,
        pub submission_id: String,
        pub status: String,
        pub time: f32,
        pub memory: f32,
        pub eval_message: String,
        pub stdout: String,
        pub stderr: String,
        pub testcase_id: i32,
        pub expected_score: i32,
        pub compile_output: String,
    }

    impl From<TestCase> for domain::submission::TestCase {
        fn from(test_case: TestCase) -> Self {
            domain::submission::TestCase::new(
                test_case.token.parse().unwrap(),
                test_case.submission_id.parse().unwrap(),
                test_case.testcase_id,
                test_case.status.into(),
                test_case.time,
                test_case.memory,
                test_case.expected_score,
                Some(test_case.eval_message),
                Some(test_case.compile_output),
                Some(test_case.stdout),
                Some(test_case.stderr),
            )
        }
    }

    lazy_static! {
        pub static ref SUBMISSIONS: StructLoader<Submission> = {
            let mut loader =
                StructLoader::<Submission>::new("submissions.yaml", "tests-setup/fixtures");
            loader.load(&Dict::<String>::new()).unwrap();

            loader
        };
        pub static ref TEST_CASES: StructLoader<TestCase> = {
            let mut loader =
                StructLoader::<TestCase>::new("test_cases.yaml", "tests-setup/fixtures");
            loader.load(&Dict::<String>::new()).unwrap();

            loader
        };
    }
}
