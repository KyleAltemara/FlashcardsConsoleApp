using FlashcardsConsoleApp.DataAccess.Services;
using Spectre.Console;
using System.Text.Json;

namespace FlashcardsConsoleApp.DemoStacks;

public static class DemoImporter
{
    /// <summary>
    /// Import demo stacks from DemoStacks.json
    /// </summary>
    /// <param name="stackService"> The stack service used to add stacks and flashcards. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public static async Task ImportDemoStacks(IStackService stackService)
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

            var stackId = await stackService.GetStackIdFromDisplayId(stack.DisplayId);
            foreach (var card in jsonStack.Cards)
            {
                await stackService.AddFlashCardToStackAsync(stackId, card.Question, card.Answer);
            }
        }
    }

    /// <summary>
    /// Creates 5 years of random study session data for each stack.
    /// </summary>
    /// <param name="stackService"> The stack service used to add and store study sessions. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    internal static async Task GenerateStudySessions(IStackService stackService)
    {
        var random = new Random();
        var stacks = await stackService.GetAllStacksAsync();
        foreach (var stack in stacks)
        {
            var stackId = await stackService.GetStackIdFromDisplayId(stack.DisplayId);
            var flashcardsCount = (await stackService.GetFlashCardsByStackId(stackId)).Count();
            var numFlashcardsToStudyOptions = new List<int> { flashcardsCount };

            if (flashcardsCount > 50)
            {
                numFlashcardsToStudyOptions.Add(50);
            }

            if (flashcardsCount > 25)
            {
                numFlashcardsToStudyOptions.Add(25);
            }

            if (flashcardsCount > 10)
            {
                numFlashcardsToStudyOptions.Add(10);
            }

            const int yearsOfData = 5;
            for (int i = 0; i < yearsOfData; i++)
            {
                var numSessions = random.Next(5, 15);
                for (int j = 0; j < numSessions; j++)
                {
                    var numFlashcardsToStudy = numFlashcardsToStudyOptions[random.Next(0, numFlashcardsToStudyOptions.Count)];
                    var correctCount = random.Next(0, numFlashcardsToStudy + 1);
                    var incorrectCount = numFlashcardsToStudy - correctCount;

                    var currentDate = DateTime.Now;
                    var minDate = currentDate.AddYears(-i);
                    var maxDate = currentDate.AddYears(-(i + 1));
                    var timeSpan = maxDate - minDate;
                    var randomTimeSpan = new TimeSpan((long)(random.NextDouble() * timeSpan.Ticks));
                    var randomDateTime = minDate + randomTimeSpan;

                    await stackService.AddStudySession(randomDateTime, correctCount, incorrectCount, stack.Name, stackId);
                }
            }
        }
    }
}
