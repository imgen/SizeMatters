using System.Collections.Generic;

namespace SizeMatters
{
    public record Settings(string TableSizesCsvPath, 
        string DatabaseConnection,
        Dictionary<long, string> SizeCategorizations);

    public record TableSize(string TableName, long Size);
}