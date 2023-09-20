FROM rust:1.71.1-slim-buster AS build
LABEL stage=builder
ENV CARGO_TERM_COLOR always

# create empty project for caching dependencies
RUN USER=root cargo new --bin anubis-eval
WORKDIR /anubis-eval
COPY ./Cargo.lock ./
COPY ./Cargo.toml ./

# cache dependencies
RUN cargo install --path . --locked
COPY . .
RUN touch src/main.rs
RUN cargo install --path . --locked

FROM debian:buster-slim

COPY --from=build /usr/local/cargo/bin/anubis-eval /usr/local/bin/anubis-eval
CMD ["anubis-eval"]