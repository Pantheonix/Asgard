package main

import (
	"thor-submissions/api"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/gin-gonic/gin"
)

func main() {
	daprClient, err := dapr.NewClient()
	if err != nil {
		panic(err)
	}
	ginEngine := gin.Default()

	server := api.NewServer(ginEngine, &daprClient)
	server.RegisterRoutes()
	defer server.Stop()

	if err := server.Start(":5214"); err != nil {
		panic(err)
	}
}
