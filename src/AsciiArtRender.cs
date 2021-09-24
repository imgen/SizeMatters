using Spectre.Console;

namespace SizeMatters
{
    public static class AsciiArtRender
    {
        public static void RenderAsAsciiArt(this string asciiArt)
        {
            AnsiConsole.Render(new FigletText(asciiArt)
                .Centered()
                .Color(Color.Green)
            );
        }
    }
}