-- Your SQL goes here
CREATE TABLE problems (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    proposer_id TEXT NOT NULL,
    is_published BOOLEAN NOT NULL,
    time REAL DEFAULT 0.0,
    stack_memory REAL DEFAULT 0.0,
    total_memory REAL DEFAULT 0.0
);

ALTER TABLE submissions ADD CONSTRAINT fk_submissions_problems
    FOREIGN KEY (problem_id) REFERENCES problems (id);