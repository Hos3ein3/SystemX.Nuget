# UnitOfWork

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore`

## What it is
Coordinates transactional changes across multiple repositories and provides transaction control and read connections via `IUnitOfWork`, `ITransactionScope`, and `IReadConnectionFactory`.

## When to use it
- Inject `IUnitOfWork` into command handlers to commit pending entity mutations via `SaveChangesAsync`.
- Use `unitOfWork.ExecuteInTransactionAsync` when multiple operations or repositories must execute atomically within an explicit transaction.
- Use `IReadConnectionFactory` for high-performance direct SQL read queries (e.g. Dapper) sharing the underlying DbContext connection.
- Do NOT dispose the connection returned by `IReadConnectionFactory.GetOpenConnectionAsync()`; the connection is owned by the DbContext.

## Retrying Execution Strategies Warning
When using resilient database providers configured with retry policies (e.g., SQL Server `EnableRetryOnFailure`), manual user transactions (`BeginTransactionAsync`) cannot be executed directly unless wrapped inside EF Core's execution strategy:
```csharp
var strategy = context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    await unitOfWork.ExecuteInTransactionAsync(async ct => { ... });
});
```
Failing to wrap user transactions in an execution strategy when retries are enabled causes `InvalidOperationException`.

## Sample code

### Atomic Transaction with `ExecuteInTransactionAsync`

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;

namespace ApplicationSample;

public sealed record AccountId(Guid Value) : GuidId(Value), IGuidIdFactory<AccountId>
{
    public static AccountId Create(Guid value) => new(value);
}

public class Account : AggregateRoot<AccountId>
{
    public Account(AccountId id, decimal balance) : base(id) => Balance = balance;
    public decimal Balance { get; private set; }
    public void Debit(decimal amount) => Balance -= amount;
    public void Credit(decimal amount) => Balance += amount;
}

public sealed class TransferMoneyHandler
{
    private readonly IRepository<Account, AccountId> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferMoneyHandler(IRepository<Account, AccountId> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(AccountId fromId, AccountId toId, decimal amount, CancellationToken cancellationToken)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var source = await _repository.GetByIdAsync(fromId, ct);
            var destination = await _repository.GetByIdAsync(toId, ct);

            if (source is null || destination is null)
            {
                throw new InvalidOperationException("One or both accounts not found.");
            }

            source.Debit(amount);
            destination.Credit(amount);

            _repository.Update(source);
            _repository.Update(destination);

            await _unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}
```

### High-Speed Read Query with `IReadConnectionFactory`

```csharp
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;

namespace ApplicationSample;

public sealed class FastMetricsReader
{
    private readonly IReadConnectionFactory _readConnectionFactory;

    public FastMetricsReader(IReadConnectionFactory readConnectionFactory)
    {
        _readConnectionFactory = readConnectionFactory;
    }

    public async Task<int> GetTotalOrdersTodayAsync(CancellationToken cancellationToken)
    {
        // Connection belongs to the ambient DbContext; do NOT dispose it!
        var connection = await _readConnectionFactory.GetOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM Orders WHERE CreatedAt >= CURRENT_DATE";

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `RegisterRepositories` | `true` | When true, registers `IUnitOfWork` and `IReadConnectionFactory` in DI. |

## Related
- [ProtectingTheApplicationLayer](ProtectingTheApplicationLayer.md)
- [Repositories](Repositories.md)
- [SystemXDbContext](SystemXDbContext.md)
- [ConnectionFactory](ConnectionFactory.md)
