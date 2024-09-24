-- Your SQL goes here
CREATE TABLE submissions (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    problem_id INTEGER NOT NULL,
    language VARCHAR(255) NOT NULL,
    source_code TEXT NOT NULL,
    status VARCHAR(255) NOT NULL,
    score INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE submissions_testcases (
    token SERIAL PRIMARY KEY,
    submission_id INTEGER NOT NULL,
    status VARCHAR(255) NOT NULL,
    time DECIMAL NOT NULL,
    memory DECIMAL NOT NULL,
    score INTEGER NOT NULL,
    answer TEXT NOT NULL,
    eval_message TEXT NULL,
    compile_output TEXT NULL
);