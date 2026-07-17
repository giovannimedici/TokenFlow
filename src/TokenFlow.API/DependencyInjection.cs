using TokenFlow.API.Services;
using TokenFlow.API.Repositories;
using Microsoft.EntityFrameworkCore;
using TokenFlow.API.Data;

namespace TokenFlow.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionURI"] 
            ?? throw new InvalidOperationException("MongoDB:ConnectionURI is not set");

        var database = configuration["MongoDB:DatabaseName"]
            ?? throw new InvalidOperationException("MongoDB:DatabaseName is not set");

        services.AddDbContext<TokenFlowDbContext>(options =>
            options.UseMongoDB(connectionString, database));
            
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}