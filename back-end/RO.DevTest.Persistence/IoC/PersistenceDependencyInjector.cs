using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace RO.DevTest.Persistence.IoC;

public static class PersistenceDependencyInjector {
    /// <summary>
    /// Inject the dependencies of the Persistence layer into an
    /// <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to inject the dependencies into
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> with dependencies injected
    /// </returns>
    public static IServiceCollection InjectPersistenceDependencies(this IServiceCollection services) {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Server=localhost;port=5432;Database=rodevtest;User Id=postgres;Password=root;";
            
        services.AddDbContext<DefaultContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
