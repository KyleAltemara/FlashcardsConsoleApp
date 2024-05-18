using FlashcardsConsoleApp.Services;
using Spectre.Console;

namespace FlashcardsConsoleApp;

public class Application
{
    private readonly IStackService _stackService;

    public Application(IStackService stackService)
    {
        _stackService = stackService;
    }

    public async Task Run()
    {
        if (!_stackService.GetAllStacksAsync().Result.Any())
        {
            bool answer = AnsiConsole.Confirm("No stacks found. Import demo stacks?", false);
            if (answer)
            {
                DemoImporter.ImportDemoStacks(_stackService);
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
                    var added = await _stackService.AddStackAsync(stackName);
                    if (!added)
                    {
                        AnsiConsole.Markup("[red]Stack name is already taken. Please choose a different name.[/]");
                    }

                    break;

                case "View All Stacks":
                    var stacks = await _stackService.GetAllStacksAsync();
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
                    await _stackService.AddFlashCardToStackAsync(stackId, question, answer);
                    break;

                case "View All FlashCards in a Stack":
                    var viewStackId = AnsiConsole.Ask<int>("Enter stack ID:");
                    var stackDetails = await _stackService.GetStackByIdAsync(viewStackId);
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
                    else
                    {
                        AnsiConsole.Markup("[red]Stack not found.[/]");
                    }

                    break;

                case "Update Stack":
                    var updateStackId = AnsiConsole.Ask<int>("Enter stack ID to update:");
                    var newStackName = AnsiConsole.Ask<string>("Enter new stack name:");
                    if (await _stackService.GetStackByNameAsync(newStackName) != null)
                    {
                        AnsiConsole.Markup("[red]Stack name is already taken. Please choose a different name.[/]");
                        break;
                    }

                    if (await _stackService.GetStackByIdAsync(updateStackId) == null)
                    {
                        AnsiConsole.Markup("[red]Stack not found.[/]");
                        break;
                    }

                    await _stackService.RenameStackAsync(updateStackId, newStackName);
                    break;

                case "Delete Stack":
                    var deleteStackId = AnsiConsole.Ask<int>("Enter stack ID to delete:");
                    if (await _stackService.GetStackByIdAsync(deleteStackId) == null)
                    {
                        AnsiConsole.Markup("[red]Stack not found.[/]");
                        break;
                    }

                    await _stackService.DeleteStackAsync(deleteStackId);
                    break;

                case "Exit":
                    AnsiConsole.Markup("[red]Exiting Program[/]");
                    return;

                default:
                    AnsiConsole.Markup("[red]Invalid option.[/]");
                    break;
            }
        }
    }

    private static IEnumerable<string> GetMenuOptions()
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
