# specify the base image to  be used for the application, alpine or ubuntu
FROM golang:1.22.4-alpine AS build

# create a working directory inside the image
WORKDIR /app

# copy Go modules and dependencies to image
COPY go.mod ./
COPY go.sum ./

# download Go modules and dependencies
RUN go mod download

# copy directory files i.e all files ending with .go
COPY *.go ./

# compile application
RUN go build -o /cache-stub

FROM scratch

WORKDIR /

COPY --from=build /cache-stub /cache-stub

ENTRYPOINT ["/cache-stub"]