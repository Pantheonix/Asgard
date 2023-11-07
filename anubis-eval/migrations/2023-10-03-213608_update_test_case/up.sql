-- Your SQL goes here
ALTER TABLE submissions_testcases DROP COLUMN compile_output;
ALTER TABLE submissions_testcases DROP COLUMN answer;

ALTER TABLE submissions_testcases ADD COLUMN stdout TEXT;
ALTER TABLE submissions_testcases ADD COLUMN stderr TEXT;