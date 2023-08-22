package api

import (
	"fmt"
	app "thor-submissions/application"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/gin-gonic/gin"
)

type Server struct {
	ginEngine  *gin.Engine
	daprClient *dapr.Client
}

func NewServer(ginEngine *gin.Engine, daprClient *dapr.Client) *Server {
	return &Server{
		ginEngine:  ginEngine,
		daprClient: daprClient,
	}
}

func (s *Server) RegisterRoutes() {
	s.ginEngine.POST("/submit", func(c *gin.Context) {
		app.Submit(c, s.daprClient)
	})

	s.ginEngine.POST("/receive", func(c *gin.Context) {
		app.Receive(c, s.daprClient)
	})
}

func (s *Server) Start(port string) error {
	fmt.Printf("Starting server on port %s\n", port)
	return s.ginEngine.Run(port)
}

func (s *Server) Stop() {
	(*s.daprClient).Close()
}
