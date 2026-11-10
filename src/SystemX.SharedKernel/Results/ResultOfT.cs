namespace SystemX.SharedKernel.Results;

/// <summary>The outcome of an operation with a value.</summary>
/// <typeparam name="T">The value type.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>Creates a result. Use the factory methods instead.</summary>
    protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>Gets the value. Throws when the result is a failure.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failed result cannot be accessed.");

    /// <summary>Converts a value to a result.</summary>
    public static implicit operator Result<T>(T? value) => Create(value);

    /// <summary>Converts an error to a failed result.</summary>
    public static implicit operator Result<T>(Error error) => Failure<T>(error);
}
