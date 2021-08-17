using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SizeMatters
{
    class Program
    {
        private const string SettingsFilePath = "settings.json";
        static async Task Main(string[] args)
        {
            if (File.Exists(SettingsFilePath) is false)
            {
                Console.Error.WriteLine($"The setting file {SettingsFilePath}");
            }
            var json = await File.ReadAllTextAsync(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<Settings>(json);
            var tableSizesCsvPath = settings.TableSizesCsvPath;
        }
    }
}
