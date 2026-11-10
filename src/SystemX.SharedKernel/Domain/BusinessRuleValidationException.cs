namespace SystemX.SharedKernel.Domain;

/// <summary>Thrown when a business rule is broken.</summary>
public sealed class BusinessRuleValidationException : DomainException
{
    /// <summary>Creates the exception from a broken rule.</summary>
    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule?.Message ?? throw new ArgumentNullException(nameof(brokenRule)))
    {
        BrokenRule = brokenRule;
        Code = brokenRule.Code;
    }

    /// <summary>Gets the broken rule.</summary>
    public IBusinessRule BrokenRule { get; }

    /// <summary>Gets the rule code.</summary>
    public string Code { get; }
}
