-- This file should undo anything in `up.sql`
ALTER TABLE submissions ADD COLUMN new_user_id SERIAL;
UPDATE submissions SET new_user_id = (('x' || md5(user_id::text))::bit(32)::bigint);
ALTER TABLE submissions DROP COLUMN user_id;
ALTER TABLE submissions RENAME COLUMN new_user_id TO user_id;

ALTER TABLE submissions ADD COLUMN new_problem_id SERIAL;
UPDATE submissions SET new_problem_id = (('x' || md5(problem_id::text))::bit(32)::bigint);
ALTER TABLE submissions DROP COLUMN problem_id;
ALTER TABLE submissions RENAME COLUMN new_problem_id TO problem_id;
