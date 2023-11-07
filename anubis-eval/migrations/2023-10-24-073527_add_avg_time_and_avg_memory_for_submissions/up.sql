-- Your SQL goes here
ALTER TABLE submissions ADD COLUMN avg_time real DEFAULT 0.0;
ALTER TABLE submissions ADD COLUMN avg_memory real DEFAULT 0.0;