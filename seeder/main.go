package main

import "log"

func main() {
	// Load fixtures from fixtures.yaml
	fixtures, err := LoadFixtures("./fixtures.yaml")
	if err != nil {
		log.Fatalf("failed to load fixtures: %s", err)
	}

	// Create a new Pantheonix client
	client := NewPantheonixClient(fixtures)

	// Create a new seeder
	seeder := NewSeeder(client)

	// Seed problems
	if err := seeder.SeedProblems(); err != nil {
		log.Fatalf("failed to seed problems: %s", err)
	}
}
