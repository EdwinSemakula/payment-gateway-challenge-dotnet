## Tech Stack
- C#
- .NET 8
- ASP.NET Core Web API
- NUnit 

## Architecture:
Mainly follows the Layered Architecture pattern for simplicity with elements of the Clean Architecture pattern.
- API Layer: Contains the controllers that define the API endpoints and handle HTTP requests and responses.
  - Controllers: Handle incoming HTTP requests and return appropriate HTTP responses.
  - Program.cs configures the application including services, middleware and routing.

- Application Layer: Contains the business logic of the application.
  - Services: Implement the core business logic for handling payments and merchants.
  - Interfaces: Define contracts for services and external dependencies.

- Infrastructure Layer: Contains implementations for external dependencies.
  - Bank Client: Simulates interaction with an external bank service.
  - Data Storage: Uses static lists to store payment and merchant data.
  - Interfaces: Define contracts for external dependencies.

## Testing
- 93% code coverage by unit tests

## Assumptions
- Amount for the payment must be greater than 0
- Currency codes that are accepted are: GBP, EUR, USD
- The merchant will be given their own unique ID which they will use to identify themselves when making requests to the payment controller
- There can be multiple merchants that use the payment gateway, hence the need for the merchant controller
- The merchant is already registered in the system before they make a payment request


## Improvements
- Add integration & smoke tests
- Implement retry logic for when the bank client (simulator) returns with serice unavailable (use Polly) 
- Use in-memory database for storing payment and merchant details instead of a static list
- Implement authentication and authorization for the API endpoints for the payments endpoints e.g. JWT Bearer tokens
- Make use of Data Annotations for the majority of model validation in the payment request model
- There is a possibility that the gateway may need to support multiple banks in the future, in this case we can implement a adapter pattern to handle different banks