# RepositoryFactory

**Package:** `SystemX.Infrastructure.Persistence.EfCore`
**Namespace:** `SystemX.Infrastructure.Persistence.EfCore.Repositories`

## What it is
Implementation of `IRepositoryFactory` (`EfRepositoryFactory`) that dynamically instantiates and caches repository instances bound to a single DbContext unit of work.

## When to use it
- Use it when dynamic repository resolution is needed at runtime, such as in generic handlers, dynamic batch processors, or within `IDatabaseSession`.
- Do NOT use it as a replacement for constructor injection in standard handlers where dependencies are known statically at design time.

## Installation
`dotnet add package SystemX.Infrastructure.Persistence.EfCore`

## How to use it
Register the persistence layer using `services.AddSystemXDbContext<TContext>()`. `IRepositoryFactory` is automatically registered as a scoped service.

Inject `IRepositoryFactory` and retrieve repositories:
```csharp
var repository = repositoryFactory.GetRepository<TEntity, TId>();
```

## Sample code

### Dynamic Repository Resolution

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;

namespace ApplicationSample;

public sealed record ArticleId(Guid Value) : GuidId(Value), IGuidIdFactory<ArticleId>
{
    public static ArticleId Create(Guid value) => new(value);
}

public class Article : AggregateRoot<ArticleId>
{
    public Article(ArticleId id, string title) : base(id) => Title = title;
    public string Title { get; private set; }
}

public sealed class DynamicArchiver
{
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly IUnitOfWork _unitOfWork;

    public DynamicArchiver(IRepositoryFactory repositoryFactory, IUnitOfWork unitOfWork)
    {
        _repositoryFactory = repositoryFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task DeleteArticleAsync(ArticleId articleId, CancellationToken cancellationToken)
    {
        // Dynamic resolution returns a cached repository instance sharing the ambient unit of work
        var repo = _repositoryFactory.GetRepository<Article, ArticleId>();
        var article = await repo.GetByIdAsync(articleId, cancellationToken);
        if (article is not null)
        {
            repo.Remove(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
```

### Constructor Injection vs Repository Factory

```csharp
using System.Threading;
using System.Threading.Tasks;
using SystemX.Application.Abstractions.Persistence;

namespace ApplicationSample;

// PREFERRED: Constructor injection makes dependencies explicit, supports easy mocking, and avoids service location
public sealed class PreferredArticleService
{
    private readonly IRepository<Article, ArticleId> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PreferredArticleService(
        IRepository<Article, ArticleId> articleRepository,
        IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateArticleAsync(ArticleId id, string title, CancellationToken cancellationToken)
    {
        _articleRepository.Add(new Article(id, title));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

## Configuration (if any)
| Option | Default | Description |
|---|---|---|
| `RegisterRepositories` | `true` | When true, registers `IRepositoryFactory` in the DI container. |

## Related
- [DatabaseFactories](../SystemX.Application.Abstractions/DatabaseFactories.md)
- [Repositories](Repositories.md)
- [UnitOfWork](UnitOfWork.md)
- [DatabaseFactory](DatabaseFactory.md)
