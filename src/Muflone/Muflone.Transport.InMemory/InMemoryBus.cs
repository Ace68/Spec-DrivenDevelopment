using Microsoft.Extensions.Logging;
using Muflone.Messages;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace Muflone.Transport.InMemory;

public sealed class InMemoryBus : IServiceBus, IEventBus, IDisposable
{
    private readonly InMemorySubscriber _messageSubscriber;
    private readonly ILogger _logger;
    
    public InMemoryBus(IMessageSubscriber messageSubscriber, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType())
                  ?? throw new ArgumentNullException(nameof(loggerFactory));
        
        if (messageSubscriber is InMemorySubscriber inMemorySubscriber)
        {
            _messageSubscriber = inMemorySubscriber;
        }
        else
        {
            _logger.LogError("{MessageSubscriber} is not supported", nameof(messageSubscriber));
            throw new ArgumentException($"{nameof(messageSubscriber)} is not supported");
        }
    }
    
    public Task SendAsync<T>(T command, CancellationToken cancellationToken = new ()) where T : class, ICommand
    {
        if (!_messageSubscriber.TryGetSubscribers(command.GetType(), out var subs))
        {
            _logger.LogInformation("No subscribers found for command of type {CommandType}", command.GetType().Name);
            return Task.CompletedTask;
        }
        
        foreach (var subscriber in subs!)
        {
            subscriber.Channel.Writer.TryWrite(command);
            _logger.LogInformation("Command of type {CommandType} sent to subscriber", command.GetType().Name);
        }
        
        return Task.CompletedTask;
    }
    
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = new ()) where T : class, IEvent
    {
        if (!_messageSubscriber.TryGetSubscribers(@event.GetType(), out var subs))
        {
            _logger.LogInformation("No subscribers found for event of type {EventType}", @event.GetType().Name);
            return Task.CompletedTask;
        }
        
        foreach (var subscriber in subs!)
        {
            subscriber.Channel.Writer.TryWrite(@event);
            _logger.LogInformation("Event of type {EventType} published to subscriber", @event.GetType().Name);
        }
        
        return Task.CompletedTask;
    }
    
    #region IDisposable Support
    private bool _disposedValue; // To detect redundant calls

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;
        
        if (disposing)
        {
            // TODO: dispose managed state (managed objects).
        }
        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.
        _disposedValue = true;
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~InMemoryBus() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}