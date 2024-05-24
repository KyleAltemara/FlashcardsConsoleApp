using Microsoft.Extensions.DependencyInjection;

namespace FlashcardsConsoleApp;

static class Program
{
    static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.Unicode;
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        var serviceProvider = Startup.ConfigureServices();
        var application = serviceProvider.GetRequiredService<Application>();
        Task.Run(application.Run).GetAwaiter().GetResult();
    }
}
