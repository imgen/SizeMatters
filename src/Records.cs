using System.Collections.Generic;

namespace SizeMatters
{
    public record Settings(string TableSizesCsvPath, 
        string TableSizesApiUrl,
        string DatabaseConnectionString,
        string ColumnSizeDetailsJsonExportToDir,
        Dictionary<long, string> SizeCategorizations);
    
    public record TableSize(string TableName, long Size);
    
    public record ColumnStats(
        long Max, long Min, long Mean, long Median)
    {
        public override string ToString()
        {
            return
$@"Max: {Max}
Min: {Min}
Mean: {Mean}
Median: {Median}";
        }
    }

    public record ColumnSizeQueryResult(object? Value, long ValueCount)
    {
        // This constructor is for json deserialization to work
        // ReSharper disable once UnusedMember.Global
        public ColumnSizeQueryResult(): this(default, default) { }
    }

    public record ColumnSize(string TableName, 
        string ColumnName, 
        long Size, 
        long NullSize,
        ColumnStats Stats,
        List<ColumnSizeQueryResult> Details);
}