using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Muflone.Messages;

namespace Muflone.Transport.InMemory;

public class InMemorySubscriber(
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider) : MessageSubscriberBase<IInMemoryChannel>(loggerFactory, serviceProvider)
{
    private readonly ConcurrentDictionary<Type, List<IInMemoryChannel>> _subscribers = new();
    private readonly ILogger _logger = loggerFactory.CreateLogger<InMemorySubscriber>();
    
    protected override Task StopChannelAsync(HandlerSubscription<IInMemoryChannel> handlerSubscription)
    {
        if (handlerSubscription.Channel == null) return Task.CompletedTask;

        handlerSubscription.Channel.Channel.Writer.TryComplete();

        if (_subscribers.TryGetValue(handlerSubscription.EventType, out var channels))
        {
            lock (channels)
            {
                channels.Remove(handlerSubscription.Channel);
                if (channels.Count == 0)
                {
                    _subscribers.TryRemove(handlerSubscription.EventType, out _);
                }
            }
        }

        handlerSubscription.Channel = null;

        return Task.CompletedTask;
    }

    protected override Task InitChannelAsync(HandlerSubscription<IInMemoryChannel> handlerSubscription)
    {
        return Task.CompletedTask;
    }

    protected override Task InitSubscriptionAsync(HandlerSubscription<IInMemoryChannel> handlerSubscription)
    {
        var channel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        var routingKey = GetRoutingKey(handlerSubscription);
        var inMemoryChannel = new InMemoryChannel(handlerSubscription, channel, routingKey);

        var stream = Subscribe(inMemoryChannel, handlerSubscription.EventType, CancellationToken.None);

        _ = Task.Run(async () =>
        {
            await foreach (var msg in stream)
            {
                await handlerSubscription.MessageAsync(JsonSerializer.Serialize(msg) ,CancellationToken.None);
            }
        });
        
        return Task.CompletedTask;
    }
    
    public bool TryGetSubscribers(Type type, out IInMemoryChannel[]? typeSubscribers)
    {
        typeSubscribers = null;
        
        string?[]? namingComponent = type.FullName?.Split('.');
        var registeredSubscribers = _subscribers.Where(s => s.Key.FullName!.Contains(namingComponent?[^1] ?? string.Empty));
        _subscribers.TryGetValue(type, out var list);

        var subscribersArray = registeredSubscribers as KeyValuePair<Type, List<IInMemoryChannel>>[] ?? registeredSubscribers.ToArray();
        if (list == null && subscribersArray.Any())
        {
            // Looking for same subscriber, but with different namespace
            list = [];
            foreach (var subscriber in subscribersArray)
            {
                foreach (var sub in subscriber.Value)
                    list.Add(sub);
            }
            
            typeSubscribers = list.ToArray();
            
            if (typeSubscribers.Length.Equals(0))
                _logger.LogInformation("No subscribers found for message type {MessageType}", type.Name);
        }
        else
            typeSubscribers = list?.ToArray();

        return typeSubscribers != null;
    }
    
    private static string GetRoutingKey(HandlerSubscription<IInMemoryChannel> handlerSubscription)
    {
        return handlerSubscription.Configuration?.RoutingKey ?? handlerSubscription.EventTypeName;
    }
    
    private IAsyncEnumerable<object> Subscribe(IInMemoryChannel inMemoryChannel,
        Type messageType,
        CancellationToken cancellationToken)
    {
        _subscribers.AddOrUpdate(
            messageType,
            _ => [inMemoryChannel],
            (_, list) =>
            {
                lock (list) list.Add(inMemoryChannel);
                return list;
            });

        return inMemoryChannel.Channel.Reader.ReadAllAsync(cancellationToken);
    }
}