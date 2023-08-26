use std::collections::HashMap;

use serde::Deserialize;

use crate::domain::submission::ProgrammingLanguage;

use super::language_config::LanguageConfig;

#[derive(Debug, Deserialize)]
pub struct Config {
    pub programming_languages: HashMap<ProgrammingLanguage, LanguageConfig>,
}

impl Config {
    pub fn new(config_file_path: String) -> Self {
        let config_file_content = std::fs::read_to_string(config_file_path).unwrap();
        let config: Config = toml::from_str(&config_file_content).unwrap();
        config
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_config_new() {
        let config = Config::new(String::from("config.toml"));

        assert_eq!(config.programming_languages.len(), 7);

        let csharp = config
            .programming_languages
            .get(&ProgrammingLanguage::CSharp)
            .unwrap();

        assert_eq!(csharp.name, String::from(".NET Core (AOT)"));
        assert_eq!(csharp.main_file, String::from("Program.cs"));
        assert_eq!(
            csharp.compile_command,
            String::from("dotnet publish -c Release -r net7.0")
        );
        assert_eq!(
            csharp.execute_command,
            String::from("dotnet <published-directory>/Main.dll")
        );

        let csharp_pre_compilation_commands = vec![
            "touch Program.csproj",
            "echo '<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net7.0</TargetFramework><PublishTrimmed>true</PublishTrimmed></PropertyGroup></Project>' > Program.csproj",
        ];
        assert_eq!(
            csharp.pre_compilation_commands,
            csharp_pre_compilation_commands
        );
    }
}
