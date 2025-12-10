using Microsoft.Extensions.DependencyInjection;
using Muflone.Messages;
using Muflone.Persistence;

namespace Muflone.Transport.InMemory;

public static class MufloneTransportInMemoryHelper
{
	public static IServiceCollection AddMufloneTransportInMemory(this IServiceCollection services)
	{
		services.AddSingleton<IMessageSubscriber, InMemorySubscriber>();
		services.AddSingleton<IServiceBus, InMemoryBus>();
		services.AddSingleton<IEventBus, InMemoryBus>();

		services.AddHostedService<MessageHandlersStarter>();
        
		return services;
	}
}