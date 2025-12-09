namespace SantaClaus.Shared.Exceptions;

/// <summary>
/// Exception thrown when a requested aggregate or entity cannot be found.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Gets the type of the entity that was not found.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity that was not found.
    /// </summary>
    public object EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="entityId">The identifier of the entity that was not found.</param>
    public NotFoundException(string entityType, object entityId)
        : base($"{entityType} with identifier '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom message.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="entityId">The identifier of the entity that was not found.</param>
    /// <param name="message">A custom error message.</param>
    public NotFoundException(string entityType, object entityId, string message)
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
