use crate::domain::submission::{Language, SubmissionStatus};
use chrono::Utc;
use rocket::form::{Errors, FromFormField};
use rocket::FromForm;
use uuid::Uuid;

// FSP stands for Filter, Sort, Paginate
#[derive(Debug, PartialEq, FromForm)]
pub struct FspSubmissionDto {
    pub user_id: Option<Uuids>,
    pub problem_id: Option<Uuids>,
    pub language: Option<Languages>,
    pub status: Option<SubmissionStatuses>,
    pub lt_score: Option<usize>,
    pub gt_score: Option<usize>,
    pub lt_avg_time: Option<f32>,
    pub gt_avg_time: Option<f32>,
    pub lt_avg_memory: Option<f32>,
    pub gt_avg_memory: Option<f32>,
    pub start_date: Option<DateTime>,
    pub end_date: Option<DateTime>,
    pub sort_by: Option<SortDiscriminant>,
    pub page: Option<i64>,
    pub per_page: Option<i64>,
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct DateTime {
    pub date_time: chrono::DateTime<Utc>,
}

impl<'v> FromFormField<'v> for DateTime {
    fn from_value(field: rocket::form::ValueField<'v>) -> rocket::form::Result<'v, Self> {
        let date_time =
            chrono::DateTime::<Utc>::from_timestamp(field.value.parse::<i64>().unwrap_or(0), 0)
                .unwrap();

        Ok(DateTime {
            date_time: date_time.into(),
        })
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct Uuids {
    pub uuids: Vec<Uuid>,
}

impl<'v> FromFormField<'v> for Uuids {
    fn from_value(field: rocket::form::ValueField<'v>) -> rocket::form::Result<'v, Self> {
        let uuids = field
            .value
            .split(',')
            .map(|s| Uuid::parse_str(s).unwrap_or(Uuid::nil()))
            .collect();

        Ok(Uuids { uuids })
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct SubmissionStatuses {
    pub statuses: Vec<SubmissionStatus>,
}

impl<'v> FromFormField<'v> for SubmissionStatuses {
    fn from_value(field: rocket::form::ValueField<'v>) -> rocket::form::Result<'v, Self> {
        let statuses = field
            .value
            .split(',')
            .map(|s| match s {
                "evaluating" => SubmissionStatus::Evaluating,
                "accepted" => SubmissionStatus::Accepted,
                "rejected" => SubmissionStatus::Rejected,
                "internal_error" => SubmissionStatus::InternalError,
                _ => SubmissionStatus::Unknown,
            })
            .collect();

        Ok(SubmissionStatuses { statuses })
    }
}

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct Languages {
    pub languages: Vec<Language>,
}

impl<'v> FromFormField<'v> for Languages {
    fn from_value(field: rocket::form::ValueField<'v>) -> rocket::form::Result<'v, Self> {
        let languages = field
            .value
            .split(',')
            .map(|s| match s {
                "c" => Language::C,
                "cpp" => Language::Cpp,
                "java" => Language::Java,
                "lua" => Language::Lua,
                "py" => Language::Python,
                "rs" => Language::Rust,
                "go" => Language::Go,
                "cs" => Language::CSharp,
                "ml" => Language::OCaml,
                "js" => Language::Javascript,
                "kt" => Language::Kotlin,
                "hs" => Language::Haskell,
                _ => Language::Unknown,
            })
            .collect();

        Ok(Languages { languages })
    }
}

#[derive(Debug, PartialEq)]
pub enum SortDiscriminant {
    ScoreAsc,
    ScoreDesc,
    CreatedAtAsc,
    CreatedAtDesc,
    AvgTimeAsc,
    AvgTimeDesc,
    AvgMemoryAsc,
    AvgMemoryDesc,
}

impl<'v> FromFormField<'v> for SortDiscriminant {
    fn from_value(field: rocket::form::ValueField<'v>) -> rocket::form::Result<'v, Self> {
        match field.value {
            "score_asc" => Ok(SortDiscriminant::ScoreAsc),
            "score_desc" => Ok(SortDiscriminant::ScoreDesc),
            "created_at_asc" => Ok(SortDiscriminant::CreatedAtAsc),
            "created_at_desc" => Ok(SortDiscriminant::CreatedAtDesc),
            "avg_time_asc" => Ok(SortDiscriminant::AvgTimeAsc),
            "avg_time_desc" => Ok(SortDiscriminant::AvgTimeDesc),
            "avg_memory_asc" => Ok(SortDiscriminant::AvgMemoryAsc),
            "avg_memory_desc" => Ok(SortDiscriminant::AvgMemoryDesc),
            _ => Err(Errors::from(rocket::form::Error::validation(
                "Invalid sort discriminant",
            ))),
        }
    }
}
