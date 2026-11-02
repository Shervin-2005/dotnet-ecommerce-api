# ☕ Neo Coffee – E-commerce API

A feature-rich e-commerce REST API built with **C#**, **ASP.NET Core 10**, and **Clean Architecture**.

Neo Coffee is a team-based e-commerce project designed to provide essential online store functionality through a structured and maintainable backend architecture.

The backend and database are developed by me, while the frontend is developed separately using React.

## 🚀 Features

### Authentication & User Management
- User registration and authentication
- JWT-based authentication
- Access and refresh token management using HttpOnly cookies
- Role-based authorization
- User profile management

### Product Management
- Product, category, and brand management
- Product variants and specifications
- Product image storage using ArvanCloud Object Storage
- Product search, filtering, sorting, and pagination

### Shopping & Orders
- Shopping cart management
- Order creation and management
- Order status tracking
- Automatic cancellation of pending orders after 30 minutes
- Optimistic concurrency control to handle simultaneous payment and cancellation operations

### Discounts & Reviews
- Discount code management
- Discount validation and expiration handling
- Product reviews

### Real-Time Support Chat
- Real-time customer support using SignalR
- Customer and admin conversations
- Conversation assignment and management

### Background Processing
- Background job processing using Hangfire
- Automatic cancellation of expired pending orders

### Logging & Observability
- Structured logging with Serilog
- Application monitoring and instrumentation with OpenTelemetry

## 🛠️ Tech Stack

| Category | Technologies |
|---|---|
| Language | C# |
| Framework | ASP.NET Core 10 |
| Architecture | Clean Architecture |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Authentication | JWT |
| Real-Time Communication | SignalR |
| Background Jobs | Hangfire |
| Validation | FluentValidation |
| Object Mapping | AutoMapper |
| Logging | Serilog |
| Observability | OpenTelemetry |
| Object Storage | ArvanCloud S3 |
| API Documentation | Swagger / OpenAPI |

## 🏛️ Architecture

The project follows Clean Architecture principles to separate business logic from infrastructure concerns.

The solution is organized into four main layers:

- **Domain:** Contains core entities, business rules, and domain logic.
- **Application:** Contains application services, business use cases, interfaces, and validation.
- **Infrastructure:** Implements persistence, repositories, external services, and infrastructure concerns.
- **API:** Exposes RESTful endpoints and handles HTTP requests.

This separation helps keep the business logic independent, maintainable, and easier to test.

## 📁 Project Structure

```text
dotnet-ecommerce-api/
│
├── Domain/
│
├── Application/
│
├── Infrastructure/
│
├── dotnet-ecommerce-api/
│   └── API
│
└── dotnet-ecommerce-api.slnx
```

## ⚙️ Getting Started

### Prerequisites

Make sure you have the following installed:

- .NET 10 SDK
- PostgreSQL
- Git

### Clone the Repository

```bash
git clone https://github.com/Shervin-2005/dotnet-ecommerce-api.git

cd dotnet-ecommerce-api
```

### Configuration

Configure the required application settings before running the project.

Sensitive values such as database credentials, JWT settings, and storage credentials should be configured using **User Secrets** or environment variables.

Do not commit passwords, secret keys, or other sensitive configuration values to source control.

### Database Setup

After configuring the database connection, apply the Entity Framework Core migrations to create the database schema.

### Run the Application

Open the solution in your preferred IDE or build and run the API project using the .NET CLI.

```bash
dotnet build
```

## 👨‍💻 Development

This project is actively developed and improved as part of an ongoing team project.

The main focus is on backend development, maintainable architecture, database design, and implementing practical software engineering concepts.

## 📫 Contact

**Developer:** Shervin Shams

- GitHub: [Shervin-2005](https://github.com/Shervin-2005)
- Telegram: [@Shervin_2005](https://t.me/Shervin_2005)
- LinkedIn: [Shervin Shams](https://www.linkedin.com/in/shervin-shams-3a9574375/)
