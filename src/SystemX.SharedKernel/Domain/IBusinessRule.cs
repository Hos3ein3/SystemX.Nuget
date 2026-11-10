namespace SystemX.SharedKernel.Domain;

/// <summary>A business invariant that must hold.</summary>
public interface IBusinessRule
{
    /// <summary>Gets a stable code, for example "Order.EmptyLines".</summary>
    string Code { get; }

    /// <summary>Gets a human readable message.</summary>
    string Message { get; }

    /// <summary>Returns true when the rule is violated.</summary>
    bool IsBroken();
}
