using src.Data;
using src.Repositories;

namespace src.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));

        return services;
    }

    
}