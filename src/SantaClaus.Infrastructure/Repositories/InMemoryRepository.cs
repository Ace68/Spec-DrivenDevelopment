using Muflone;
using Muflone.Core;
using Muflone.Persistence;

namespace SantaClaus.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of IRepository for storing and retrieving aggregates.
/// </summary>
public class InMemoryRepository : IRepository
{
    private IEnumerable<object> _givenEvents = Enumerable.Empty<object>();
    public IEnumerable<object> Events { get; private set; } = Enumerable.Empty<object>();

    private static TAggregate? ConstructAggregate<TAggregate>()
    {
        return (TAggregate?)Activator.CreateInstance(typeof(TAggregate), true);
    }

    public void Dispose()
    {
        // no-op
    }

    public virtual void ApplyGivenEvents(IList<object> events)
    {
        _givenEvents = events;
    }

    public virtual async Task<TAggregate?> GetByIdAsync<TAggregate>(IDomainId id, CancellationToken cancellationToken)
        where TAggregate : class, IAggregate
    {
        return await GetByIdAsync<TAggregate>(id, 0, cancellationToken);
    }

    public Task<TAggregate?> GetByIdAsync<TAggregate>(IDomainId id, long version, CancellationToken cancellationToken)
        where TAggregate : class, IAggregate
    {
        var aggregate = ConstructAggregate<TAggregate>();
        if (aggregate is not null)
        {
            _givenEvents.ForEach(aggregate.ApplyEvent);
        }
        return Task.FromResult(aggregate);
    }

    public virtual async Task SaveAsync(IAggregate aggregate, Guid commitId, CancellationToken cancellationToken = default)
    {
        await SaveAsync(aggregate, commitId, null, cancellationToken);
    }

    public virtual Task SaveAsync(IAggregate aggregate, Guid commitId, Action<IDictionary<string, object>>? updateHeaders, CancellationToken cancellationToken = default)
    {
        Events = aggregate.GetUncommittedEvents().Cast<object>();
        return Task.CompletedTask;
    }
}

public static class Helpers
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        if (items == null)
            return;
        foreach (var obj in items)
            action(obj);
    }
}
