using Muflone.Messages.Commands;
using Muflone.Persistence;
using Microsoft.Extensions.Logging;
using SantaClaus.Shared.Commands;

namespace SantaClaus.Marketing.Domain.CommandHandlers;

public abstract class CommandHandlerBaseAsync<TCommand>(IRepository repository,
    ILoggerFactory loggerFactory) : ICommandHandlerAsync<TCommand> where TCommand : SantaClaus.Shared.Commands.Command
{
    protected readonly IRepository Repository = repository;
    protected readonly ILogger Logger = loggerFactory.CreateLogger<CommandHandlerBaseAsync<TCommand>>();

    public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken = new());

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CommandHandlerBaseAsync()
    {
        Dispose(false);
    }

    #endregion
}
