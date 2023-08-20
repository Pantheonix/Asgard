package api

import (
	"context"
	"fmt"
	"thor-submissions/domain"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

func Submit(c *gin.Context, daprClient *dapr.Client) {
	var submission domain.Submission
	if err := c.ShouldBindJSON(&submission); err != nil {
		c.JSON(400, gin.H{"error": err.Error()})
		return
	}

	submission.SubmissionId = uuid.New()

	if err := (*daprClient).PublishEvent(
		context.Background(),
		domain.PUBSUB_COMPONENT_NAME,
		domain.PUBSUB_TOPIC,
		[]byte(submission.String()),
	); err != nil {
		c.JSON(500, gin.H{"error": err.Error()})
		return
	}

	fmt.Println("Published data:", submission)
	c.JSON(200, submission)
}
