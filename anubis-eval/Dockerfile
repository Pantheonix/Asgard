FROM rust:1.71.1-slim-buster AS build
LABEL stage=builder
ENV CARGO_TERM_COLOR always
RUN apt-get update && apt-get install -y libpq-dev pkg-config libssl1.1 libssl-dev && apt-get clean && rm -rf /var/lib/apt/lists/*

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