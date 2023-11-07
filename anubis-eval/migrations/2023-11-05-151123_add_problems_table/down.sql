-- This file should undo anything in `up.sql`
ALTER TABLE submissions DROP CONSTRAINT fk_submissions_problems;

DROP TABLE problems;