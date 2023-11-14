use diesel::{allow_tables_to_appear_in_same_query, joinable, table};
table! {
    problems (id) {
        id -> Text,
        name -> Text,
        proposer_id -> Text,
        is_published -> Bool,
        time -> Nullable<Float4>,
        stack_memory -> Nullable<Float4>,
        total_memory -> Nullable<Float4>,
    }
}

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
        stdout -> Nullable<Text>,
        stderr -> Nullable<Text>,
        token -> Text,
        submission_id -> Text,
        testcase_id -> Int4,
        expected_score -> Int4,
        compile_output -> Nullable<Text>,
    }
}

joinable!(submissions -> problems (problem_id));
joinable!(submissions_testcases -> submissions (submission_id));

allow_tables_to_appear_in_same_query!(problems, submissions, submissions_testcases,);
