# SystemX.Contracts

## Purpose
The public data shapes exchanged between services and clients: Request/response DTOs, `PagedRequest`/`PagedResponse<T>`, integration events, error codes. Plain records.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Contracts
```

## Usage

```csharp
using SystemX.Contracts.Pagination;

public sealed record ProductDto(Guid Id, string Name, decimal Price);
public sealed record GetProductsResponse(PagedResponse<ProductDto> Products);
```

## Documentation

- [Pagination](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Contracts/Pagination.md)
