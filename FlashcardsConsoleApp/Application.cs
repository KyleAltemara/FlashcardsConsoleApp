using FlashcardsConsoleApp.DataAccess.Services;
using FlashcardsConsoleApp.DemoStacks;
using FlashcardsConsoleApp.Models;
using Spectre.Console;

namespace FlashcardsConsoleApp;

public class Application
{
    /// <summary>
    /// the IStackService is used to interact with the data store
    /// </summary>
    private readonly IStackService _stackService;

    public Application(IStackService stackService)
    {
        _stackService = stackService;
    }

    /// <summary>
    /// This is the main entry point for the application.
    /// </summary>
    public async Task Run()
    {
        bool continueRunning = true;
        while (continueRunning)
        {
            var stacks = await _stackService.GetAllStacksAsync();
            if (!stacks.Any())
            {
                if (AnsiConsole.Confirm("No stacks found. Import demo stacks?", false))
                {
                    await DemoImporter.ImportDemoStacks(_stackService);
                    if (AnsiConsole.Confirm("Demo stacks imported. Generate demo study sessions?", false))
                    {
                        await DemoImporter.GenerateStudySessions(_stackService);
                    }

                    stacks = await _stackService.GetAllStacksAsync();
                }
            }

            var menuOptions = new Dictionary<string, Func<Task>>
            {
                { "Study a stack", () => SelectStackToStudy(stacks) },
                { "View stacks or flashcards", () => ViewStacksOrFlashcards(stacks) },
                { "Add stack or flashcard", () => AddStackOrFlashcard(stacks) },
                { "Update stack or flashcard", () => UpdateStackOrFlashcard(stacks) },
                { "Delete stack or flashcard", () => DeleteStackOrFlashcard(stacks) },
                { "View study session data", () => ViewStudySessionData(stacks) },
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
                AnsiConsole.Clear();
                await selectedAction.Invoke();
            }
            else
            {
                AnsiConsole.Markup("[red]Invalid option.[/]");
            }
        }
    }

    /// <summary>
    /// Present the user with a list of stacks to select to study.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task SelectStackToStudy(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Return");
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select a stack to study: ")
            .PageSize(10)
            .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        var selectedStack = optionsDictionary[choice];
        await StudyStack(selectedStack);
    }

    /// <summary>
    /// Ask the user how many flashcards to study and then present them with flashcards to study.
    /// When the study session is complete, the results are saved to the data store.
    /// </summary>
    /// <param name="selectedStack">The stack to study</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task StudyStack(StackDTO selectedStack)
    {

        var stackId = _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId).Result;
        var flashcards = _stackService.GetFlashCardsByStackId(stackId).Result.ToList();
        if (flashcards.Count == 0)
        {
            AnsiConsole.Markup("[red]No flashcards found in stack.[/]");
            return;
        }

        AnsiConsole.Markup($"[bold]Studing Stack {selectedStack.Name}[/]");
        AnsiConsole.Markup($"[bold]{flashcards.Count} flashcards in stack[/]");
        var numFlashcardsToStudyOptions = new Dictionary<string, int>
        {
            { $"All ({flashcards.Count})", flashcards.Count }
        };

        if (flashcards.Count > 50)
        {
            numFlashcardsToStudyOptions.Add("50", 50);
        }

        if (flashcards.Count > 25)
        {
            numFlashcardsToStudyOptions.Add("25", 25);
        }

        if (flashcards.Count > 10)
        {
            numFlashcardsToStudyOptions.Add("10", 10);
        }

        var numFlashcardsToStudyChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Choose the number of flashcards to study:")
            .AddChoices(numFlashcardsToStudyOptions.Keys));
        var numFlashcardsToStudy = numFlashcardsToStudyOptions[numFlashcardsToStudyChoice];

        var random = new Random();
        var correctCount = 0;
        var incorrectCount = 0;
        var flashcardsToStudy = new List<FlashCardDTO>(flashcards);

        for (int i = 0; i < numFlashcardsToStudy; i++)
        {
            var flashcardIndex = random.Next(flashcardsToStudy.Count);
            var flashcard = flashcardsToStudy[flashcardIndex];
            flashcardsToStudy.RemoveAt(flashcardIndex);

            AnsiConsole.MarkupLine($"[bold]Correct: {correctCount} Incorrect: {incorrectCount} Remaining: {numFlashcardsToStudy - i}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{flashcard.Question}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Press Enter to reveal the answer.[/]");
            AnsiConsole.MarkupLine("[bold]Press Esc to exit the study session.[/]");
            var key = Console.ReadKey(true).Key;
            while (key != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.Escape)
                {
                    return;
                }

                key = Console.ReadKey(true).Key;
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold]Correct: {correctCount} Incorrect: {incorrectCount} Remaining: {numFlashcardsToStudy - i}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{flashcard.Question}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{flashcard.Answer}[/]");
            AnsiConsole.WriteLine();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Did you get the answer correct?").AddChoices("Yes", "No"));
            switch (choice)
            {
                case "Yes":
                    correctCount++;
                    break;
                case "No":
                    incorrectCount++;
                    break;
                default:
                    throw new InvalidOperationException("Invalid choice.");
            }

            AnsiConsole.Clear();
        }

        await _stackService.AddStudySession(DateTime.Now, correctCount, incorrectCount, selectedStack.Name, stackId);
        AnsiConsole.MarkupLine($"[bold]Study Session Complete. Correct: {correctCount} Incorrect: {incorrectCount} Total: {numFlashcardsToStudy}[/]");
        AnsiConsole.MarkupLine("[bold]Press any key to return to the main menu.[/]");
        Console.ReadKey();
    }

    /// <summary>
    /// Present the user with a list of stacks to select to view flashcards.
    /// When a stack is selected, the flashcards are presented in a table.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Present the user with options to add a stack or flashcard.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Presents the user with a list of stacks to select to add flashcards to.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Presents the user with a list of stacks to select to update.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

        var updateOptions = new Dictionary<string, Func<Task>>
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

    /// <summary>
    /// Presents the user with a list of flashcards to select to update.
    /// </summary>
    /// <param name="stackId">The id of the stack to select flashcards to update</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Presents the user with a list of stacks to select to delete the entire stack or individual flashcards from.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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
        options = ["Delete stack", "Delete flashcards", "Return"];
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

    /// <summary>
    /// Prsents the user with a list of flashcards to select to delete.
    /// </summary>
    /// <param name="stackId">The id of the stack to select flashcards to delete</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Presents the user with a list of stacks to select to view study session data.
    /// </summary>
    /// <param name="stacks">The stacks to present</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task ViewStudySessionData(IEnumerable<StackDTO> stacks)
    {
        var optionsDictionary = stacks.ToDictionary(stack => $"{stack.DisplayId} {stack.Name} Flashcards: {stack.FlashCardCount}", stack => stack);
        var options = optionsDictionary.Keys.ToList();
        options.Add("Show data for all stacks");
        options.Add("Return");
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select a stack to view study session data:")
            .PageSize(10)
            .AddChoices(options));

        if (choice == "Return")
        {
            return;
        }

        IEnumerable<StudySessionDTO> studySessions;
        if (choice == "Show data for all stacks")
        {
            studySessions = await _stackService.GetAllStudySessionData();
        }
        else
        {
            var selectedStack = optionsDictionary[choice];
            var stackId = await _stackService.GetStackIdFromDisplayId(selectedStack.DisplayId);
            studySessions = await _stackService.GetStudySessionsByStackId(stackId);
        }

        if (!studySessions.Any())
        {
            AnsiConsole.Markup("[red]No study session data found.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Stack");
        table.AddColumn("Date");
        table.AddColumn("Correct");
        table.AddColumn("Incorrect");

        foreach (var studySession in studySessions)
        {
            table.AddRow(studySession.StackName, studySession.Date.ToString(), studySession.Correct.ToString(), studySession.Incorrect.ToString());
        }

        AnsiConsole.Write(table);
    }
}
