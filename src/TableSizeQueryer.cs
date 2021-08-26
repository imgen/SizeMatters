using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SizeMatters
{
    public class TableSizeQueryer
    {
        public static async Task<List<TableSize>> GetTableSizesAsync(string[] tableNames,
            string connectionString,
            List<TableSize> fallbackTableSizes)
        {
            var fallbackProvided = fallbackTableSizes.Any();
            var dbProvided = string.IsNullOrWhiteSpace(connectionString) is not true;
            var tableSizes = (fallbackProvided, dbProvided) switch
            {
                (false, false) => throw new ArgumentException($"Either CSV file path or an api url or a connection string should be provided"),
                (true, false) => fallbackTableSizes,
                _  => await GetTableSizesFromDb()
            };

            if (tableSizes == null || tableSizes.Any() is false)
            {
                tableSizes = fallbackTableSizes;
            }

            if (tableSizes.Any() is false)
            {
                return null;
            }

            if (tableNames.Any() is false)
            {
                return tableSizes;
            }

            return tableSizes
                .Where(x => tableNames.Any() is false ||
                            tableNames.Any(
                                tableName =>
                                    x.TableName.Contains(tableName, StringComparison.InvariantCultureIgnoreCase)
                                )
                ).ToList();
            
            async Task<List<TableSize>> GetTableSizesFromDb()
            {
                try
                {
                    var connection = new SqlConnection(connectionString);
                    return (await connection.QueryAsync<TableSize>(TableSizeQuery))
                        .ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cannot connect to the provided database or the user doesn't have sufficient privileges. Error: {e}");
                    Console.WriteLine(fallbackProvided
                        ? "Fallback to CSV file or API response"
                        : "Cannot get any table size data");
                    return null;
                }
            }
        }

        private const string TableSizeQuery = @"
SELECT
      QUOTENAME(SCHEMA_NAME(sysobj.schema_id)) + '.' + QUOTENAME(sysobj.name) AS [TableName], 
      SUM(sysptn.Rows) AS [Size]
FROM 
      sys.objects AS sysobj
      INNER JOIN sys.partitions AS sysptn
            ON sysobj.object_id = sysptn.object_id
WHERE
      sysobj.type = 'U'
      AND sysobj.is_ms_shipped = 0x0
      AND index_id < 2 -- 0:Heap, 1:Clustered
GROUP BY 
      sysobj.schema_id
      , sysobj.name
ORDER BY [Size] DESC;
";
    }
}