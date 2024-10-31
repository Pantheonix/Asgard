-- Your SQL goes here
CREATE TABLE tests (
    id INTEGER PRIMARY KEY,
    problem_id TEXT NOT NULL,
    score INTEGER NOT NULL,
    input_url TEXT NOT NULL,
    output_url TEXT NOT NULL
);

ALTER TABLE tests ADD CONSTRAINT problem_id_fk FOREIGN KEY (problem_id) REFERENCES problems(id);