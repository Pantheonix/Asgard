FROM lukemathwalker/cargo-chef:0.1.67-rust-slim-buster AS chef
WORKDIR /app

FROM chef AS planner
COPY . .
RUN cargo chef prepare --recipe-path recipe.json

FROM chef as builder
LABEL stage=builder
ENV CARGO_TERM_COLOR always
RUN apt-get update && apt-get install -y libpq-dev libsqlite3-dev libmariadbclient-dev-compat pkg-config libssl1.1 libssl-dev && apt-get clean && rm -rf /var/lib/apt/lists/*
COPY --from=planner /app/recipe.json recipe.json
RUN cargo chef cook --release --recipe-path recipe.json
COPY . .
RUN cargo build --release --bin anubis-eval

FROM debian:buster-slim AS runtime
WORKDIR  /app
RUN apt-get update && apt-get install -y libpq-dev libsqlite3-dev libmariadbclient-dev-compat pkg-config libssl1.1 libssl-dev && apt-get clean && rm -rf /var/lib/apt/lists/*
COPY --from=builder /app/target/release/anubis-eval  /usr/local/bin
ENTRYPOINT ["/usr/local/bin/anubis-eval"]