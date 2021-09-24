namespace SizeMatters;

using System.IO;
using System.Text.Json;

public static class ColumnSizeExporter
{
    public static async Task ExportToJsonAsync(List<ColumnSize> columnSizes, string dir)
    {
        var filePath = Path.Combine(dir, "columnSizes.json");
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, columnSizes);
    }
}