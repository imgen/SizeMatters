using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;
using SizeMatters;

const string settingsFilePath = "settings.json";
if (File.Exists(settingsFilePath) is false)
{
    await Console.Error.WriteLineAsync($"The setting file {settingsFilePath}");
    return;
}
var json = await File.ReadAllTextAsync(settingsFilePath);
var settings = JsonSerializer.Deserialize<Settings>(json);
// ReSharper disable once PossibleNullReferenceException
var tableSizesCsvPath = settings.TableSizesCsvPath;
var tableSizesFromCsv = new List<TableSize>();
if (string.IsNullOrEmpty(tableSizesCsvPath) is not true)
{
    using var reader = new StreamReader(tableSizesCsvPath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    tableSizesFromCsv = await csv.GetRecordsAsync<TableSize>().ToListAsync();
}

var tableSizesQueryResults = await TableSizeQueryer.GetTableSizesAsync(
    tableNames: args,
    settings.DatabaseConnection,
    tableSizesFromCsv
);
if (tableSizesQueryResults is null)
{
    Console.WriteLine($"No table size data provided");
    return;
}
if (tableSizesQueryResults.Any() is false)
{
    Console.WriteLine($"No table match the provided table names");
    return;
}

TableSizeResultsDisplay.Render(tableSizesQueryResults, settings.SizeCategorizations);
    
