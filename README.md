# UseCaseCore

Base library for implementing **Use Cases**, **Result wrappers**, **Generic Dispatcher**, and **Domain Events** in a clean architecture style.  

This library is framework-agnostic and compatible with **.NET Standard 2.1**, making it usable in **.NET 6, .NET 7, .NET 8**, Minimal APIs, Controllers, or other .NET applications.

---

## Features

- **UseCaseBase<TRequest, TResponse>**: Abstract base class for all Use Cases, ensuring a consistent execution pattern.
- **ResultCase<T>**: Standardized result wrapper with support for:
  - Success/Failure
  - HTTP-like status codes (200 OK, 201 Created, 204 NoContent, 400 BadRequest, 404 NotFound, 500 ServerError, custom)
  - Optional messages and created resource locations
- **UseCaseDispatcher**: Generic, overridable dispatcher to execute Use Cases with hooks for:
  - Logging
  - Caching
  - Retry policies
  - Domain Event publishing
- **DomainEvent & IDomainEventDispatcher**: Abstract definitions for domain events and event dispatching, supporting decoupled, event-driven designs.

---

## Installation

Install from NuGet:
dotnet add package MiguelBrav.UseCaseCore

Or reference the DLL directly in your project.

You can consume check the nuget using this URL: "https://www.nuget.org/packages/UseCaseCore".