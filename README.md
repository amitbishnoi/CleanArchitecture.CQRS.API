# 🧩 Clean Architecture CQRS API

This project is a sample implementation of **Clean Architecture** with **CQRS** using **ASP.NET Core 8 Web API**.  
It demonstrates how to structure enterprise-grade applications with separation of concerns, maintainability, and scalability.

---

## 🚀 Features

- ASP.NET Core 8 Web API  
- Clean Architecture pattern  
- CQRS (Command Query Responsibility Segregation)  
- MediatR for request handling  
- Entity Framework Core (Code First)  
- Dependency Injection  
- Validation and Exception Handling  
- Repository & Unit of Work pattern

---

## 🏗️ Project Structure

src/
├── Application/ # Business logic (CQRS commands & queries)
│ ├── Commands/
│ ├── Queries/
│ ├── Interfaces/
│ └── DTOs/
│
├── Domain/ # Entities and domain models
│ └── Entities/
│
├── Infrastructure/ # Data access and persistence (EF Core)
│ ├── Context/
│ └── Repositories/
│
└── API/ # ASP.NET Core Web API (Controllers, Startup)
├── Controllers/
└── Program.cs
---

## 🧰 Technologies Used

- .NET 8  
- C# 12  
- Entity Framework Core  
- MediatR  
- FluentValidation  
- Swagger / OpenAPI  
- SQL Server

---

🧠 About CQRS

CQRS (Command Query Responsibility Segregation) divides the application logic into:

Commands → Write operations (Create, Update, Delete)
Queries → Read operations (Get, Search)

This pattern improves scalability, separation of concerns, and testability.

---
💬 Author

👨‍💻 Amit Bishnoi