using FlashcardsConsoleApp.DataAccess.Services;
using Spectre.Console;
using System.Text.Json;

namespace FlashcardsConsoleApp.DemoStacks;

public static class DemoImporter
{
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
}
