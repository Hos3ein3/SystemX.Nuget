# Pagination

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Pagination`

## What it is
Domain and application primitives for handling offset-based (`PageRequest`, `PagedList<T>`) and keyset/cursor-based (`CursorPageRequest`, `CursorPagedList<T>`) pagination.

## When to use it
- Use `PageRequest` and `PagedList<T>` for UI grids, tables, and search pages requiring page navigation and total counts.
- Use `CursorPageRequest` and `CursorPagedList<T>` for high-throughput infinite scrolling, event streams, or large datasets where offset pagination (`SKIP/OFFSET`) incurs severe database performance penalties.
- Do NOT reference database drivers directly; persistence layers materialize these types.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
1. Create a `PageRequest` using `PageRequest.Create(page, pageSize, sortBy, sortDescending)`.
2. Wrap results in a `PagedList<T>` using `PagedList<T>.Create(items, totalCount, request)`.
3. Project items using `.Map(item => new ItemDto(...))`.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Pagination;

namespace Example.SharedKernelPaginationDoc;

public static class PaginationSample
{
    public static void Run()
    {
        PageRequest request = PageRequest.Create(1, 10, "CreatedAt", sortDescending: true);
        List<string> items = ["Item1", "Item2", "Item3"];
        int totalCount = 42;

        PagedList<string> paged = PagedList<string>.Create(items, totalCount, request);
        Console.WriteLine($"Page {paged.Page} of {paged.TotalPages}, Total items: {paged.TotalCount}");

        PagedList<int> mappedLengths = paged.Map(x => x.Length);
        Console.WriteLine($"Mapped item length: {mappedLengths.Items[0]}");

        // Cursor / Keyset pagination
        CursorPageRequest cursorReq = new(Cursor: "eyJpZCI6MTIzfQ", PageSize: 20);
        CursorPagedList<string> cursorPaged = new(items, NextCursor: "eyJpZCI6MTI1fQ", HasNext: true);
        Console.WriteLine($"Next cursor: {cursorPaged.NextCursor}");
    }
}
```

## Configuration (if any)
| Constant | Default | Description |
|---|---|---|
| `PageRequest.DefaultPageSize` | `20` | Default number of items per page. |
| `PageRequest.MaxPageSize` | `200` | Safety limit to prevent memory exhaustion. |

## Related
- [Specifications](Specifications.md)
- [ResultPattern](ResultPattern.md)
