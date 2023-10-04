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
    pub status: Status,
    pub score: i32,
    pub created_at: SystemTime,
    pub test_cases: Vec<TestCase>,
}

#[derive(Debug, Clone)]
pub struct TestCase {
    pub token: Uuid,
    pub submission_id: Uuid,
    pub testcase_id: i32,
    pub status: Status,
    pub time: f32,
    pub memory: f32,
    pub score: i32,
    pub eval_message: Option<String>,
    pub stdout: Option<String>,
    pub stderr: Option<String>,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum Language {
    C,
    Cpp,
    Java,
    Python,
    Rust,
    CSharp,
    Haskell,
    Javascript,
}

impl std::str::FromStr for Language {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "C" => Ok(Language::C),
            "C++" => Ok(Language::Cpp),
            "Java" => Ok(Language::Java),
            "Python" => Ok(Language::Python),
            "Rust" => Ok(Language::Rust),
            "C#" => Ok(Language::CSharp),
            "Haskell" => Ok(Language::Haskell),
            "Javascript" => Ok(Language::Javascript),
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
            Language::Python => write!(f, "Python"),
            Language::Rust => write!(f, "Rust"),
            Language::CSharp => write!(f, "C#"),
            Language::Haskell => write!(f, "Haskell"),
            Language::Javascript => write!(f, "Javascript"),
        }
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum Status {
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

impl std::str::FromStr for Status {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s {
            "Pending" => Ok(Status::Pending),
            "Running" => Ok(Status::Running),
            "Accepted" => Ok(Status::Accepted),
            "Wrong Answer" => Ok(Status::WrongAnswer),
            "SIGSEGV" => Ok(Status::SIGSEGV),
            "SIGXFSZ" => Ok(Status::SIGXFSZ),
            "SIGFPE" => Ok(Status::SIGFPE),
            "SIGABRT" => Ok(Status::SIGABRT),
            "NZEC" => Ok(Status::NZEC),
            "Internal Error" => Ok(Status::InternalError),
            "Exec Format Error" => Ok(Status::ExecFormatError),
            "Runtime Error" => Ok(Status::RuntimeError),
            "Compilation Error" => Ok(Status::CompilationError),
            "Time Limit Exceeded" => Ok(Status::TimeLimitExceeded),
            _ => Err(()),
        }
    }
}

impl fmt::Display for Status {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Status::Pending => write!(f, "Pending"),
            Status::Running => write!(f, "Running"),
            Status::Accepted => write!(f, "Accepted"),
            Status::WrongAnswer => write!(f, "Wrong Answer"),
            Status::SIGSEGV => write!(f, "SIGSEGV"),
            Status::SIGXFSZ => write!(f, "SIGXFSZ"),
            Status::SIGFPE => write!(f, "SIGFPE"),
            Status::SIGABRT => write!(f, "SIGABRT"),
            Status::NZEC => write!(f, "NZEC"),
            Status::InternalError => write!(f, "Internal Error"),
            Status::ExecFormatError => write!(f, "Exec Format Error"),
            Status::RuntimeError => write!(f, "Runtime Error"),
            Status::CompilationError => write!(f, "Compilation Error"),
            Status::TimeLimitExceeded => write!(f, "Time Limit Exceeded"),
            Status::Unknown => write!(f, "Unknown"),
        }
    }
}
