namespace SizeMatters;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

public static class ColumnSizeQueryer
{
    private const string ColumnSizeSql = @"
SELECT
@columnName AS [Value],
COUNT(*) AS [ValueCount]
FROM @tableName
GROUP BY @columnName";
        
    public static async Task<List<ColumnSize>> GetColumnSizesAsync(
        string[] columnExpressions,
        string connectionString)
    {
        var columnSizes = await FetchColumnSizesAsync(
                columnExpressions, connectionString
            ).ToListAsync();
        return columnSizes;
    }

    private const char ColumnExpressionSeparator = '.';
    private static async IAsyncEnumerable<ColumnSize> FetchColumnSizesAsync(
        string[] columnExpressions,
        string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);
            
        foreach (var columnExpression in columnExpressions)
        {
            var parts = columnExpression.Split(ColumnExpressionSeparator);
            var partsCount = parts.Length;
            if (partsCount is 1 or > 3)
            {
                $"The provided column expression {columnExpression} is invalid".RenderAsYellowBoldText();
                continue;
            }

            var columnName = parts.Last();
            var tableName = string.Join(ColumnExpressionSeparator, parts.Take(partsCount - 1));
            var sql = ColumnSizeSql.Replace("@columnName", columnName)
                .Replace("@tableName", tableName);
            List<ColumnSizeQueryResult> results;
            try
            {
                results = (await connection.QueryAsync<ColumnSizeQueryResult>(sql)).ToList();
            }
            catch (Exception e)
            {
                $"Cannot retrieve column size data for {columnExpression} due to {e}".RenderAsYellowBoldText();
                continue;
            }
            if (results.Count == 0)
            {
                $"The column {columnExpression} is empty".RenderAsYellowBoldText();
                continue;
            }

            var size = results.Count;
            var nullSize = results.FirstOrDefault(x => x.Value is null)
                ?.ValueCount?? 0;
            var nonNullValueSizes = results.Where(x => x.Value is not null)
                .Select(x => x.ValueCount)
                .ToArray();
            var stats = new ColumnStats(
                    nonNullValueSizes.Max(),
                    nonNullValueSizes.Min(),
                    (long)nonNullValueSizes.Average(),
                    nonNullValueSizes.GetMedian(cloneArray: false)
                );
            yield return new ColumnSize(tableName, 
                columnName, 
                size, 
                nullSize, 
                stats,
                results);
        }
    }
}
