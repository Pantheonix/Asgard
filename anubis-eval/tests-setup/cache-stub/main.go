package main

import (
	"github.com/gin-gonic/gin"
	"github.com/redis/go-redis/v9"
	"os"
)

type State struct {
	Key   string `json:"key"`
	Value string `json:"value"`
}

func main() {
	// read the redis dsn from the environment variable REDIS_DSN
	redisDSN := os.Getenv("REDIS_DSN")

	if redisDSN == "" {
		redisDSN = "redis://localhost:6379"
	}

	// create a new redis client
	opt, err := redis.ParseURL(redisDSN)
	if err != nil {
		panic(err)
	}

	client := redis.NewClient(opt)

	r := gin.Default()
	r.GET("/healthy", func(ctx *gin.Context) {
		// check if the redis server is up
		_, err := client.Ping(ctx).Result()
		if err != nil {
			ctx.JSON(500, gin.H{
				"status": "unhealthy",
				"error":  err.Error(),
			})
		} else {
			ctx.JSON(200, gin.H{
				"status": "healthy",
			})
		}
	})

	r.GET("/v1.0/state/statestore/:key", func(ctx *gin.Context) {
		key := ctx.Param("key")
		val, err := client.Get(ctx, key).Result()
		if err != nil {
			ctx.JSON(404, gin.H{
				"error": err.Error(),
			})
		} else {
			ctx.JSON(200, val)
		}
	})

	r.POST("/v1.0/state/statestore", func(ctx *gin.Context) {
		var state []State
		if err := ctx.BindJSON(&state); err != nil {
			ctx.JSON(400, gin.H{
				"error": err.Error(),
			})
			return
		}

		for _, s := range state {
			if err := client.Set(ctx, s.Key, s.Value, 0).Err(); err != nil {
				ctx.JSON(500, gin.H{
					"error": err.Error(),
				})
			}
		}

		ctx.JSON(200, gin.H{
			"status": "success",
		})
	})

	if err := r.Run(); err != nil {
		return
	}
}
