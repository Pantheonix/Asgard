use rocket::form::{Errors, FromFormField};

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
