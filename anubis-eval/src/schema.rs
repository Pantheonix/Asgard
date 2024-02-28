// @generated automatically by Diesel CLI.

diesel::table! {
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

diesel::table! {
    submissions (id) {
        id -> Text,
        user_id -> Text,
        problem_id -> Text,
        #[max_length = 255]
        language -> Varchar,
        source_code -> Text,
        #[max_length = 255]
        status -> Varchar,
        score -> Int4,
        created_at -> Timestamp,
        avg_time -> Nullable<Float4>,
        avg_memory -> Nullable<Float4>,
    }
}

diesel::table! {
    submissions_testcases (token) {
        token -> Text,
        submission_id -> Text,
        #[max_length = 255]
        status -> Varchar,
        time -> Float4,
        memory -> Float4,
        eval_message -> Nullable<Text>,
        stdout -> Nullable<Text>,
        stderr -> Nullable<Text>,
        testcase_id -> Int4,
        expected_score -> Int4,
        compile_output -> Nullable<Text>,
    }
}

diesel::table! {
    tests (id, problem_id) {
        id -> Int4,
        problem_id -> Text,
        score -> Int4,
        input_url -> Text,
        output_url -> Text,
    }
}

diesel::joinable!(submissions -> problems (problem_id));
diesel::joinable!(submissions_testcases -> submissions (submission_id));
diesel::joinable!(tests -> problems (problem_id));

diesel::allow_tables_to_appear_in_same_query!(problems, submissions, submissions_testcases, tests,);
