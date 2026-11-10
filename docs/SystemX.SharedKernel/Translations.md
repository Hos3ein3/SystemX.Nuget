# Translations

**Package:** `SystemX.SharedKernel`  
**Namespace:** `SystemX.SharedKernel.Markers`

## What it is
Domain abstractions (`ITranslatable<T>`, `ITranslation`) and lookup utilities (`TranslationExtensions.Pick`) for managing multilingual entities.

## When to use it
- Use it on product catalogs, article content, or categories requiring content in multiple languages.
- Do NOT use it for static UI label localization; use resource files (`.resx`) for UI strings.

## Installation
`dotnet add package SystemX.SharedKernel`

## How to use it
Implement `ITranslatable<TTranslation>` on the parent entity and use `entity.Pick(culture, fallback)` to resolve translated text.

## Sample code

```csharp
using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Identifiers;
using SystemX.SharedKernel.Markers;

namespace Example.TranslationsDoc;

public sealed record ProductId(Guid Value) : GuidId(Value), IGuidIdFactory<ProductId>
{
    public static ProductId Create(Guid value) => new(value);
}

public sealed record ProductTranslation(string Culture, string Title, string Description) : ITranslation;

public sealed class Product : Entity<ProductId>, ITranslatable<ProductTranslation>
{
    private readonly List<ProductTranslation> _translations = [];

    public IReadOnlyCollection<ProductTranslation> Translations => _translations.AsReadOnly();

    public Product(ProductId id, IEnumerable<ProductTranslation> translations) : base(id)
    {
        _translations.AddRange(translations);
    }
}

public static class TranslationSample
{
    public static void Run()
    {
        Product product = new(
            ProductId.Create(Guid.NewGuid()),
            [
                new ProductTranslation("en-US", "Wireless Mouse", "A fast ergonomic mouse."),
                new ProductTranslation("nl-NL", "Draadloze Muis", "Een ergonomische muis."),
                new ProductTranslation("fa-IR", "موس بی سیم", "یک موس ارگونومیک.")
            ]);

        // Resolves exact match or neutral culture fallback
        ProductTranslation? translation = product.Pick(culture: "nl-BE", fallbackCulture: "en-US");
        Console.WriteLine($"Resolved title: {translation?.Title}");
    }
}
```

## Related
- [EntityMarkers](EntityMarkers.md)
