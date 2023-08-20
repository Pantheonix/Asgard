use cloudevents::binding::warp::filter;
use warp::Filter;

#[tokio::main]
async fn main() {
    // GET /_healthcheck => 200 OK
    let healthcheck = warp::path!("_healthcheck")
        .and(warp::get())
        .map(|| "Healthy!");

    // POST /evaluate => 200 OK and print body
    let evaluate = warp::path!("evaluate")
        .and(warp::post())
        .and(filter::to_event())
        .map(|event: cloudevents::Event| {
            let payload = event.data().unwrap();
            println!("Received event: {:?}", payload);
            warp::reply::with_status(payload.to_string(), warp::http::StatusCode::OK)
        });

    let routes = healthcheck.or(evaluate);
    println!("Server started...");

    warp::serve(routes).run(([127, 0, 0, 1], 5213)).await;
}
