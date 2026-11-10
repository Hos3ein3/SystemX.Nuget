# SystemX.Contracts.Validation

## Purpose
FluentValidation validators for the shared DTOs in `SystemX.Contracts`. Lets clients and services validate the same DTOs without making `Contracts` depend on FluentValidation.

Part of the SystemX library suite.

## Installation

```bash
dotnet add package SystemX.Contracts.Validation
```

## Usage

```csharp
using SystemX.Contracts.Pagination;
using SystemX.Contracts.Validation;

var validator = new PagedRequestValidator();
var result = validator.Validate(new PagedRequest(Page: 1, PageSize: 50));
```

## Documentation

- [ContractValidators](file:///Users/mohammadhosein/Development/SystemX/nuget/docs/SystemX.Contracts.Validation/ContractValidators.md)
