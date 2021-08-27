using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Spectre.Console;
using static SizeMatters.Statics;

namespace SizeMatters
{
    public static class ColumnSizeResultsDisplay
    {
        public static void Render(List<ColumnSize> columnSizes,
            Dictionary<long, string> sizeCategorizations)
        {
            columnSizes = columnSizes.OrderByDescending(x => x.Size).ToList();
            
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("TableName").Centered());
            table.AddColumn(new TableColumn("ColumnName").Centered());
            table.AddColumn("Size");
            table.AddColumn("Formatted Size");
            table.AddColumn("Readable Size");
            table.AddColumn(new TableColumn("Size Category").Centered());
            table.AddColumn("Null Size");
            table.AddColumn("Formatted Null Size");
            table.AddColumn("Readable Null Size");
            table.AddColumn(new TableColumn("Null Size Category").Centered());

            foreach (var (tableName, columnName, size, nullSize) in columnSizes)
            {
                table.AddRow(
                    tableName,
                    columnName,
                    size.ToString(),
                    $"{size:n0}",
                    Nicer.Nice(size, 3),
                    GetSizeCategory(size, sizeCategorizations),
                    nullSize.ToString(),
                    $"{nullSize:n0}",
                    Nicer.Nice(nullSize, 3),
                    GetSizeCategory(nullSize, sizeCategorizations)
                );
            }

            if (columnSizes.Count > 1)
            {
                table.AddEmptyRow();

                var totalSize = columnSizes.Sum(x => x.Size);
                table.AddRow("Total Column Size Sum",
                    "",
                    totalSize.ToString(),
                    $"{totalSize:n0}",
                    Nicer.Nice(totalSize, 3),
                    GetSizeCategory(totalSize, sizeCategorizations)
                );
                
                var product = new BigInteger(columnSizes[0].Size);
                for (int i = 1; i < columnSizes.Count; i++)
                {
                    product *= columnSizes[i].Size;
                }
                
                table.AddEmptyRow();
                table.AddRow("Total Column Size Product",
                    "",
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