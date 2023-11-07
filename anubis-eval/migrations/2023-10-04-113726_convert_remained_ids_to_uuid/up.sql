-- Your SQL goes here
ALTER TABLE submissions ALTER COLUMN user_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN user_id TYPE text;
ALTER TABLE submissions ALTER COLUMN user_id TYPE uuid USING user_id::uuid;

ALTER TABLE submissions ALTER COLUMN problem_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN problem_id TYPE text;
ALTER TABLE submissions ALTER COLUMN problem_id TYPE uuid USING problem_id::uuid;
