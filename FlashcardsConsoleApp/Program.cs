using FlashcardsConsoleApp.DataAccess.Repositories;
using FlashcardsConsoleApp.DemoStacks;
using FlashcardsConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Configuration;
using System.Text.Json;

namespace FlashcardsConsoleApp;

class Program
{

    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<FlashCardDbContext>(options =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["FlashCardDbContext"].ConnectionString;
                options.UseSqlServer(connectionString);
            })
            .AddScoped<IFlashCardRepository, FlashCardRepository>()
            .AddScoped<IStackRepository, StackRepository>()
            .AddScoped<IStackService, StackService>()
            .BuildServiceProvider();

        var stackService = serviceProvider.GetService<IStackService>();
        if (stackService is null)
        {
            AnsiConsole.Markup($"[red]Failed to resolve {nameof(IStackService)}.[/]");
            return;
        }

        if (stackService.GetAllStacksAsync().Result.Count() == 0)
        {
            bool answer = AnsiConsole.Confirm("No stacks found. Import demo stacks?", false);
            if (answer)
            {
                ImportDemoStacks(stackService);
            }

        }

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an option:")
                    .AddChoices(GetMenuOptions()));

            switch (choice)
            {

                case "Add Stack":
                    var stackName = AnsiConsole.Ask<string>("Enter stack name:");
                    var added = await stackService.AddStackAsync(stackName);
                    if (!added)
                    {
                        AnsiConsole.Markup("[red]Stack name is already taken. Please choose a different name.[/]");
                    }

                    break;
                case "View All Stacks":
                    var stacks = await stackService.GetAllStacksAsync();
                    var table = new Table();
                    table.AddColumn("ID");
                    table.AddColumn("Name");
                    table.AddColumn("FlashCards");
                    foreach (var stack in stacks)
                    {
                        table.AddRow(stack.Id.ToString(), stack.Name, stack.FlashCards.Count.ToString());
                    }

                    AnsiConsole.Write(table);
                    break;

                case "Add FlashCard to Stack":
                    var stackId = AnsiConsole.Ask<int>("Enter stack ID:");
                    var question = AnsiConsole.Ask<string>("Enter question:");
                    var answer = AnsiConsole.Ask<string>("Enter answer:");
                    await stackService.AddFlashCardToStackAsync(stackId, question, answer);
                    break;

                case "View All FlashCards in a Stack":
                    var viewStackId = AnsiConsole.Ask<int>("Enter stack ID:");
                    var stackDetails = await stackService.GetStackByIdAsync(viewStackId);
                    if (stackDetails != null)
                    {
                        var flashCardTable = new Table();
                        flashCardTable.AddColumn("ID");
                        flashCardTable.AddColumn("Question");
                        flashCardTable.AddColumn("Answer");
                        foreach (var flashCard in stackDetails.FlashCards)
                        {
                            flashCardTable.AddRow(flashCard.Id.ToString(), flashCard.Question, flashCard.Answer);
                        }

                        AnsiConsole.Write(flashCardTable);
                    }
                    break;

                case "Update Stack":
                    var updateStackId = AnsiConsole.Ask<int>("Enter stack ID to update:");
                    var newStackName = AnsiConsole.Ask<string>("Enter new stack name:");
                    await stackService.RenameStackAsync(updateStackId, newStackName);
                    break;

                case "Delete Stack":
                    var deleteStackId = AnsiConsole.Ask<int>("Enter stack ID to delete:");
                    await stackService.DeleteStackAsync(deleteStackId);
                    break;

                case "Exit":
                    return;

                default:
                    AnsiConsole.Markup("[red]Invalid option. Please try again.[/]");
                    break;
            }
        }
    }

    private static async void ImportDemoStacks(IStackService stackService)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\DemoStacks\DemoStacks.json");
        if (!File.Exists(path))
        {
            AnsiConsole.Markup("[red]DemoStacks.json not found.[/]");
            return;
        }

        var json = await File.ReadAllTextAsync(path);
        var demoStacks = JsonSerializer.Deserialize<List<JSONStack>>(json);
        if (demoStacks is null)
        {
            AnsiConsole.Markup("[red]Failed to deserialize DemoStacks.json.[/]");
            return;
        }

        foreach (var jsonStack in demoStacks)
        {
            var added = await stackService.AddStackAsync(jsonStack.Name);
            if (!added)
            {
                AnsiConsole.Markup($"[red]Failed to add stack {jsonStack.Name}.[/]");
                continue;
            }

            var stack = await stackService.GetStackByNameAsync(jsonStack.Name);
            if (stack is null)
            {
                AnsiConsole.Markup($"[red]Failed to retrieve stack {jsonStack.Name}.[/]");
                continue;
            }

            foreach (var card in jsonStack.Cards)
            {
                await stackService.AddFlashCardToStackAsync(stack.Id, card.Question, card.Answer);
            }
        }
    }

    static IEnumerable<string> GetMenuOptions()
    {
        yield return "Add Stack";
        yield return "View All Stacks";
        yield return "Add FlashCard to Stack";
        yield return "View All FlashCards in a Stack";
        yield return "Update Stack";
        yield return "Delete Stack";
        yield return "Exit";
    }
}
