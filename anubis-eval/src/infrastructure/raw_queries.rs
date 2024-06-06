pub static GET_HIGHEST_SCORE_SUBMISSIONS_PER_USER: &str = r"
SELECT s.*
FROM submissions s
WHERE (
  s.user_id = $1
  OR s.problem_id IN (
    SELECT p.id
    FROM problems p
    WHERE p.is_published = TRUE
      OR p.proposer_id = $1
  )
)
  AND s.user_id = $2
  AND s.score = (
    SELECT MAX(score)
    FROM submissions
    WHERE user_id = s.user_id AND problem_id = s.problem_id
)
  AND s.created_at = (
    SELECT MIN(created_at)
    FROM submissions
    WHERE user_id = s.user_id AND problem_id = s.problem_id AND score = s.score
)";

pub static GET_HIGHEST_SCORE_SUBMISSIONS_PER_USER_AND_PROBLEM: &str = r"
SELECT s.*
FROM submissions s
WHERE (
  s.user_id = $1
  OR s.problem_id IN (
    SELECT p.id
    FROM problems p
    WHERE p.is_published = TRUE
      OR p.proposer_id = $1
  )
)
  AND s.user_id = $2
  AND s.problem_id = $3
  AND s.score = (
    SELECT MAX(score)
    FROM submissions
    WHERE user_id = s.user_id AND problem_id = s.problem_id
)
  AND s.created_at = (
    SELECT MIN(created_at)
    FROM submissions
    WHERE user_id = s.user_id AND problem_id = s.problem_id AND score = s.score
)";
