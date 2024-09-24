-- This file should undo anything in `up.sql`
ALTER TABLE submissions_testcases ADD COLUMN score integer NOT NULL DEFAULT 0; 