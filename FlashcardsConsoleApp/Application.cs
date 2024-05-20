using FlashcardsConsoleApp.DataAccess.Services;
using FlashcardsConsoleApp.DemoStacks;
using FlashcardsConsoleApp.Models;
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
        bool continueRunning = true;
        while (continueRunning)
        {
            var stacks = await _stackService.GetAllStacksAsync();
            if (!stacks.Any())
            {
                bool answer = AnsiConsole.Confirm("No stacks found. Import demo stacks?", false);
                if (answer)
                {
                    await DemoImporter.ImportDemoStacks(_stackService);
                    stacks = await _stackService.GetAllStacksAsync();
                }
            }

            var menuOptions = new Dictionary<string, Func<Task>>()
            {
                { "View Stacks or Flashcards", () => ViewStacksOrFlashcards(stacks) },
                { "Add Stack or Flashcard", () => AddStackOrFlashcard(stacks) },
                { "Update Stack or Flashcard", () => UpdateStackOrFlashcard(stacks) },
                { "Delete Stack or Flashcard", () => DeleteStackOrFlashcard(stacks) },
                { "Exit", () =>
                    {
                        AnsiConsole.Markup("[red]Exiting Program[/]");
                        continueRunning = false;
                        return Task.CompletedTask;
                    }
                }
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an option:")
                    .AddChoices(menuOptions.Keys));

            if (menuOptions.TryGetValue(choice, out var selectedAction))
            {
                await selectedAction.Invoke();
            }
            else
            {
                AnsiConsole.Markup("[red]Invalid option.[/]");
            }
        }
    }

    private async Task ViewStacksOrFlashcards(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Return");
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select a stack to display all flashcards:")
            .PageSize(10)
            .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        var selectedStack = optionsDictionary[choice];
        var stackId = await _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId);
        var flashcards = await _stackService.GetFlashCardsByStackId(stackId);

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Question");
        table.AddColumn("Answer");

        foreach (var flashcard in flashcards)
        {
            table.AddRow(flashcard.DisplayId.ToString(), flashcard.Question, flashcard.Answer);
        }

        AnsiConsole.Write(table);
    }

    private async Task AddStackOrFlashcard(IEnumerable<StackDTO> stacks)
    {

        List<string> options = ["Add new stack", "Add flashcards to an existing stack", "Return"];
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Choose an option:")
            .PageSize(10)
            .AddChoices(options));

        switch (choice)
        {
            case "Add new stack":
                var stackName = AnsiConsole.Ask<string>("Enter the name of the new stack:");
                if (!await _stackService.AddStackAsync(stackName))
                {
                    AnsiConsole.Markup("[red]Failed to add stack. Stack name must be unique.[/]");
                }

                break;

            case "Add flashcards to an existing stack":
                await AddFlashcards(stacks);
                break;

            case "Return":
                return;

            default:
                AnsiConsole.Markup("[red]Invalid option.[/]");
                break;
        }
    }

    private async Task AddFlashcards(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Return");
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select a stack to display all flashcards:")
            .PageSize(10)
            .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        var selectedStack = optionsDictionary[choice];
        var stackId = await _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId);
        while (true)
        {
            var question = AnsiConsole.Ask<string>("Enter the question:");
            if (string.IsNullOrEmpty(question))
            {
                break;
            }

            var answer = AnsiConsole.Ask<string>("Enter the answer:");
            if (string.IsNullOrEmpty(answer))
            {
                break;
            }

            await _stackService.AddFlashCardToStackAsync(stackId, question, answer);

            var addAnother = AnsiConsole.Confirm("Add another flashcard?", false);
            if (!addAnother)
            {
                break;
            }
        }
    }

    private async Task UpdateStackOrFlashcard(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Return");
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select a stack to update:")
            .PageSize(10)
            .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        var selectedStack = optionsDictionary[choice];
        var stackId = await _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId);

        var updateOptions = new Dictionary<string, Func<Task>>()
        {
            { "Update Stack Name", async () =>
                {
                    var newStackName = AnsiConsole.Ask<string>("Enter the new stack name:");
                    if(!await _stackService.RenameStackAsync(stackId, newStackName))
                    {
                        AnsiConsole.Markup("[red]Failed to rename stack. Stack name must be unique.[/]");
                    }
                }
            },
            { "Update Flashcard cards", async () =>
                {
                    await UpdateFlashcards(stackId);
                }
            },
            {
                "Return", () => Task.CompletedTask
            }
        };

        var updateChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Choose an option to update:")
                .AddChoices(updateOptions.Keys));

        if (updateOptions.TryGetValue(updateChoice, out var selectedUpdateAction))
        {
            await selectedUpdateAction.Invoke();
        }
        else
        {
            AnsiConsole.Markup("[red]Invalid option.[/]");
        }
    }

    private async Task UpdateFlashcards(int stackId)
    {
        while (true)
        {
            var flashcards = await _stackService.GetFlashCardsByStackId(stackId);
            var optionsDictionary = flashcards.ToDictionary(flashcard => $"{flashcard.DisplayId} Q:{flashcard.Question} A:{flashcard.Answer}", flashcard => flashcard);
            var options = optionsDictionary.Keys.ToList();
            options.Add("Return");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a flashcard to update:")
                    .PageSize(10)
                    .AddChoices(options));

            if (choice == "Return")
            {
                return;
            }

            var selectedFlashcard = optionsDictionary[choice];
            var flashcardId = await _stackService.GetFlashCardIdFromDisplayId(selectedFlashcard.DisplayId);

            var newQuestion = AnsiConsole.Ask<string>("Enter the new question:");
            if (string.IsNullOrEmpty(newQuestion))
            {
                AnsiConsole.Markup("[red]No changes made to question.[/]");
                newQuestion = selectedFlashcard.Question;
            }

            var newAnswer = AnsiConsole.Ask<string>("Enter the new answer:");
            if (string.IsNullOrEmpty(newAnswer))
            {
                AnsiConsole.Markup("[red]No changes made to answer.[/]");
                newAnswer = selectedFlashcard.Answer;
            }

            if (newQuestion == selectedFlashcard.Question && newAnswer == selectedFlashcard.Answer)
            {
                AnsiConsole.Markup("[red]No changes made to flashcard.[/]");
            }
            else
            {
                await _stackService.UpdateFlashCardAsync(flashcardId, newQuestion, newAnswer);
            }
        }
    }

    private async Task DeleteStackOrFlashcard(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Return");
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a stack:")
                .PageSize(10)
                .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        var selectedStack = optionsDictionary[choice];
        var stackId = await _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId);
        options = new List<string> { "Delete stack", "Delete flashcards", "Return" };
        choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an option:")
                .PageSize(10)
                .AddChoices(options));

        switch (choice)
        {
            case "Delete stack":
                if (AnsiConsole.Confirm("Are you sure you want to delete this stack?", false))
                {
                    await _stackService.DeleteStackAsync(stackId);
                }

                break;

            case "Delete flashcards":
                await DeleteFlashcards(stackId);
                break;

            case "Return":
                return;

            default:
                AnsiConsole.Markup("[red]Invalid option.[/]");
                break;
        }
    }

    private async Task DeleteFlashcards(int stackId)
    {
        while (true)
        {
            var flashcards = await _stackService.GetFlashCardsByStackId(stackId);
            var optionsDictionary = flashcards.ToDictionary(flashcard => $"{flashcard.DisplayId} Q:{flashcard.Question} A:{flashcard.Answer}", flashcard => flashcard);
            var options = optionsDictionary.Keys.ToList();
            options.Add("Return");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a flashcard to delete:")
                    .PageSize(10)
                    .AddChoices(options));

            if (choice == "Return")
            {
                return;
            }

            if (AnsiConsole.Confirm("Are you sure you want to delete this flashcard?", false))
            {
                var selectedFlashcard = optionsDictionary[choice];
                var flashcardId = await _stackService.GetFlashCardIdFromDisplayId(selectedFlashcard.DisplayId);
                await _stackService.DeleteFlashCardAsync(flashcardId);
            }
        }
    }
}
