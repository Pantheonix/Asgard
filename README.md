# Asgard
The **Pantheonix** backend is called **Asgard**, as the realm of gods from the northern mythology, and it consists in a cluster of microservices each performing a well-defined set of actions related to the domain context of such a programming educational platform:
- **Quetzalcoatl**, as the feathered serpent Maya god of wisdom, is the identity microservice responsible to challenge each user's identity claim and authorize access to restricted resources
- **Enki**, as the sumerian god of wisdom and handcrafting, is the problems microservice responsible to manage the access to the competitive programming problems archive
- **Hermes**, as the greek god of travel, is the tests microservice responsible to allow swift access to each problem tests archive
- **Anubis**, as the egyptian god which guides the souls to the underwold, is the submissions microservice responsible to guide the users submissions to their final judgement
- **Odin**, as the almighty northern god of Asgard, is the API Gateway microservice responsible to guard the access to the rest of backend services
## Technologies
## Functionalities
## Architectures
From a macro-point of view, Asgard encompasses a few cloud design patterns which rule the overall system behavior and those are the Mesh Service, the API Gateway and the Sidecar patterns:

1. **Mesh Service:**
   
   - **Definition:** A mesh service, often associated with service mesh architecture, is a microservice that is part of a larger network of services (containerized in our case). Service mesh is designed to manage communication between microservices, handling tasks such as service discovery, load balancing, and security.

   - **Key Features:**
     - **Service Discovery:** Automatically locates and registers services within the mesh.
     - **Load Balancing:** Distributes incoming traffic across multiple instances of a service to ensure optimal performance.
     - **Fault Tolerance:** Manages communication resilience by handling failures and retries.
     - **Security:** Provides features like encryption and authentication to secure communication between services.

2. **API Gateway:**

   - **Definition:** An API Gateway is a server that acts as an entry point for a set of microservices. It is responsible for handling requests, performing authentication, and routing them to the appropriate service.

   - **Key Features:**
     - **Request Routing:** Directs incoming requests to the appropriate microservice based on predefined rules.
     - **Authentication and Authorization:** Ensures that only authorized users or systems can access certain APIs.
     - **Rate Limiting:** Controls the rate of incoming requests to prevent abuse and ensure fair usage.
     - **Logging and Monitoring:** Provides tools for tracking and analyzing API usage for diagnostics and analytics.

3. **Sidecar:**

   - **Definition:** In the context of microservices, a sidecar is a separate, containerized process or service that runs alongside the main application. It provides additional functionalities without altering the main service's code.

   - **Key Features:**
     - **Service Mesh Integration:** Acts as a proxy to manage communication between microservices in a service mesh.
     - **Code Decoupling:** Enables the addition of functionalities (e.g., logging, security, or monitoring) without modifying the core application.
     - **Dynamic Configuration:** Allows for real-time updates and changes without restarting the main application.
     - **Isolation:** Provides a level of isolation for specific concerns, improving maintainability and scalability.

In modern distributed systems, such as Pantheonix's Asgard backend, these concepts work together to enhance scalability, maintainability, and manageability of microservices-based architectures.

And now, a few words about the **Domain-Driven Design** architecture which is used at the microservice level:
Domain-Driven Design (DDD) is a software development approach that focuses on understanding and modeling the problem domain as a central aspect of the software design process. Key features of DDD include:

1. **Bounded Contexts:** Defining explicit boundaries within which a particular model is applicable. This helps avoid confusion and conflicts in understanding terms or concepts that might have different meanings in different parts of the system.

2. **Aggregates:** Grouping related entities and value objects into aggregates, with one entity designated as the aggregate root. Aggregates ensure consistency and encapsulation of business rules.

3. **Entities and Value Objects:** Distinguishing between entities (objects with a distinct identity) and value objects (objects without an identity, defined only by their attributes). This helps in modeling the domain more accurately.

4. **Repositories:** Providing a mechanism to access and persist aggregates. Repositories abstract away the details of data storage and retrieval.

5. **Domain Events:** Capturing and representing significant changes or occurrences within the domain. Domain events help in maintaining consistency and can trigger further actions in the system.

6. **Services:** Encapsulating domain logic that doesn't naturally fit into entities or value objects. Services help in keeping the domain layer clean and focused on core business logic.

![](assets/Pantheon-Architecture.png)
Pantheonix Solution Architecture
