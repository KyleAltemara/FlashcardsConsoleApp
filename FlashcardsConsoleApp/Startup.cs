using FlashcardsConsoleApp.DataAccess;
using FlashcardsConsoleApp.DataAccess.Repositories;
using FlashcardsConsoleApp.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace FlashcardsConsoleApp;

public class Startup
{
    public static IServiceProvider ConfigureServices()
    {
        // Create a new service collection
        // A service collection is a container for service descriptors, which specify the services that are available in the application.
        // The service collection is used to register services and build a service provider, which is used to resolve services.
        // The service provider is responsible for creating instances of services and managing their lifetimes.
        var services = new ServiceCollection()

            // Add a DbContext to the service collection
            .AddDbContext<FlashCardDbContext>(options =>
            {
                // Retrieve the connection string from the configuration file
                var connectionString = ConfigurationManager.ConnectionStrings["FlashCardDbContext"].ConnectionString;

                // Configure the DbContext to use SQL Server with the retrieved connection string
                options.UseSqlServer(connectionString);
            })

            // Add scoped instances of the repositories and services
            // AddScoped registers the service with a scoped lifetime. This means that a new instance of the service is created for each scope.
            // In this case, the scope is the lifetime of the request.
            .AddScoped<IFlashCardRepository, FlashCardRepository>()
            .AddScoped<IStudySessionRepository, StudySessionRepository>()
            .AddScoped<IStackRepository, StackRepository>()
            .AddScoped<IStackService, StackService>()

            // Add a singleton instance of the Application class
            .AddSingleton<Application>();

        // Build and return the service provider
        return services.BuildServiceProvider();
    }
}
