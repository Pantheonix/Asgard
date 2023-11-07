-- This file should undo anything in `up.sql`
ALTER TABLE submissions_testcases DROP COLUMN stdout;
ALTER TABLE submissions_testcases DROP COLUMN stderr;

ALTER TABLE submissions_testcases ADD COLUMN compile_output TEXT;
ALTER TABLE submissions_testcases ADD COLUMN answer TEXT;