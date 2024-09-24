-- This file should undo anything in `up.sql`
ALTER TABLE submissions DROP COLUMN avg_time;
ALTER TABLE submissions DROP COLUMN avg_memory;