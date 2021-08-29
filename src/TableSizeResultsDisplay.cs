using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using System.Numerics;
using static SizeMatters.Statics;

namespace SizeMatters
{
    public static class TableSizeResultsDisplay
    {
        public static void Render(List<TableSize> tableSizes,
            Dictionary<long, string> sizeCategorizations)
        {
            tableSizes = tableSizes.OrderByDescending(x => x.Size).ToList();
            
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("Name").Centered());
            table.AddColumn("Size");
            table.AddColumn("Formatted Size");
            table.AddColumn("Readable Size");
            table.AddColumn(new TableColumn("Size Category").Centered());

            foreach (var (tableName, size) in tableSizes)
            {
                table.AddRow(tableName.EscapeBrackets(),
                    size.ToString(),
                    $"{size:n0}",
                    Nicer.Nice(size, 3),
                    GetSizeCategory(size, sizeCategorizations)
                );
            }

            if (tableSizes.Count > 1)
            {
                table.AddEmptyRow();

                var totalSize = tableSizes.Sum(x => x.Size);
                table.AddRow("Total Size",
                    totalSize.ToString(),
                    $"{totalSize:n0}",
                    Nicer.Nice(totalSize, 3),
                    GetSizeCategory(totalSize, sizeCategorizations)
                );
                
                var product = new BigInteger(tableSizes[0].Size);
                for (int i = 1; i < tableSizes.Count; i++)
                {
                    var tableSize = tableSizes[i].Size;
                    if (tableSize == 0)
                    {
                        continue;
                    }
                    product *= tableSize;
                }

                table.AddEmptyRow();
                table.AddRow("Total Product",
                    product.ToString(),
                    $"{product:n0}",
                    "N/A",
                    "N/A"
                );
            }

            table.Expand().Centered();
            AnsiConsole.Render(table);
        }
    }
}