namespace SantaClaus.Shared.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// </summary>
public record Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Contains the error message if the operation failed, otherwise null.
    /// </summary>
    public string? Error { get; init; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("A successful result cannot have an error.");

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException("A failed result must have an error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<T> Success<T>(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
public record Result<T> : Result
{
    /// <summary>
    /// Contains the result value if the operation was successful, otherwise the default value.
    /// </summary>
    public T? Value { get; init; }

    internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        if (isSuccess && value == null)
            throw new InvalidOperationException("A successful result must have a value.");

        Value = value;
    }
}
