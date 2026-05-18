TechMove — Enterprise Logistics System
Overview

TechMove is an ASP.NET Core MVC enterprise logistics management prototype developed for EAPD7111.

The system allows administrators to:

Manage Clients
Manage Contracts
Create and manage Service Requests
Upload and download signed agreement PDFs
Convert USD values to ZAR using an external API
Enforce workflow validation rules using the State Pattern

The application was developed as a monolithic enterprise prototype using:

ASP.NET Core MVC
Entity Framework Core
SQL Server
Tailwind CSS
xUnit Testing
GitHub Actions CI Pipeline
Features
1. Client Management

Users can:

Create clients
Edit client details
View client information
Delete clients

Client fields:

Name
Contact Details
Region
2. Contract Management

Users can:

Create contracts linked to clients
Upload signed agreement PDFs
Download uploaded agreements
Edit contracts
Delete contracts

Contract fields:

Start Date
End Date
Status
Service Level
Agreement PDF

Workflow rules are enforced through contract states.

3. Service Request Management

Users can:

Create service requests
Edit service requests
Delete service requests
View request details

Service Request fields:

Description
Cost USD
Cost ZAR
Status
Linked Contract

The ZAR amount is automatically calculated using a live external exchange rate API.

Design Pattern Implementation
State Pattern

The State Pattern was implemented for Contract workflow validation.

Different contract states control whether Service Requests may be created.

Implemented states:

ActiveContractState
PendingContractState
ExpiredContractState
OnHoldContractState

Example:

Active contracts allow Service Requests.
Expired and On Hold contracts block Service Requests.

This separates workflow validation logic from controllers and demonstrates enterprise design principles.

External API Integration

The system integrates with a live exchange rate API:

https://open.er-api.com/v6/latest/USD

Purpose:

Convert USD amounts to ZAR automatically.

Implementation details:

HttpClientFactory
Async/Await
JSON Deserialization
Error handling with fallback logic
Graceful API failure handling using try/catch

If the API fails, the application falls back to a predefined conversion rate.

File Handling

The application supports:

PDF upload
File storage on server
Downloading uploaded agreements
PDF validation

Uploaded agreements are stored in:

wwwroot/uploads/

Database Architecture

Entity Framework Core with SQL Server was used.

Entities:

Client
Contract
ServiceRequest

Relationships:

One Client → Many Contracts
One Contract → Many Service Requests

Migrations were created using EF Core.

Connection strings are stored in:

appsettings.json

Unit Testing

xUnit was used for testing.

Test coverage includes:

Currency conversion calculations
File validation
Edge cases
Invalid inputs

The project demonstrates:

Test-Driven Development principles
Automated testing
Green test execution
CI/CD Pipeline

GitHub Actions was configured for continuous integration.

Pipeline tasks:

Restore dependencies
Build solution
Run unit tests automatically

Workflow file:

.github/workflows/ci.yml

Technologies Used
ASP.NET Core MVC
C#
Entity Framework Core
SQL Server
Tailwind CSS
xUnit
GitHub Actions
REST API Integration
How to Run the Project
1. Clone Repository
git clone https://github.com/andrewmpho47/TechMove.git
2. Open Project

Open the solution in:

Visual Studio OR
VS Code
3. Restore Packages
dotnet restore
4. Apply Migrations
dotnet ef database update
5. Run Application
dotnet run
GitHub Repository

Repository:

https://github.com/andrewmpho47/TechMove

Author

Mpho Andrew Ngwenya

EAPD7111 — Enterprise Application Development
