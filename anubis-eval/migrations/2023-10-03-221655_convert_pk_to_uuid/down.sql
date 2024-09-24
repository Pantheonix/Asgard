-- This file should undo anything in `up.sql`
ALTER TABLE submissions ADD COLUMN new_id SERIAL;
UPDATE submissions SET new_id = (('x' || md5(id::text))::bit(32)::bigint);
ALTER TABLE submissions DROP COLUMN id;
ALTER TABLE submissions RENAME COLUMN new_id TO id;
ALTER TABLE submissions ADD PRIMARY KEY (id);

ALTER TABLE submissions_testcases ADD COLUMN new_token SERIAL;
UPDATE submissions_testcases SET new_token = (('x' || md5(token::text))::bit(32)::bigint);
ALTER TABLE submissions_testcases DROP COLUMN token;
ALTER TABLE submissions_testcases RENAME COLUMN new_token TO token;
ALTER TABLE submissions_testcases ADD PRIMARY KEY (token);

ALTER TABLE submissions_testcases ADD COLUMN new_submission_id SERIAL;
UPDATE submissions_testcases SET new_submission_id = (('x' || md5(submission_id::text))::bit(32)::bigint);
ALTER TABLE submissions_testcases DROP COLUMN submission_id;
ALTER TABLE submissions_testcases RENAME COLUMN new_submission_id TO submission_id;
