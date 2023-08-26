use serde::Deserialize;

#[derive(Debug, Deserialize)]
pub struct LanguageConfig {
    pub name: String,
    pub main_file: String,
    pub compile_command: String,
    pub execute_command: String,
    pub pre_compilation_commands: Vec<String>,
}

impl LanguageConfig {
    pub fn new(
        name: String,
        main_file: String,
        compile_command: String,
        execute_command: String,
        pre_compilation_commands: Vec<String>,
    ) -> Self {
        Self {
            name,
            main_file,
            compile_command,
            execute_command,
            pre_compilation_commands,
        }
    }
}