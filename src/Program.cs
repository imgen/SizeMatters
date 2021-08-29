using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using CsvHelper;
using SizeMatters;
using static SizeMatters.TableSizeQueryer;
using static SizeMatters.ColumnSizeQueryer;
using static SizeMatters.TableSizeResultsDisplay;
using static SizeMatters.ColumnSizeResultsDisplay;
using static System.Console;
using static SizeMatters.Statics;

var executablePath = Environment.GetCommandLineArgs()[0];
var executableName = Path.GetFileNameWithoutExtension(executablePath);

const string executableOfColumnSizeMatters = "cszmas";

const string settingsFilePath = "settings.json";
if (File.Exists(settingsFilePath) is false)
{
    await Error.WriteLineAsync($"The setting file {settingsFilePath}");
    return;
}
var json = await File.ReadAllTextAsync(settingsFilePath);
// ReSharper disable once PossibleNullReferenceException
var (csvPath, apiUrl, connectionString, sizeCategorizations) = 
    JsonSerializer.Deserialize<Settings>(json);
var tableSizesFromCsv = new List<TableSize>();
if (executableName == executableOfColumnSizeMatters)
{
    if (args.Length == 0)
    {
        Error.WriteLine("Please provide at least one column names prefixed by table name, such as PurchaseOrder.OrderNumber. Schema is supported");
        return;
    }
    if (string.IsNullOrEmpty(connectionString))
    {
        Error.WriteLine($"Please provide database connection string");
        return;
    }

    var columnSizes = await GetColumnSizesAsync(
        columnExpressions: args,
        connectionString
    );
    if (columnSizes?.Any() is not true)
    {
        Error.WriteLine("Couldn't retrieve column size information");
        return;
    }
    
    Render(columnSizes, sizeCategorizations?? DefaultSizeCategorizations);
    return;
}

if (string.IsNullOrEmpty(csvPath) is not true)
{
    using var reader = new StreamReader(csvPath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    tableSizesFromCsv = await csv.GetRecordsAsync<TableSize>().ToListAsync();
}

var tableSizesFromApi = new List<TableSize>();
if (!string.IsNullOrEmpty(apiUrl) && tableSizesFromCsv.Count == 0)
{
    using var httpClient = new HttpClient();
    tableSizesFromApi = await httpClient.GetFromJsonAsync<List<TableSize>>(apiUrl);
}

var tableSizesQueryResults = await GetTableSizesAsync(
    tableNames: args,
    connectionString,
    tableSizesFromCsv.Count == 0?
        tableSizesFromApi : tableSizesFromCsv
);
if (tableSizesQueryResults is null)
{
    Error.WriteLine("Cannot retrieve table size data");
    return;
}
if (tableSizesQueryResults.Any() is false)
{
    WriteLine($"No table match the provided table names");
    return;
}

Render(tableSizesQueryResults, sizeCategorizations?? DefaultSizeCategorizations);
    
