use diesel::{allow_tables_to_appear_in_same_query, joinable, table};
table! {
    submissions (id) {
        language -> Varchar,
        source_code -> Text,
        status -> Varchar,
        score -> Int4,
        created_at -> Timestamp,
        id -> Text,
        user_id -> Text,
        problem_id -> Text,
        avg_time -> Nullable<Float4>,
        avg_memory -> Nullable<Float4>,
    }
}

table! {
    submissions_testcases (token) {
        status -> Varchar,
        time -> Float4,
        memory -> Float4,
        eval_message -> Nullable<Text>,
        compile_output -> Nullable<Text>,
        stdout -> Nullable<Text>,
        stderr -> Nullable<Text>,
        token -> Text,
        submission_id -> Text,
        testcase_id -> Int4,
        expected_score -> Int4,
    }
}

joinable!(submissions_testcases -> submissions (submission_id));

allow_tables_to_appear_in_same_query!(submissions, submissions_testcases,);