package main

import (
	"context"
	"fmt"
	"strconv"
	"time"

	dapr "github.com/dapr/go-sdk/client"
)

const (
	pubsubComponentName = "redis-pubsub"
	pubsubTopic         = "pending-submissions-topic"
)

func main() {
	// Create a new client for Dapr using the SDK
	client, err := dapr.NewClient()
	if err != nil {
		panic(err)
	}
	defer client.Close()

	// Publish events using Dapr pubsub
	for i := 1; i <= 10; i++ {
		submission := `{"submissionId":` + strconv.Itoa(i) + `}`

		err := client.PublishEvent(context.Background(), pubsubComponentName, pubsubTopic, []byte(submission))
		if err != nil {
			panic(err)
		}

		fmt.Println("Published data:", submission)

		time.Sleep(1000)
	}
}
