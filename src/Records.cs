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
        long NullSize,
        ColumnStats Stats);

    public record ColumnStats(
        long Max, long Min, long Mean, long Median)
    {
        public override string ToString()
        {
            return $@"Max: {Max}
Min: {Min}
Mean: {Mean}
Median: {Median}";
        }
    }

    public record ColumnSizeQueryResult(object Value, long ValueCount)
    {
        public ColumnSizeQueryResult(): this(default, default) { }
    }

}