use std::fmt;
use std::time::SystemTime;
use uuid::Uuid;

#[derive(Debug, Clone)]
pub struct Submission {
    pub id: Uuid,
    pub user_id: Uuid,
    pub problem_id: Uuid,
    pub language: Language,
    pub source_code: String,
    pub status: SubmissionStatus,
    pub score: i32,
    pub created_at: SystemTime,
    pub test_cases: Vec<TestCase>,
}

#[derive(Debug, Clone)]
pub struct TestCase {
    pub token: Uuid,
    pub submission_id: Uuid,
    pub testcase_id: i32,
    pub status: TestCaseStatus,
    pub time: f32,
    pub memory: f32,
    pub score: i32,
    pub expected_score: i32,
    pub eval_message: Option<String>,
    pub stdout: Option<String>,
    pub stderr: Option<String>,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum SubmissionStatus {
    Evaluating,
    Accepted,
    Rejected,
    InternalError,
    Unknown,
}

impl std::str::FromStr for SubmissionStatus {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "Evaluating" => Ok(SubmissionStatus::Evaluating),
            "Accepted" => Ok(SubmissionStatus::Accepted),
            "Rejected" => Ok(SubmissionStatus::Rejected),
            "Internal Error" => Ok(SubmissionStatus::InternalError),
            _ => Err(()),
        }
    }
}

impl fmt::Display for SubmissionStatus {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            SubmissionStatus::Evaluating => write!(f, "Evaluating"),
            SubmissionStatus::Accepted => write!(f, "Accepted"),
            SubmissionStatus::Rejected => write!(f, "Rejected"),
            SubmissionStatus::InternalError => write!(f, "Internal Error"),
            SubmissionStatus::Unknown => write!(f, "Unknown"),
        }
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum Language {
    C,
    Cpp,
    Java,
    Kotlin,
    Python,
    Rust,
    Go,
    CSharp,
    Haskell,
    Javascript,
}

impl std::str::FromStr for Language {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "49" => Ok(Language::C),
            "54" => Ok(Language::Cpp),
            "62" => Ok(Language::Java),
            "78" => Ok(Language::Kotlin),
            "71" => Ok(Language::Python),
            "73" => Ok(Language::Rust),
            "60" => Ok(Language::Go),
            "51" => Ok(Language::CSharp),
            "61" => Ok(Language::Haskell),
            "63" => Ok(Language::Javascript),
            _ => Err(()),
        }
    }
}

impl fmt::Display for Language {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Language::C => write!(f, "C"),
            Language::Cpp => write!(f, "C++"),
            Language::Java => write!(f, "Java"),
            Language::Kotlin => write!(f, "Kotlin"),
            Language::Python => write!(f, "Python"),
            Language::Rust => write!(f, "Rust"),
            Language::Go => write!(f, "Go"),
            Language::CSharp => write!(f, "C#"),
            Language::Haskell => write!(f, "Haskell"),
            Language::Javascript => write!(f, "Javascript"),
        }
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum TestCaseStatus {
    Pending,
    Running,
    Accepted,
    WrongAnswer,
    SIGSEGV,
    SIGXFSZ,
    SIGFPE,
    SIGABRT,
    NZEC,
    InternalError,
    ExecFormatError,
    RuntimeError,
    CompilationError,
    TimeLimitExceeded,
    Unknown,
}

impl std::str::FromStr for TestCaseStatus {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "Pending" => Ok(TestCaseStatus::Pending),
            "Running" => Ok(TestCaseStatus::Running),
            "Accepted" => Ok(TestCaseStatus::Accepted),
            "Wrong Answer" => Ok(TestCaseStatus::WrongAnswer),
            "SIGSEGV" => Ok(TestCaseStatus::SIGSEGV),
            "SIGXFSZ" => Ok(TestCaseStatus::SIGXFSZ),
            "SIGFPE" => Ok(TestCaseStatus::SIGFPE),
            "SIGABRT" => Ok(TestCaseStatus::SIGABRT),
            "NZEC" => Ok(TestCaseStatus::NZEC),
            "Internal Error" => Ok(TestCaseStatus::InternalError),
            "Exec Format Error" => Ok(TestCaseStatus::ExecFormatError),
            "Runtime Error" => Ok(TestCaseStatus::RuntimeError),
            "Compilation Error" => Ok(TestCaseStatus::CompilationError),
            "Time Limit Exceeded" => Ok(TestCaseStatus::TimeLimitExceeded),
            _ => Err(()),
        }
    }
}

impl fmt::Display for TestCaseStatus {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            TestCaseStatus::Pending => write!(f, "Pending"),
            TestCaseStatus::Running => write!(f, "Running"),
            TestCaseStatus::Accepted => write!(f, "Accepted"),
            TestCaseStatus::WrongAnswer => write!(f, "Wrong Answer"),
            TestCaseStatus::SIGSEGV => write!(f, "SIGSEGV"),
            TestCaseStatus::SIGXFSZ => write!(f, "SIGXFSZ"),
            TestCaseStatus::SIGFPE => write!(f, "SIGFPE"),
            TestCaseStatus::SIGABRT => write!(f, "SIGABRT"),
            TestCaseStatus::NZEC => write!(f, "NZEC"),
            TestCaseStatus::InternalError => write!(f, "Internal Error"),
            TestCaseStatus::ExecFormatError => write!(f, "Exec Format Error"),
            TestCaseStatus::RuntimeError => write!(f, "Runtime Error"),
            TestCaseStatus::CompilationError => write!(f, "Compilation Error"),
            TestCaseStatus::TimeLimitExceeded => write!(f, "Time Limit Exceeded"),
            TestCaseStatus::Unknown => write!(f, "Unknown"),
        }
    }
}
