using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using System.Numerics;

namespace SizeMatters
{
    public static class TableSizeResultsDisplay
    {
        private static readonly Dictionary<long, string>
            DefaultSizeCategorizations = new ()
            {
                [10] = "Tiny",
                [1000] = "Small",
                [100_000] = "Medium",
                [1000_000] = "Large",
                [1000_000_000] = "Huge",
                [1000_000_000_000] = "Humongous",
                [long.MaxValue / 2] = "Gigantic"
            };

        public static void Render(List<TableSize> tableSizes,
            Dictionary<long, string> sizeCategorizations)
        {
            tableSizes = tableSizes.OrderByDescending(x => x.Size).ToList();
            sizeCategorizations ??= DefaultSizeCategorizations;
            var orderedSizes = sizeCategorizations
                .Keys.OrderBy(x => x).ToArray();
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("Name").Centered());
            table.AddColumn("Size");
            table.AddColumn("Formatted Size");
            table.AddColumn("Readable Size");
            table.AddColumn(new TableColumn("Size Category").Centered());

            foreach (var (tableName, size) in tableSizes)
            {
                table.AddRow(tableName,
                    size.ToString(),
                    $"{size:n0}",
                    Nicer.Nice(size, 3),
                    GetSizeCategory(size)
                );
            }

            if (tableSizes.Count > 1)
            {
                table.AddRow("", "", "", "", "");

                var totalSize = tableSizes.Sum(x => x.Size);
                table.AddRow("Total Size",
                    totalSize.ToString(),
                    $"{totalSize:n0}",
                    Nicer.Nice(totalSize, 3),
                    GetSizeCategory(totalSize)
                );
                
                var product = new BigInteger(tableSizes[0].Size);
                for (int i = 1; i < tableSizes.Count; i++)
                {
                    product *= tableSizes[i].Size;
                }
                
                table.AddRow("", "", "", "", "");
                table.AddRow("Total Product",
                    product.ToString(),
                    $"{product:n0}",
                    "N/A",
                    "N/A"
                );
            }

            table.Expand().Centered();
            AnsiConsole.Render(table);

            string GetSizeCategory(long tableSize)
            {
                int i = 0;
                
                while (i < orderedSizes.Length && 
                       tableSize >= orderedSizes[i])
                {
                    i++;
                }

                return i == orderedSizes.Length ? 
                    "More than Gigantic" : 
                    sizeCategorizations[orderedSizes[i]];
            }
        }
    }
}