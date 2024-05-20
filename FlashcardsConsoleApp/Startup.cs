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
        var services = new ServiceCollection()
        .AddDbContext<FlashCardDbContext>(options =>
        {
            var connectionString = ConfigurationManager.ConnectionStrings["FlashCardDbContext"].ConnectionString;
            options.UseSqlServer(connectionString);
        })
        .AddScoped<IFlashCardRepository, FlashCardRepository>()
        .AddScoped<IStudySessionRepository, StudySessionRepository>()
        .AddScoped<IStackRepository, StackRepository>()
        .AddScoped<IStackService, StackService>()
        .AddSingleton<Application>();

        return services.BuildServiceProvider();
    }
}
