-- Your SQL goes here
-- submission
ALTER TABLE submissions ALTER COLUMN id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN id TYPE text;

ALTER TABLE submissions ALTER COLUMN user_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN user_id TYPE text;

ALTER TABLE submissions ALTER COLUMN problem_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN problem_id TYPE text;

-- testcase
ALTER TABLE submissions_testcases ALTER COLUMN token DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN token TYPE text;

ALTER TABLE submissions_testcases ALTER COLUMN submission_id DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN submission_id TYPE text;
ALTER TABLE submissions_testcases ADD FOREIGN KEY (submission_id) REFERENCES submissions(id);