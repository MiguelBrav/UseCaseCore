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

## Basic Usage (without Dispatcher)

This is the simplest way to use **UseCaseCore**.  
You directly execute the Use Case from your API (Minimal API or Controller).

### Define a Use Case

```csharp
public class GetCategoryByIdUseCase 
    : UseCaseBase<string, ResultCase<CategoryResponse>>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<ResultCase<CategoryResponse>> Execute(string id)
    {
        var category = await _repository.GetById(id);
        if (category == null)
            return ResultCase<CategoryResponse>.NotFound("Category not found");

        return ResultCase<CategoryResponse>.Ok(new CategoryResponse(category));
    }
}

// Example
app.MapGet("/categories/{id}", async (
    string id,
    GetCategoryByIdUseCase useCase) =>
{
    var result = await useCase.Execute(id);

    return result.StatusCode switch
    {
        200 => Results.Ok(result.Value),
        404 => Results.NotFound(result.Error),
        _   => Results.StatusCode(result.StatusCode)
    };
});
```
### Mapping ResultCase to IResult (Helper)

When using `ResultCase<T>` with Minimal APIs or Controller API´s, the HTTP mapping logic can be centralized
in a helper method to avoid repeating `switch` statements in every endpoint.

### Result Mapping Helper

```csharp
public static class ResultCaseExtensions
{
    public static IResult ToIResult<T>(this ResultCase<T> result)
    {
        return result.StatusCode switch
        {
            200 => Results.Ok(result.Value),
            201 => Results.Created(result.CreatedLocation ?? string.Empty, result.Value),
            204 => Results.NoContent(),
            400 => Results.BadRequest(result.Error),
            404 => Results.NotFound(result.Error),
            500 => Results.Problem(result.Error),
            _   => Results.StatusCode(result.StatusCode)
        };
    }
}

// Example
app.MapGet("/categories/{id}", async (
    string id,
    GetCategoryByIdUseCase useCase) =>
{
    var result = await useCase.Execute(id);
    return result.ToIResult();
});
```

## Advanced Usage (with Dispatcher Override – .NET 8 / IResult)

This approach uses a **Dispatcher override** to centralize cross-cutting concerns  
(logging, retries, metrics, domain events) while returning `IResult` directly.

### Use Case returning IResult (.NET 8)

```csharp
public class GetCategoryByIdUseCase 
    : UseCaseBase<string, IResult>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<IResult> Execute(string id)
    {
        var category = await _repository.GetById(id);
        if (category == null)
            return TypedResults.NotFound("Category not found");

        return TypedResults.Ok(new CategoryResponse(category));
    }
}

// Dispatcher override
public class Net8Dispatcher : UseCaseDispatcher
{
    public override async Task<TResponse> Dispatch<TRequest, TResponse>(
        UseCaseBase<TRequest, TResponse> useCase,
        TRequest request)
    {
        // Cross-cutting logic (logs, retry, metrics, etc.)
        Console.WriteLine($"Executing {useCase.GetType().Name}");

        return await useCase.Execute(request);
    }
}

// Example
app.MapGet("/categories/{id}", async (
    string id,
    GetCategoryByIdUseCase useCase,
    Net8Dispatcher dispatcher) =>
{
    // Dispatcher returns IResult directly
    return await dispatcher.Dispatch(useCase, id);
});
```

## Using an Aggregator (Optional Cross-Cutting Layer)

In some scenarios, you may want to **compose multiple Use Cases** or apply **additional cross-cutting concerns** (logging, caching, retries, transactions, metrics, etc.) **without modifying your Use Cases or Dispatcher**.

For this purpose, you can introduce an **Aggregator** layer.

---

## What is an Aggregator?

An **Aggregator** is a thin orchestration layer that:

- Coordinates one or more Use Cases
- Applies cross-cutting logic (logging, caching, retries, metrics)
- Delegates execution to the Dispatcher
- Keeps endpoints and controllers clean

This allows you to scale behavior **without increasing coupling**.

---

## Example: Aggregator with Dispatcher (Minimal API – .NET 8)

### Aggregator

```csharp
public class CategoryAggregator
{
    private readonly Net8Dispatcher _dispatcher;
    private readonly GetCategoryByIdUseCase _getById;

    public CategoryAggregator(
        Net8Dispatcher dispatcher,
        GetCategoryByIdUseCase getById)
    {
        _dispatcher = dispatcher;
        _getById = getById;
    }

    public async Task<IResult> GetById(string id)
    {
        // Cross-cutting logic (optional)
        Console.WriteLine("Aggregator: before execution");

        var result = await _dispatcher.Dispatch(_getById, id);

        Console.WriteLine("Aggregator: after execution");

        return result;
    }
}

// Example
app.MapGet("/categories/{id}", async (
    string id,
    CategoryAggregator aggregator) =>
{
    return await aggregator.GetById(id);
});
```

## Installation

Install from NuGet:
dotnet add package MiguelBrav.UseCaseCore

Or reference the DLL directly in your project.

You can consume check the nuget using this URL: "https://www.nuget.org/packages/UseCaseCore".
