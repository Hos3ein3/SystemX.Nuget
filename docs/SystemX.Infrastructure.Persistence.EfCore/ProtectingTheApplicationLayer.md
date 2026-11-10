# ProtectingTheApplicationLayer

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore`

## What it is
Architectural guidelines and compiler-enforced boundaries that prevent Entity Framework Core implementation details from leaking into domain and application layers.

## When to use it
- Use this design across all consumer applications to keep commands, queries, domain entities, and handlers cleanly decoupled from database technology.
- Do NOT bypass these boundaries by adding EF Core references directly to Application or Domain projects.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## How to use it
To enforce the persistence boundary:
1. Reference only `SystemX.SharedKernel` and `SystemX.Application.Abstractions` from Domain and Application projects.
2. In application command and query handlers, interact strictly with `IRepository<TEntity, TId>`, `IReadRepository<TEntity, TId>`, `ISpecification<T>`, and `IUnitOfWork`.
3. Add `Microsoft.CodeAnalysis.BannedApiAnalyzers` to the consumer's Application project to flag forbidden persistence types at compile-time.

## Sample code

### The Good Handler vs The Bad Handler

The following good handler depends only on application abstractions:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Specifications;

namespace ApplicationSample.Good;

public sealed record CustomerId(Guid Value) : GuidId(Value), IGuidIdFactory<CustomerId>
{
    public static CustomerId Create(Guid value) => new(value);
}

public class Customer : AggregateRoot<CustomerId>
{
    public Customer(CustomerId id, string name) : base(id) => Name = name;
    public string Name { get; private set; }
    public void Rename(string newName) => Name = newName;
}

public sealed class RenameCustomerHandler
{
    private readonly IRepository<Customer, CustomerId> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RenameCustomerHandler(
        IRepository<Customer, CustomerId> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CustomerId id, string newName, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new InvalidOperationException($"Customer {id} was not found.");
        }

        customer.Rename(newName);
        _repository.Update(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

By contrast, the bad handler couples directly to EF Core:

```csharp
// BAD HANDLER: Leaks DbContext, DbSet, and EF Core change tracking directly into application logic.
// Why it is harmful:
// 1. Tightly couples business workflows to EF Core and specific relational SQL semantics.
// 2. Bypasses domain invariants, aggregate root boundaries, and SystemX multi-tenancy filters.
// 3. Makes unit testing impossible without setting up an in-memory or mock relational database.
/*
public sealed class BadRenameCustomerHandler
{
    private readonly MyDbContext _context; // VIOLATION: DbContext in application layer

    public BadRenameCustomerHandler(MyDbContext context) => _context = context;

    public async Task HandleAsync(Guid id, string newName, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
        customer.Name = newName;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
*/
```

### Defining Custom Application Repository Interfaces

When domain or application logic requires aggregate-specific queries, define an interface in the Application layer extending `IRepository`:

```csharp
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;

namespace ApplicationSample.Good;

public interface ICustomerRepository : IRepository<Customer, CustomerId>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
```

Implement this interface in your Infrastructure/Persistence project by deriving from `EfRepository<Customer, CustomerId>`.

### BannedApiAnalyzer Setup

To automate enforcement in the consumer's Application `.csproj`, add the banned API analyzer:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.BannedApiAnalyzers" Version="3.3.4">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <AdditionalFiles Include="BannedSymbols.txt" />
</ItemGroup>
```

Create `BannedSymbols.txt` in the root of the Application project:

```text
T:Microsoft.EntityFrameworkCore.DbContext;Entity Framework Core DbContext is banned in Application layer. Use IUnitOfWork or IRepository.
T:Microsoft.EntityFrameworkCore.DbSet`1;Entity Framework Core DbSet is banned in Application layer. Use IRepository<TEntity, TId>.
M:Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.*;EF Core extension methods are banned outside Persistence.
T:System.Linq.IQueryable`1;IQueryable is banned from repository contracts. Repositories must return evaluated results or accept ISpecification<T>.
```

## Configuration (if any)
No configuration options.

## Related
- [DatabaseFactories](../SystemX.Application.Abstractions/DatabaseFactories.md)
- [SystemXDbContext](SystemXDbContext.md)
- [Repositories](Repositories.md)
- [UnitOfWork](UnitOfWork.md)
