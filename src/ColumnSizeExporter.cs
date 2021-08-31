using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace SizeMatters
{
    public static class ColumnSizeExporter
    {
        public static async Task ExportToJsonAsync(List<ColumnSize> columnSizes, string dir)
        {
            var filePath = Path.Combine(dir, "columnSizes.json");
            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, columnSizes);
        }
    }
}