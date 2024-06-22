mod common;

#[tokio::test]
async fn test_example() {
    common::setup();
    assert_eq!(2 + 2, 4);
}