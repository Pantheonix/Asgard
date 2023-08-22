use serde::{Deserialize, Serialize};
use std::fmt::Display;

#[derive(Debug, Deserialize, Serialize, Clone, Copy, PartialEq, Eq, Hash)]
pub enum ProgrammingLanguage {
    C,
    Cpp,
    Java,
    CSharp,
    Go,
    Rust,
    Dart,
}

#[derive(Debug, Deserialize, Serialize, Clone, Copy, PartialEq, Eq, Hash)]
pub enum SubmissionStatus {
    Pending,
    Evaluated,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, Hash)]
pub enum IoType {
    Standard,
    File,
}

impl<'de> Deserialize<'de> for IoType {
    fn deserialize<D>(deserializer: D) -> Result<IoType, D::Error>
    where
        D: serde::Deserializer<'de>,
    {
        let s = i32::deserialize(deserializer)?;
        match s {
            0 => Ok(IoType::Standard),
            1 => Ok(IoType::File),
            _ => Err(serde::de::Error::custom(format!(
                "Invalid IoType value: {}",
                s
            ))),
        }
    }
}

impl Serialize for IoType {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: serde::Serializer,
    {
        match self {
            IoType::Standard => serializer.serialize_i32(0),
            IoType::File => serializer.serialize_i32(1),
        }
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct TestExecutionResult {
    pub test_id: i32,
    pub time: i32,
    pub memory: i32,
    pub grade: i32,
    pub runtime_errors: Option<Vec<String>>,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct Test {
    pub test_id: i32,
    pub score: i32,
    pub input_url: String,
    pub output_url: String,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct Submission {
    pub submission_id: uuid::Uuid,
    pub user_id: uuid::Uuid,
    pub problem_id: uuid::Uuid,
    pub programming_language: ProgrammingLanguage,
    pub source_code: String,
    pub status: SubmissionStatus,
    pub time_limit: i32,
    pub total_memory_limit: i32,
    pub stack_memory_limit: i32,
    pub io_type: IoType,
    pub tests: Vec<Test>,
    pub compiletime_errors: Option<Vec<String>>,
    pub test_results: Vec<TestExecutionResult>,
    pub total_grade: i32,
}

impl Display for ProgrammingLanguage {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            ProgrammingLanguage::C => write!(f, "C"),
            ProgrammingLanguage::Cpp => write!(f, "C++"),
            ProgrammingLanguage::Java => write!(f, "Java"),
            ProgrammingLanguage::CSharp => write!(f, "C#"),
            ProgrammingLanguage::Go => write!(f, "Go"),
            ProgrammingLanguage::Rust => write!(f, "Rust"),
            ProgrammingLanguage::Dart => write!(f, "Dart"),
        }
    }
}

impl Display for SubmissionStatus {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            SubmissionStatus::Pending => write!(f, "Pending"),
            SubmissionStatus::Evaluated => write!(f, "Evaluated"),
        }
    }
}

impl Display for IoType {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            IoType::Standard => write!(f, "Standard"),
            IoType::File => write!(f, "File"),
        }
    }
}

impl Display for TestExecutionResult {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let mut errors = String::new();
        if let Some(runtime_errors) = &self.runtime_errors {
            for error in runtime_errors {
                errors.push_str(&format!("{}\n", error));
            }
        }

        write!(
            f,
            "Test ID: {}\nTest Name: {}\nTest Status: {}\nTest Grade: {}\nTest Errors: {}",
            self.test_id, self.time, self.memory, self.grade, errors
        )
    }
}

impl Display for Test {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "Test ID: {}\nTest Score: {}\nTest Input URL: {}\nTest Output URL: {}",
            self.test_id, self.score, self.input_url, self.output_url
        )
    }
}

impl Display for Submission {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let mut compiletime_errors = String::new();
        if let Some(errors) = &self.compiletime_errors {
            for error in errors {
                compiletime_errors.push_str(&format!("{}\n", error));
            }
        }

        let mut test_results = String::new();
        for test_result in &self.test_results {
            test_results.push_str(&format!("{}\n", test_result));
        }

        let mut tests = String::new();
        for test in &self.tests {
            tests.push_str(&format!("{}\n", test));
        }

        write!(
            f,
            "Submission ID: {}\nUser ID: {}\nProblem ID: {}\nProgramming Language: {}\nSource Code: {}\nStatus: {}\nTime Limit: {}\nTotal Memory Limit: {}\nStack Memory Limit: {}\nI/O Type: {}\nTests: {}\nCompiletime Errors: {}\nTest Results: {}\nTotal Grade: {}",
            self.submission_id,
            self.user_id,
            self.problem_id,
            self.programming_language,
            self.source_code,
            self.status,
            self.time_limit,
            self.total_memory_limit,
            self.stack_memory_limit,
            self.io_type,
            tests,
            compiletime_errors,
            test_results,
            self.total_grade
        )
    }
}
