use cloudevents::binding::warp::filter;
use lib::api::evaluate::evaluate;
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
        .and_then(evaluate);

    let routes = healthcheck.or(evaluate);
    println!("Server started...");

    warp::serve(routes).run(([127, 0, 0, 1], 5213)).await;
}
