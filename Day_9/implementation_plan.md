# Implement Serilog and Correlation ID System

This plan outlines the steps to integrate Serilog and a Correlation ID middleware into the ASP.NET Core Web API project, satisfying all logging requirements and best practices.

## Proposed Changes

### 1. NuGet Packages Installation
Execute bulk installation of the required Serilog packages:
- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.MSSqlServer`
- `Serilog.Enrichers.Environment`
- `Serilog.Enrichers.Thread`

### 2. Middlewares

#### [NEW] `Middlewares/CorrelationIdMiddleware.cs`
- Check request headers for `X-Correlation-ID`.
- Generate a new GUID if not found.
- Add the ID to `HttpContext.Items["CorrelationId"]`.
- Enqueue adding the correlation ID to the response header `X-Correlation-ID`.
- Wrap the `next(context)` call within `LogContext.PushProperty("CorrelationId", correlationId)` to ensure the ID is included in all downstream logs for the current request.

#### [NEW] `Middlewares/GlobalExceptionMiddleware.cs`
- Global error handling wrapper over request pipeline.
- Catch all unhandled exceptions, log an error utilizing `ILogger<GlobalExceptionMiddleware>`, and return a standardized JSON error response. This ensures exceptions are centrally logged with the Correlation ID correctly appended.

### 3. Application Startup

#### [MODIFY] `Program.cs`
- Add `builder.Host.UseSerilog((context, services, configuration) => ...)` to set up the Serilog pipeline.
- Configure logging:
  - Enrich from `LogContext`, `Environment`, and `Thread`.
  - Sink to `Console` with a custom output template: `"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CorrelationId}{NewLine}{Exception}"`
  - Sink to `MSSQLServer` with connection string provided, `Logs` table, and `AutoCreateSqlTable = true`.
- Register `CorrelationIdMiddleware` and `GlobalExceptionMiddleware` at the beginning of the middleware pipeline to ensure all subsequent steps (including Controller invocations) have enriched logs.
- Add `builder.Services.AddHttpContextAccessor()` to support retrieving Context variables from DI layers if necessary.

### 4. Controller & Service Implementation (Examples)

#### [MODIFY] `Controllers/ProductsController.cs`
- Inject `ILogger<ProductsController>`.
- Add an informational log to `GetProducts` method.
- Add an error log inside `PostProduct` or an intentional exception point to demonstrate the Correlation ID and error handling.

#### [MODIFY] `Services/ProductsService.cs`
- Inject `ILogger<ProductsService>` and `IHttpContextAccessor`.
- Log an informational message to show that logs generated within the service layer inherit the Correlation ID automatically via `LogContext`.
- Add example logic showing how to manually retrieve the `CorrelationId` from `_httpContextAccessor.HttpContext.Items["CorrelationId"]` if needed for downstream API calls.

## Verification Plan

### Automated Tests
1. Send requests to the API and check if the `X-Correlation-ID` header is in the HTTP response.
2. Ensure we can send an existing `X-Correlation-ID` and see it echoed back.
3. Verify Serilog Console output shows the custom Correlation ID format.
4. Verify the `Logs` table is auto-created in MSSQL Server and contains rows with `CorrelationId`.
5. Trigger an error endpoint and verify `GlobalExceptionMiddleware` catches it and logs it.
