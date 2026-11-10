# Repositories

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Repositories`

## What it is
Generic repository implementations (`EfRepository<TEntity, TId>` and `EfReadRepository<TEntity, TId>`) providing specification evaluation, safe ID lookup, change tracking coordination, and pagination.

## When to use it
- Inject `IRepository<TEntity, TId>` for aggregate mutation (Add, Update, Remove).
- Inject `IReadRepository<TEntity, TId>` when read-only queries with projection or pagination are needed.
- Use `ISpecification<T>` and `ISpecification<T, TResult>` to encapsulate query logic.
- Do NOT expose `IQueryable<T>` from repositories; keep queries encapsulated behind specifications.

## The Second-Context Rule
When an application registers multiple `SystemXDbContext` types (e.g. `AppDbContext` and `ReportingDbContext`), only the primary context registers generic `IRepository<,>` and `IUnitOfWork` in the DI container (controlled by `RegisterRepositories = true`). Secondary contexts must configure `RegisterRepositories = false` and resolve their specific context or custom repository implementations directly.

## Navigation Includes
- **Simple Property Chains:** Use strongly-typed lambda expressions (`Include(x => x.Customer.Address)`). These are parsed by `IncludePathBuilder` into dot-separated paths.
- **Collection Navigations:** For complex collection navigation paths or `ThenInclude` over collections, use string-based navigation paths (`Include("Orders.LineItems")`).

## Sample code

### Querying with Specification and Paging

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Pagination;
using SystemX.SharedKernel.Specifications;

namespace ApplicationSample;

public sealed record ProductId(Guid Value) : GuidId(Value), IGuidIdFactory<ProductId>
{
    public static ProductId Create(Guid value) => new(value);
}

public class Product : AggregateRoot<ProductId>
{
    public Product(ProductId id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }
}

public sealed class ProductsByMinPriceSpec : Specification<Product>
{
    public ProductsByMinPriceSpec(decimal minPrice)
    {
        Where(p => p.Price >= minPrice);
        OrderBy(p => p.Name);
    }
}

public sealed class CatalogSearchService
{
    private readonly IReadRepository<Product, ProductId> _readRepository;

    public CatalogSearchService(IReadRepository<Product, ProductId> readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PagedList<Product>> SearchProductsAsync(
        decimal minPrice,
        PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var spec = new ProductsByMinPriceSpec(minPrice);
        return await _readRepository.ListPagedAsync(spec, pageRequest, cancellationToken);
    }
}
```

### Mutating Entities via `IRepository`

```csharp
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;

namespace ApplicationSample;

public sealed class CreateProductHandler
{
    private readonly IRepository<Product, ProductId> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(IRepository<Product, ProductId> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(ProductId id, string name, decimal price, CancellationToken cancellationToken)
    {
        var product = new Product(id, name, price);
        _repository.Add(product);

        // Repositories never call SaveChanges; the Unit of Work commits the transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `RegisterRepositories` | `true` | When true, registers `IRepository<,>` and `IReadRepository<,>` in DI. |

## Related
- [RepositoryFactory](RepositoryFactory.md)
- [ProtectingTheApplicationLayer](ProtectingTheApplicationLayer.md)
- [UnitOfWork](UnitOfWork.md)
- [QueryFilters](QueryFilters.md)
