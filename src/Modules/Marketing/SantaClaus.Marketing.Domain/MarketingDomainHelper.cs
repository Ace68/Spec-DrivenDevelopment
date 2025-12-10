using Microsoft.Extensions.DependencyInjection;
using Muflone;
using SantaClaus.Marketing.Domain.CommandHandlers;

namespace SantaClaus.Marketing.Domain;

public static class MarketingDomainHelper
{
    public static IServiceCollection AddMarketingDomain(this IServiceCollection services)
    {
        services.AddCommandHandler<ApproveWishHandler>();
        services.AddCommandHandler<CreateWishHandler>();
        services.AddCommandHandler<CreateLetterHandler>();
        services.AddCommandHandler<ProcessLetterHandler>();
        services.AddCommandHandler<RejectWishHandler>();
        
        return services;
    }
}