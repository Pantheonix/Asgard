use diesel::{allow_tables_to_appear_in_same_query, table};

table! {
    submissions (id) {
        id -> Int4,
        user_id -> Int4,
        problem_id -> Int4,
        language -> Varchar,
        source_code -> Text,
        status -> Varchar,
        score -> Int4,
        created_at -> Timestamp,
    }
}

table! {
    submissions_testcases (token) {
        token -> Int4,
        submission_id -> Int4,
        status -> Varchar,
        time -> Numeric,
        memory -> Numeric,
        score -> Int4,
        answer -> Text,
        eval_message -> Nullable<Text>,
        compile_output -> Nullable<Text>,
    }
}

allow_tables_to_appear_in_same_query!(
    submissions,
    submissions_testcases,
);
