-- Your SQL goes here
ALTER TABLE tests
DROP CONSTRAINT tests_pkey;

ALTER TABLE tests
ADD PRIMARY KEY (id, problem_id);
