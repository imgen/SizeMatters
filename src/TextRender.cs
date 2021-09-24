using System;
using Spectre.Console;

namespace SizeMatters
{
    public static class TextRender
    {
        public static void RenderAsAsciiArt(this string asciiArt)
        {
            AnsiConsole.Render(new FigletText(asciiArt)
                .Centered()
                .Color(Color.Green)
            );
        }

        public static void RenderAsColoredText(this string text, Color color)
        {
            AnsiConsole.MarkupLine($"[{color.ToString().ToLowerInvariant()}]{text}[/]");
            Console.WriteLine();
        }

        public static void RenderAsYellowBoldText(this string text)
        {
            AnsiConsole.MarkupLine($"[bold yellow]{text}[/]");
        }
    }
}