package main

func main() {
	// Load fixtures from fixtures.yaml
	fixtures := LoadFixtures("./fixtures.yaml")

	// Create a new Pantheonix client
	client := NewPantheonixClient(fixtures)

	// Create a new seeder
	seeder := NewSeeder(client)

	// Seed problems
	_ = seeder.SeedProblems()
}
