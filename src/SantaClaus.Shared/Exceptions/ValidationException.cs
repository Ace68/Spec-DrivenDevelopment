namespace SantaClaus.Shared.Exceptions;

/// <summary>
/// Exception thrown when validation fails for domain objects or operations.
/// </summary>
public class ValidationException : DomainException
{
    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a single error message.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with multiple validation errors.
    /// </summary>
    /// <param name="errors">A dictionary of field names and their associated error messages.</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a message and validation errors.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    /// <param name="errors">A dictionary of field names and their associated error messages.</param>
    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, string[]>();
    }
}
