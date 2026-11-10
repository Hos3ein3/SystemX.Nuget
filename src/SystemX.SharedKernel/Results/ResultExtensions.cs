namespace SystemX.SharedKernel.Results;

/// <summary>Helper methods to chain results.</summary>
public static class ResultExtensions
{
    /// <summary>Transforms the value of a successful result.</summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map) =>
        result.IsSuccess ? Result.Success(map(result.Value)) : Result.Failure<TOut>(result.Error);

    /// <summary>Runs the next step that returns a result.</summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind) =>
        result.IsSuccess ? bind(result.Value) : Result.Failure<TOut>(result.Error);

    /// <summary>Fails with the error when the predicate is false.</summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error) =>
        result.IsFailure ? result : predicate(result.Value) ? result : Result.Failure<T>(error);

    /// <summary>Returns one of two values depending on success or failure.</summary>
    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    /// <summary>Returns one of two values depending on success or failure.</summary>
    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<Error, TOut> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);
}
