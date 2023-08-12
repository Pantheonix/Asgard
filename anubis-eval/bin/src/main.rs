use serde::{Deserialize, Serialize};
use warp::Filter;

#[derive(Serialize, Deserialize, Debug)]
struct PubSubEndpoint {
    #[serde(rename = "pubsubName")]
    pubsub_name: String,
    #[serde(rename = "topic")]
    topic_name: String,
    #[serde(rename = "route")]
    route_name: String,
}

#[tokio::main]
async fn main() {
    // GET /_healthcheck => 200 OK
    let healthcheck = warp::path!("_healthcheck")
        .and(warp::get())
        .map(|| "Healthy!");

    // POST /evaluate => 200 OK and print body
    let evaluate = warp::path!("evaluate")
        .and(warp::post())
        .and(warp::body::json())
        .map(|body: serde_json::Value| {
            println!("Received body: {:?}", body);
            warp::reply::json(&body)
        });

    let routes = healthcheck.or(evaluate);
    println!("Server started...");

    warp::serve(routes).run(([127, 0, 0, 1], 5213)).await;
}
