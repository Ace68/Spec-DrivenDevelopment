using System.Threading.Channels;
using Muflone.Messages.Events;

namespace Muflone.Transport.InMemory;

public interface IInMemoryChannel
{
    Channel<object> Channel { get; }
}

public interface IInMemoryChannel<T> : IInMemoryChannel
    where T : class, IEvent
{
}