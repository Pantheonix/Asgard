-- Your SQL goes here
ALTER TABLE submissions ALTER COLUMN id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN id TYPE text;
ALTER TABLE submissions ALTER COLUMN id TYPE uuid USING id::uuid;

ALTER TABLE submissions_testcases ALTER COLUMN token DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN token TYPE text;
ALTER TABLE submissions_testcases ALTER COLUMN token TYPE uuid USING token::uuid;

ALTER TABLE submissions_testcases ALTER COLUMN submission_id DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN submission_id TYPE text;
ALTER TABLE submissions_testcases ALTER COLUMN submission_id TYPE uuid USING submission_id::uuid;