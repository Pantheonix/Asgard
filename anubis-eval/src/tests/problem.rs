#[cfg(test)]
pub mod tests {
    use crate::domain;
    use cder::{Dict, StructLoader};
    use lazy_static::lazy_static;
    use serde::Deserialize;

    #[derive(Debug, Clone, Deserialize)]
    pub struct Problem {
        pub id: String,
        pub name: String,
        pub proposer_id: String,
        pub is_published: bool,
        pub time: f32,
        pub stack_memory: f32,
        pub total_memory: f32,
    }

    impl From<(Problem, Vec<Test>)> for domain::problem::Problem {
        fn from((problem, tests): (Problem, Vec<Test>)) -> Self {
            domain::problem::Problem::new(
                problem.id.parse().unwrap(),
                problem.name,
                problem.proposer_id.parse().unwrap(),
                problem.is_published,
                Some(problem.time),
                Some(problem.stack_memory),
                Some(problem.total_memory),
                tests.into_iter().map(|test| test.into()).collect(),
            )
        }
    }

    #[derive(Debug, Clone, Deserialize)]
    pub struct Test {
        pub id: i32,
        pub problem_id: String,
        pub score: i32,
        pub input_url: String,
        pub output_url: String,
    }

    impl From<Test> for domain::test::Test {
        fn from(test: Test) -> Self {
            domain::test::Test::new(
                test.id,
                test.problem_id.parse().unwrap(),
                test.score,
                test.input_url,
                test.output_url,
            )
        }
    }

    lazy_static! {
        pub static ref PROBLEMS: StructLoader<Problem> = {
            let mut loader = StructLoader::<Problem>::new("problems.yaml", "tests-setup/fixtures");
            loader.load(&Dict::<String>::new()).unwrap();

            loader
        };
        pub static ref TESTS: StructLoader<Test> = {
            let mut loader = StructLoader::<Test>::new("tests.yaml", "tests-setup/fixtures");
            loader.load(&Dict::<String>::new()).unwrap();

            loader
        };
    }
}
