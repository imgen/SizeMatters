using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using SizeMatters;
using Spectre.Console;
using static SizeMatters.Statics;
using static SizeMatters.TableSizeQueryer;
using static SizeMatters.TableSizeResultsDisplay;
using static SizeMatters.ColumnSizeQueryer;
using static SizeMatters.ColumnSizeResultsDisplay;
using static SizeMatters.ColumnSizeExporter;

const string executableOfColumnSizeMatters = "cszmas";
const string settingsFilePath = "settings.json";

if (File.Exists(settingsFilePath) is false)
{
    $"The setting file {settingsFilePath} doesn't exist".RenderAsYellowBoldText();
    return;
}
var json = await File.ReadAllTextAsync(settingsFilePath);
// ReSharper disable once PossibleNullReferenceException
var (csvPath, apiUrl, connectionString, columnSizeDetailsJsonExportToDir, sizeCategorizations) = 
    JsonSerializer.Deserialize<Settings>(json)!;
sizeCategorizations ??= DefaultSizeCategorizations;

var executablePath = Environment.GetCommandLineArgs()[0];
var executableName = Path.GetFileNameWithoutExtension(executablePath);
var task = executableName switch
{
    executableOfColumnSizeMatters => RetrieveColumnSizesAsync(),
    _ => RetrieveTableSizesAsync()
};
try
{
    await task;
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, 
        ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
        ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
}

async Task RetrieveTableSizesAsync()
{
    "Table Size Matters".RenderAsAsciiArt();
    var tableSizesFromCsv = new List<TableSize>();
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
        "Cannot retrieve table size data".RenderAsYellowBoldText();
        return;
    }
    if (tableSizesQueryResults.Any() is false)
    {
        "No table match the provided table names".RenderAsYellowBoldText();
        return;
    }

    Render(tableSizesQueryResults, sizeCategorizations);
}

async Task RetrieveColumnSizesAsync()
{
    "Column Size Matters".RenderAsAsciiArt();
    if (args.Length == 0)
    {
        "Please provide at least one column names prefixed by table name, such as PurchaseOrder.OrderNumber. Schema such as dbo.PurchaseOrder.OrderNumber is supported"
            .RenderAsYellowBoldText();
        return;
    }
    if (string.IsNullOrEmpty(connectionString))
    {
        "Please provide database connection string".RenderAsYellowBoldText();
        return;
    }

    var columnSizes = await GetColumnSizesAsync(
        columnExpressions: args,
        connectionString
    );
    if (columnSizes?.Any() is not true)
    {
        "Couldn't retrieve column size information".RenderAsYellowBoldText();
        return;
    }
    
    Render(columnSizes, sizeCategorizations);

    if (string.IsNullOrEmpty(columnSizeDetailsJsonExportToDir) is not true)
    {
        if (Directory.Exists(columnSizeDetailsJsonExportToDir) is not true)
        {
            $"The provided dir {columnSizeDetailsJsonExportToDir} doesn't exist. Creating it now".RenderAsYellowBoldText();
            try
            {
                Directory.CreateDirectory(columnSizeDetailsJsonExportToDir);
            }
            catch (Exception e)
            {
                $"Cannot create directory {columnSizeDetailsJsonExportToDir} due to error: {e}. Exiting now".RenderAsYellowBoldText();
                return;
            }
        }

        await ExportToJsonAsync(columnSizes, columnSizeDetailsJsonExportToDir);
    }
}
