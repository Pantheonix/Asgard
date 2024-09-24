-- This file should undo anything in `up.sql`
ALTER TABLE submissions_testcases DROP CONSTRAINT submissions_testcases_submission_id_fkey;

-- submissions
ALTER TABLE submissions ALTER COLUMN id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN id TYPE uuid USING id::uuid;

ALTER TABLE submissions ALTER COLUMN user_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN user_id TYPE uuid USING user_id::uuid;

ALTER TABLE submissions ALTER COLUMN problem_id DROP DEFAULT;
ALTER TABLE submissions ALTER COLUMN problem_id TYPE uuid USING problem_id::uuid;

-- testcases
ALTER TABLE submissions_testcases ALTER COLUMN token DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN token TYPE uuid USING token::uuid;

ALTER TABLE submissions_testcases ALTER COLUMN submission_id DROP DEFAULT;
ALTER TABLE submissions_testcases ALTER COLUMN submission_id TYPE uuid USING submission_id::uuid;