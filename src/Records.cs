using System.Collections.Generic;

namespace SizeMatters
{
    public record Settings(string TableSizesCsvPath, 
        string TableSizesApiUrl,
        string DatabaseConnectionString,
        Dictionary<long, string> SizeCategorizations);

    public record TableSize(string TableName, long Size);

    public record ColumnSize(string TableName, 
        string ColumnName, 
        long Size, 
        long NullSize);

    public record ColumnSizeQueryResult(object Value, long ValueCount);
}