-- This file should undo anything in `up.sql`
ALTER TABLE submissions_testcases ALTER COLUMN time TYPE decimal;
ALTER TABLE submissions_testcases ALTER COLUMN memory TYPE decimal;
