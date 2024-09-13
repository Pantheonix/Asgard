package main

import "fmt"

func main() {
	// Load fixtures from fixtures.yaml
	fixtures := LoadFixtures("./fixtures.yaml")

	// Access the fixtures
	fmt.Println(fixtures)
}
