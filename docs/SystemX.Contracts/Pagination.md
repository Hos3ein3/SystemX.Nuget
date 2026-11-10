# Pagination Contracts

**Package:** `SystemX.Contracts`  
**Namespace:** `SystemX.Contracts.Pagination`

## What it is
Public, pure data transfer objects (DTOs) for exchanging paginated requests and responses across service boundaries, HTTP APIs, and message brokers.

## When to use it
- Use `PagedRequest` and `PagedResponse<T>` as input and output models in controllers, Minimal APIs, and client SDKs.
- Use `CursorPagedResponse<T>` for APIs serving cursor/keyset paginated feeds.
- Do NOT include domain logic or entity types in these DTOs.

## Installation
`dotnet add package SystemX.Contracts`

## How to use it
Map between `SystemX.SharedKernel.Pagination.PagedList<T>` in your application layer and `SystemX.Contracts.Pagination.PagedResponse<TDto>` for API responses.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using SystemX.Contracts.Pagination;
using SystemX.SharedKernel.Pagination;

namespace Example.ContractsPaginationDoc;

public sealed record ProductDto(Guid Id, string Name, decimal Price);

public static class PaginationMappingSample
{
    public static PagedResponse<ProductDto> MapToContract(PagedList<ProductDto> domainPaged)
    {
        return new PagedResponse<ProductDto>(
            Items: domainPaged.Items,
            Page: domainPaged.Page,
            PageSize: domainPaged.PageSize,
            TotalCount: domainPaged.TotalCount,
            TotalPages: domainPaged.TotalPages,
            HasPreviousPage: domainPaged.HasPreviousPage,
            HasNextPage: domainPaged.HasNextPage);
    }

    public static void Run()
    {
        PagedRequest request = new(Page: 1, PageSize: 10, SortBy: "Name", SortDescending: false);
        List<ProductDto> dtos = [new ProductDto(Guid.NewGuid(), "Laptop", 999m)];
        PagedList<ProductDto> domainList = new(dtos, 1, 10, 1);

        PagedResponse<ProductDto> response = MapToContract(domainList);
        Console.WriteLine($"Response Page: {response.Page}, Items: {response.Items.Count}");
    }
}
```

## Related
- [Pagination (SharedKernel)](../SystemX.SharedKernel/Pagination.md)
