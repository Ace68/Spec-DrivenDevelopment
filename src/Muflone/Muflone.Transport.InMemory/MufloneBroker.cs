using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using System.Collections.ObjectModel;

namespace Muflone.Transport.InMemory;

public static class MufloneBroker
{
	public static ObservableCollection<ICommand> Commands = new();
	public static ObservableCollection<IEvent> Events = new();

	public static void Send(ICommand command)
	{
		Commands.Add(command);
	}

	public static void Publish(IEvent @event)
	{
		Events.Add(@event);
	}
}