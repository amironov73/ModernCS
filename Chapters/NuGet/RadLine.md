### RadLine

[RadLine](https://github.com/spectreconsole/radline) -- библиотека для продвинутого консольного ввода от создателей Spectre.Console.

Демонстрация возможностей библиотеки:

```c#
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RadLine;
using Spectre.Console;

static class Program
{
    public static async Task Main()
    {
        if (!LineEditor.IsSupported (AnsiConsole.Console))
        {
            Console.WriteLine ("The terminal does not support ANSI codes,"
                + " or it isn't a terminal.");
        }

        var editor = new LineEditor()
        {
            MultiLine = true,
            Text = "Hello, and welcome to RadLine!\n" 
                   + "Press SHIFT+ENTER to insert a new line\n"
                   + "Press ENTER to submit",
            Prompt = new LineNumberPrompt (new Style (foreground: Color.Yellow, 
                background: Color.Black)),
            Completion = new TestCompletion(),
            Highlighter = new WordHighlighter()
                .AddWord ("git", new Style (foreground: Color.Yellow))
                .AddWord ("code", new Style (foreground: Color.Yellow))
                .AddWord ("vim", new Style (foreground: Color.Yellow))
                .AddWord ("init", new Style (foreground: Color.Blue))
                .AddWord ("push", new Style (foreground: Color.Red))
                .AddWord ("commit", new Style (foreground: Color.Blue))
                .AddWord ("rebase", new Style (foreground: Color.Red))
                .AddWord ("Hello", new Style (foreground: Color.Blue))
                .AddWord ("SHIFT", new Style (foreground: Color.Grey))
                .AddWord ("ENTER", new Style (foreground: Color.Grey))
                .AddWord ("RadLine", new Style (foreground: Color.Yellow, 
                    decoration: Decoration.SlowBlink)),
        };

        // Add some history
        editor.History.Add ("foo\nbar\nbaz");
        editor.History.Add ("bar");
        editor.History.Add ("Spectre.Console");
        editor.History.Add ("Whaaat?");

        // Add custom commands
        editor.KeyBindings.Add<InsertSmiley>
        (
            ConsoleKey.I,
            ConsoleModifiers.Control
        );

        // Read a line (or many)
        var result = await editor.ReadLine (CancellationToken.None);

        // Write the buffer
        AnsiConsole.WriteLine();
        AnsiConsole.Render(new Panel (result.EscapeMarkup())
            .Header ("[yellow]Text:[/]")
            .RoundedBorder());
    }

    public sealed class InsertSmiley : LineEditorCommand
    {
        public override void Execute
            (
                LineEditorContext context
            )
        {
            context.Buffer.Insert (":-)");
        }
    }

    public sealed class TestCompletion : ITextCompletion
    {
        public IEnumerable<string>? GetCompletions
            (
                string context,
                string word,
                string suffix
            )
        {
            if (string.IsNullOrWhiteSpace (context))
            {
                return new[] { "git", "code", "vim" };
            }

            if (context.Equals ("git ", StringComparison.Ordinal))
            {
                return new[] { "init", "branch", "push", "commit", "rebase" };
            }

            return null;
        }
    }
}
```
