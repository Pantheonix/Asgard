# Deploy Asgard on Kubernetes

This guide will walk you through deploying the Pantheonix's cluster of microservices aka Asgard on Kubernetes and explain
the architectural affinities of the cluster.

## Prerequisites

Make sure you have the following installed on your local machine:
- [Docker](https://docs.docker.com/get-docker/)
- [Kind (Kubernetes in Docker)](https://kind.sigs.k8s.io/docs/user/quick-start/)
- [Kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
- [Helm](https://helm.sh/docs/intro/install/)
- [Helmfile](https://helmfile.readthedocs.io/en/latest/#installation)

## Setup

1. Clone the repository and navigate to the `k8s` directory:

```bash
cd k8s
```

2. Create a Kubernetes cluster using the create-cluster.sh script:

```bash
cd kind
./create-cluster.sh
```

3. Deploy the Asgard helm chart and its ancillary operators using the provided helmfile:

```bash
cd ..
helmfile sync
```

4. Provide your own values.yaml file to override the default values of the Asgard helm chart by appending a `values`
attribute to the helmfile.yaml file like so:

```yaml
releases:
  - name: asgard
    chart: ./asgard
    values:
      - /path/to/your/values.yaml
```

5. Tear down the Kubernetes cluster using the delete-cluster.sh script:

```bash
cd kind
./delete-cluster.sh
```

## Architecture

The Asgard cluster consists of the following microservices:
- **Odin**: The API Gateway
- **Quetzalcoatl**: The Authentication Service
- **Enki**: The Problems Service
- **Hermes**: The Tests Service
- **Anubis**: The Submissions Service

Besides the microservices, the k8s cluster also includes the following operators:
- **Dapr**: the Distributed Application Runtime for microservices, including components for service invocation, state management, pub/sub, etc.
- **RabbitMQ**: the message broker for asynchronous communication between the microservices, associated with the `dapr` pub/sub component.
- **MongoDB**: the NoSQL database for storing the data of the Enki microservice.

### In depth topology depiction

The Asgard cluster is composed of the following k8s resources grouped by microservices scopes:

* **Odin**: 
    * _Service_: Exposes the API Gateway on ports 80 (HTTP) and 443 (HTTPS).
    * _Deployment_: Manages the Odin API Gateway pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Odin service.
    * _Ingress_: Manages external access to the Odin service, handling HTTP and HTTPS traffic.

* **Quetzalcoatl**:
    * _Service_: Exposes the Quetzalcoatl service on port 80.
    * _Deployment_: Manages the Quetzalcoatl API pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Quetzalcoatl service.
    * _Secret_: Stores sensitive information such as database passwords and JWT secret keys.

* **Enki**:
    * _Service_: Exposes the Enki service on port 80.
    * _Deployment_: Manages the Enki API pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Enki service.
    * _Secret_: Stores sensitive information such as database passwords.

* **Hermes**:
    * _Service_: Exposes the Hermes service on port 80.
    * _Deployment_: Manages the Hermes API pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Hermes service.
    * _Secret_: Stores sensitive information such as configuration data.

* **Anubis**:
    * _Service_: Exposes the Anubis service on port 80.
    * _Deployment_: Manages the Anubis API pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Anubis service.
    * _Secret_: Stores sensitive information such as database passwords.

* **Judge0**: The Third-Party Code Execution Service
    * _Service_: Exposes the Judge0 service on port 80.
    * _Deployment_: Manages the Judge0 API and worker pods, ensuring the desired number of replicas are running.
    * _ConfigMap_: Stores environment variables for the Judge0 service.
    * _Secret_: Stores sensitive information such as database passwords.

* **PostgreSQL**: Used by Anubis and Judge0
    * _Service_: Exposes the PostgreSQL database on port 5432.
    * _StatefulSet_: Manages the PostgreSQL pods, ensuring data persistence and ordered, graceful deployment and scaling.
    * _ConfigMap_: Stores environment variables for the PostgreSQL service.
    * _Secret_: Stores sensitive information such as database passwords.
    * _PersistentVolume_: Provides persistent storage for the PostgreSQL database.
    * _PersistentVolumeClaim_: Requests storage resources for the PostgreSQL database.
  
* **MongoDB**: Used by Enki
    * _Service_: Exposes the MongoDB database.
    * _StatefulSet_: Manages the MongoDB pods, ensuring data persistence and ordered, graceful deployment and scaling.
    * _Secret_: Stores sensitive information such as database passwords.
    * _PersistentVolume_: Provides persistent storage for the MongoDB database.
    * _PersistentVolumeClaim_: Requests storage resources for the MongoDB database.
  
* **Mssql**: Used by the Quetzalcoatl service
    * _Service_: Exposes the Mssql database.
    * _StatefulSet_: Manages the Mssql pods, ensuring data persistence and ordered, graceful deployment and scaling.
    * _Secret_: Stores sensitive information such as database passwords.
    * _PersistentVolume_: Provides persistent storage for the Mssql database.
    * _PersistentVolumeClaim_: Requests storage resources for the Mssql database.
  
* **Redis**: Used by the Dapr statestore
* **RabbitMQ**: Used by the Dapr pub/sub component
* **Zipkin**: Used for distributed tracing

### Replica Set and Desired State Mechanisms
Kubernetes ensures that the desired state of the cluster is maintained through mechanisms such as ReplicaSets and StatefulSets. 
These controllers manage the lifecycle of pods, ensuring that the specified number of replicas are running and healthy.  
* **ReplicaSet**: Ensures that a specified number of pod replicas are running at any given time. 
It is used by Deployments to manage stateless applications.
* **StatefulSet**: Ensures the deployment and scaling of a set of pods with unique, persistent identities and stable network identities.
It is used for stateful applications such as databases and caches.

### Autoscaling and Ever-Healthy Resources
Kubernetes supports autoscaling to dynamically adjust the number of pod replicas based on resource usage and demand. 
This ensures that applications can handle varying loads and remain healthy.  
Concepts:
* **Horizontal Pod Autoscaler (HPA)**: Automatically scales the number of pod replicas based on observed CPU utilization or other select metrics.
* **Vertical Pod Autoscaler (VPA)**: Automatically adjusts the resource requests and limits of pods based on observed usage.

### Load Balancing
Kubernetes services provide load balancing to distribute traffic across multiple pod replicas. This ensures high availability and reliability of applications.  
Concepts:
* **Service**: Acts as a load balancer, distributing incoming traffic to the appropriate pods based on their labels and selectors.

### Dapr Mesh service of Sidecars
Dapr (Distributed Application Runtime) provides a set of building blocks for microservices, including service invocation, 
state management, and pub/sub messaging. Each microservice in the Asgard cluster has a Dapr sidecar that handles these functionalities.  
Concepts:
* **Sidecar**: Runs alongside each microservice, providing capabilities such as service discovery, state management, and pub/sub messaging.

### Difference Between Deployments and StatefulSets
* **Deployments**: Used for stateless applications. They manage ReplicaSets to ensure that the desired number of pod replicas are running. 
Deployments provide features such as rolling updates and rollbacks.
* **StatefulSets**: Used for stateful applications. They manage the deployment and scaling of a set of pods with unique, persistent identities. 
StatefulSets ensure ordered, graceful deployment and scaling, and provide stable network identities and persistent storage.