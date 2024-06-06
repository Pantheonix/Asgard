use diesel::PgConnection;
use std::fmt;
use std::time::SystemTime;
use uuid::Uuid;

#[derive(Debug, Clone)]
pub struct Submission {
    id: Uuid,
    user_id: Uuid,
    problem_id: Uuid,
    language: Language,
    source_code: String,
    status: SubmissionStatus,
    score: i32,
    created_at: SystemTime,
    avg_time: Option<f32>,
    avg_memory: Option<f32>,
    test_cases: Vec<TestCase>,
}

impl Submission {
    pub fn new(
        id: Uuid,
        user_id: Uuid,
        problem_id: Uuid,
        language: Language,
        source_code: String,
        status: SubmissionStatus,
        score: i32,
        created_at: SystemTime,
        avg_time: Option<f32>,
        avg_memory: Option<f32>,
        test_cases: Vec<TestCase>,
    ) -> Self {
        Self {
            id,
            user_id,
            problem_id,
            language,
            source_code,
            status,
            score,
            created_at,
            avg_time,
            avg_memory,
            test_cases,
        }
    }

    pub fn new_without_test_cases(
        id: Uuid,
        user_id: Uuid,
        problem_id: Uuid,
        language: Language,
        source_code: String,
        status: SubmissionStatus,
        score: i32,
        created_at: SystemTime,
        avg_time: Option<f32>,
        avg_memory: Option<f32>,
    ) -> Self {
        Self {
            id,
            user_id,
            problem_id,
            language,
            source_code,
            status,
            score,
            created_at,
            avg_time,
            avg_memory,
            test_cases: vec![],
        }
    }

    pub fn new_in_pending(
        id: Uuid,
        user_id: Uuid,
        problem_id: Uuid,
        language: Language,
        source_code: String,
        test_cases: Vec<TestCase>,
    ) -> Self {
        Self {
            id,
            user_id,
            problem_id,
            language,
            source_code,
            status: SubmissionStatus::Evaluating,
            score: 0,
            created_at: SystemTime::now(),
            avg_time: None,
            avg_memory: None,
            test_cases,
        }
    }

    pub fn new_with_metadata(
        id: Uuid,
        status: SubmissionStatus,
        score: i32,
        avg_time: Option<f32>,
        avg_memory: Option<f32>,
    ) -> Self {
        Self {
            id,
            user_id: Uuid::nil(),
            problem_id: Uuid::nil(),
            language: Language::Unknown,
            source_code: "".to_string(),
            status,
            score,
            created_at: SystemTime::now(),
            avg_time,
            avg_memory,
            test_cases: vec![],
        }
    }

    pub fn without_source_code(&self) -> Submission {
        Submission {
            id: self.id,
            user_id: self.user_id,
            problem_id: self.problem_id,
            language: self.language.clone(),
            source_code: "".to_string(),
            status: self.status.clone(),
            score: self.score,
            created_at: self.created_at,
            avg_time: self.avg_time,
            avg_memory: self.avg_memory,
            test_cases: self.test_cases.clone(),
        }
    }

    pub fn user_is_allowed_to_view_source_code(
        &self,
        user_id: &Uuid,
        proposer_id: &Uuid,
        conn: &mut PgConnection,
    ) -> bool {
        // user has solved the problem or is submission owner or is problem proposer
        let user_has_solved_problem = Submission::is_problem_solved_by_user(
            &user_id.to_string(),
            &self.problem_id.to_string(),
            conn,
        );

        user_has_solved_problem || self.user_id == *user_id || user_id == proposer_id
    }

    pub fn id(&self) -> Uuid {
        self.id
    }

    pub fn user_id(&self) -> Uuid {
        self.user_id
    }

    pub fn problem_id(&self) -> Uuid {
        self.problem_id
    }

    pub fn language(&self) -> Language {
        self.language.clone()
    }

    pub fn source_code(&self) -> &String {
        &self.source_code
    }

    pub fn status(&self) -> SubmissionStatus {
        self.status.clone()
    }

    pub fn set_status(&mut self, status: SubmissionStatus) {
        self.status = status;
    }

    pub fn score(&self) -> i32 {
        self.score
    }

    pub fn set_score(&mut self, score: i32) {
        self.score = score;
    }

    pub fn created_at(&self) -> SystemTime {
        self.created_at
    }

    pub fn avg_time(&self) -> Option<f32> {
        self.avg_time
    }

    pub fn set_avg_time(&mut self, avg_time: Option<f32>) {
        self.avg_time = avg_time;
    }

    pub fn avg_memory(&self) -> Option<f32> {
        self.avg_memory
    }

    pub fn set_avg_memory(&mut self, avg_memory: Option<f32>) {
        self.avg_memory = avg_memory;
    }

    pub fn test_cases(&self) -> Vec<TestCase> {
        self.test_cases.clone()
    }

    pub fn set_test_cases(&mut self, test_cases: Vec<TestCase>) {
        self.test_cases = test_cases;
    }
}

#[derive(Debug, Clone)]
pub struct TestCase {
    token: Uuid,
    submission_id: Uuid,
    testcase_id: i32,
    status: TestCaseStatus,
    time: f32,
    memory: f32,
    expected_score: i32,
    eval_message: Option<String>,
    compile_output: Option<String>,
    stdout: Option<String>,
    stderr: Option<String>,
}

impl TestCase {
    pub fn new(
        token: Uuid,
        submission_id: Uuid,
        testcase_id: i32,
        status: TestCaseStatus,
        time: f32,
        memory: f32,
        expected_score: i32,
        eval_message: Option<String>,
        compile_output: Option<String>,
        stdout: Option<String>,
        stderr: Option<String>,
    ) -> Self {
        Self {
            token,
            submission_id,
            testcase_id,
            status,
            time,
            memory,
            expected_score,
            eval_message,
            compile_output,
            stdout,
            stderr,
        }
    }

    pub fn new_for_update(
        token: Uuid,
        eval_message: Option<String>,
        compile_output: Option<String>,
        status: TestCaseStatus,
        time: f32,
        memory: f32,
        stdout: Option<String>,
        stderr: Option<String>,
    ) -> Self {
        Self {
            token,
            submission_id: Uuid::nil(),
            testcase_id: 0,
            status,
            time,
            memory,
            expected_score: 0,
            eval_message,
            compile_output,
            stdout,
            stderr,
        }
    }

    pub fn token(&self) -> Uuid {
        self.token
    }

    pub fn submission_id(&self) -> Uuid {
        self.submission_id
    }

    pub fn testcase_id(&self) -> i32 {
        self.testcase_id
    }

    pub fn status(&self) -> TestCaseStatus {
        self.status.clone()
    }

    pub fn time(&self) -> f32 {
        self.time
    }

    pub fn memory(&self) -> f32 {
        self.memory
    }

    pub fn expected_score(&self) -> i32 {
        self.expected_score
    }

    pub fn eval_message(&self) -> Option<String> {
        self.eval_message.clone()
    }

    pub fn compile_output(&self) -> Option<String> {
        self.compile_output.clone()
    }

    pub fn stdout(&self) -> Option<String> {
        self.stdout.clone()
    }

    pub fn stderr(&self) -> Option<String> {
        self.stderr.clone()
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub enum SubmissionStatus {
    Evaluating,
    Accepted,
    Rejected,
    InternalError,
    Unknown,
}

impl From<String> for SubmissionStatus {
    fn from(value: String) -> Self {
        match value.as_str() {
            "Evaluating" => SubmissionStatus::Evaluating,
            "Accepted" => SubmissionStatus::Accepted,
            "Rejected" => SubmissionStatus::Rejected,
            "Internal Error" => SubmissionStatus::InternalError,
            _ => SubmissionStatus::Unknown,
        }
    }
}

impl From<SubmissionStatus> for String {
    fn from(value: SubmissionStatus) -> Self {
        match value {
            SubmissionStatus::Evaluating => "Evaluating".to_string(),
            SubmissionStatus::Accepted => "Accepted".to_string(),
            SubmissionStatus::Rejected => "Rejected".to_string(),
            SubmissionStatus::InternalError => "Internal Error".to_string(),
            SubmissionStatus::Unknown => "Unknown".to_string(),
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
    Lua,
    Python,
    Rust,
    Go,
    CSharp,
    OCaml,
    Javascript,
    Kotlin,
    Haskell,
    Unknown,
}

impl From<usize> for Language {
    fn from(value: usize) -> Self {
        match value {
            49 => Language::C,
            54 => Language::Cpp,
            62 => Language::Java,
            64 => Language::Lua,
            71 => Language::Python,
            73 => Language::Rust,
            60 => Language::Go,
            51 => Language::CSharp,
            65 => Language::OCaml,
            63 => Language::Javascript,
            78 => Language::Kotlin,
            61 => Language::Haskell,
            _ => Language::Unknown,
        }
    }
}

impl From<Language> for usize {
    fn from(value: Language) -> Self {
        match value {
            Language::C => 49,
            Language::Cpp => 54,
            Language::Java => 62,
            Language::Lua => 64,
            Language::Python => 71,
            Language::Rust => 73,
            Language::Go => 60,
            Language::CSharp => 51,
            Language::OCaml => 65,
            Language::Javascript => 63,
            Language::Kotlin => 78,
            Language::Haskell => 61,
            Language::Unknown => 0,
        }
    }
}

impl From<String> for Language {
    fn from(value: String) -> Self {
        match value.as_str() {
            "C" => Language::C,
            "C++" => Language::Cpp,
            "Java" => Language::Java,
            "Lua" => Language::Lua,
            "Python" => Language::Python,
            "Rust" => Language::Rust,
            "Go" => Language::Go,
            "C#" => Language::CSharp,
            "OCaml" => Language::OCaml,
            "Javascript" => Language::Javascript,
            "Kotlin" => Language::Kotlin,
            "Haskell" => Language::Haskell,
            _ => Language::Unknown,
        }
    }
}

impl From<Language> for String {
    fn from(value: Language) -> Self {
        match value {
            Language::C => "C".to_string(),
            Language::Cpp => "C++".to_string(),
            Language::Java => "Java".to_string(),
            Language::Lua => "Lua".to_string(),
            Language::Python => "Python".to_string(),
            Language::Rust => "Rust".to_string(),
            Language::Go => "Go".to_string(),
            Language::CSharp => "C#".to_string(),
            Language::OCaml => "OCaml".to_string(),
            Language::Javascript => "Javascript".to_string(),
            Language::Kotlin => "Kotlin".to_string(),
            Language::Haskell => "Haskell".to_string(),
            Language::Unknown => "Unknown".to_string(),
        }
    }
}

impl fmt::Display for Language {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Language::C => write!(f, "C"),
            Language::Cpp => write!(f, "C++"),
            Language::Java => write!(f, "Java"),
            Language::Lua => write!(f, "Lua"),
            Language::Python => write!(f, "Python"),
            Language::Rust => write!(f, "Rust"),
            Language::Go => write!(f, "Go"),
            Language::CSharp => write!(f, "C#"),
            Language::OCaml => write!(f, "OCaml"),
            Language::Javascript => write!(f, "Javascript"),
            Language::Kotlin => write!(f, "Kotlin"),
            Language::Haskell => write!(f, "Haskell"),
            Language::Unknown => write!(f, "Unknown"),
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

impl From<usize> for TestCaseStatus {
    fn from(value: usize) -> Self {
        match value {
            1 => TestCaseStatus::Pending,
            2 => TestCaseStatus::Running,
            3 => TestCaseStatus::Accepted,
            4 => TestCaseStatus::WrongAnswer,
            5 => TestCaseStatus::TimeLimitExceeded,
            6 => TestCaseStatus::CompilationError,
            7 => TestCaseStatus::SIGSEGV,
            8 => TestCaseStatus::SIGXFSZ,
            9 => TestCaseStatus::SIGFPE,
            10 => TestCaseStatus::SIGABRT,
            11 => TestCaseStatus::NZEC,
            12 => TestCaseStatus::RuntimeError,
            13 => TestCaseStatus::InternalError,
            14 => TestCaseStatus::ExecFormatError,
            _ => TestCaseStatus::Unknown,
        }
    }
}

impl From<String> for TestCaseStatus {
    fn from(value: String) -> Self {
        match value.as_str() {
            "Pending" => TestCaseStatus::Pending,
            "Running" => TestCaseStatus::Running,
            "Accepted" => TestCaseStatus::Accepted,
            "Wrong Answer" => TestCaseStatus::WrongAnswer,
            "SIGSEGV" => TestCaseStatus::SIGSEGV,
            "SIGXFSZ" => TestCaseStatus::SIGXFSZ,
            "SIGFPE" => TestCaseStatus::SIGFPE,
            "SIGABRT" => TestCaseStatus::SIGABRT,
            "NZEC" => TestCaseStatus::NZEC,
            "Internal Error" => TestCaseStatus::InternalError,
            "Exec Format Error" => TestCaseStatus::ExecFormatError,
            "Runtime Error" => TestCaseStatus::RuntimeError,
            "Compilation Error" => TestCaseStatus::CompilationError,
            "Time Limit Exceeded" => TestCaseStatus::TimeLimitExceeded,
            _ => TestCaseStatus::Unknown,
        }
    }
}

impl From<TestCaseStatus> for String {
    fn from(value: TestCaseStatus) -> Self {
        match value {
            TestCaseStatus::Pending => "Pending".to_string(),
            TestCaseStatus::Running => "Running".to_string(),
            TestCaseStatus::Accepted => "Accepted".to_string(),
            TestCaseStatus::WrongAnswer => "Wrong Answer".to_string(),
            TestCaseStatus::SIGSEGV => "SIGSEGV".to_string(),
            TestCaseStatus::SIGXFSZ => "SIGXFSZ".to_string(),
            TestCaseStatus::SIGFPE => "SIGFPE".to_string(),
            TestCaseStatus::SIGABRT => "SIGABRT".to_string(),
            TestCaseStatus::NZEC => "NZEC".to_string(),
            TestCaseStatus::InternalError => "Internal Error".to_string(),
            TestCaseStatus::ExecFormatError => "Exec Format Error".to_string(),
            TestCaseStatus::RuntimeError => "Runtime Error".to_string(),
            TestCaseStatus::CompilationError => "Compilation Error".to_string(),
            TestCaseStatus::TimeLimitExceeded => "Time Limit Exceeded".to_string(),
            TestCaseStatus::Unknown => "Unknown".to_string(),
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
