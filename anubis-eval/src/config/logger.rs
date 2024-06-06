use log4rs::append::console::ConsoleAppender;
use log4rs::append::file::FileAppender;
use log4rs::config::{Appender, Logger, Root};
use log4rs::encode::pattern::PatternEncoder;
use rocket::log::private::LevelFilter;

pub fn init_logger() {
    match log4rs::init_file("log4rs.yaml", Default::default()) {
        Ok(_) => {
            println!("Logger initialized using log4rs.yaml");
        }
        Err(err) => {
            println!(
                "Failed to initialize logger, using default configuration due to error: {:?}",
                err
            );
            println!("Logger initialized using default configuration");

            let file_appender = FileAppender::builder()
                .encoder(Box::new(PatternEncoder::new(
                    "{d(%Y-%m-%d %H:%M:%S%.3f)} {h({l})} {M} - {m}{n}",
                )))
                .build("logs/anubis.logs")
                .unwrap();

            let console_appender = ConsoleAppender::builder()
                .encoder(Box::new(PatternEncoder::new(
                    "{d(%Y-%m-%d %H:%M:%S%.3f)} {h({l})} {M} - {m}{n}",
                )))
                .build();

            let log_config = log4rs::config::Config::builder()
                .appender(Appender::builder().build("file_appender", Box::new(file_appender)))
                .appender(Appender::builder().build("console_appender", Box::new(console_appender)))
                .logger(
                    Logger::builder()
                        .appender("file_appender")
                        .appender("console_appender")
                        .additive(false)
                        .build("anubis", LevelFilter::Info),
                )
                .build(
                    Root::builder()
                        .appender("file_appender")
                        .appender("console_appender")
                        .build(LevelFilter::Info),
                )
                .unwrap();

            log4rs::init_config(log_config).unwrap();
        }
    }
}
