-- This file should undo anything in `up.sql`
ALTER TABLE tests
DROP CONSTRAINT tests_pkey;

ALTER TABLE tests
ADD PRIMARY KEY (id);