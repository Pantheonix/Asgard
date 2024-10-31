#[cfg(test)]
pub mod tests {
    use cder::{Dict, StructLoader};
    use lazy_static::lazy_static;
    use serde::Deserialize;
    use std::fmt;
    use uuid::Uuid;

    #[derive(Debug, Clone, Deserialize)]
    pub struct User {
        pub id: Uuid,
        pub email: String,
        pub role: Vec<Role>,
    }

    pub enum UserProfile {
        Admin,
        Proposer,
        Ordinary,
    }

    impl User {
        pub fn get(user_profile: UserProfile) -> User {
            match user_profile {
                UserProfile::Admin => USERS.get("Admin").unwrap().to_owned(),
                UserProfile::Proposer => USERS.get("Proposer").unwrap().to_owned(),
                UserProfile::Ordinary => USERS.get("Ordinary").unwrap().to_owned(),
            }
        }
    }

    #[derive(Debug, Clone, Deserialize)]
    pub enum Role {
        Admin,
        Proposer,
    }

    impl fmt::Display for Role {
        fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
            match self {
                Role::Admin => write!(f, "Admin"),
                Role::Proposer => write!(f, "Proposer"),
            }
        }
    }

    lazy_static! {
        static ref USERS: StructLoader<User> = {
            let mut loader = StructLoader::<User>::new("users.yaml", "tests-setup/fixtures");
            loader.load(&Dict::<String>::new()).unwrap();

            loader
        };
    }
}
