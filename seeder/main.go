package main

import "log"

func main() {
	// Load fixtures from fixtures.yaml
	fixtures := LoadFixtures("./fixtures.yaml")

	// Create a new Pantheonix client
	client := NewPantheonixClient(fixtures)

	// Login with a user
	admin := fixtures.Users.Data[0]
	token, err := client.Login(admin)

	if err != nil {
		log.Fatalf("Failed to login: %s", err)
	}

	log.Printf("Successfully logged in with user %s", admin.Email)
	log.Printf("Access token: %s", token.AccessToken)
}
