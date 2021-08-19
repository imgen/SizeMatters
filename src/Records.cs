namespace SizeMatters
{
    public record Settings(string TableSizesCsvPath, string DatabaseConnection);

    public record TableSize(string TableName, int Size);
}