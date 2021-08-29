using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace SizeMatters
{
    public static class Statics
    {
        public static readonly Dictionary<long, string>
            DefaultSizeCategorizations = new ()
            {
                [10] = "Tiny",
                [1000] = "Small",
                [100_000] = "Medium",
                [1000_000] = "Large",
                [1000_000_000] = "Huge",
                [1000_000_000_000] = "Humongous",
                [long.MaxValue / 2] = "Gigantic"
            };
        
        public static string GetSizeCategory(long size,
            Dictionary<long, string> sizeCategorizations)
        {
            if (size <= 0)
            {
                return "Empty";
            }
            var orderedSizes = sizeCategorizations
                .Keys.OrderBy(x => x).ToArray();
            int i = 0;
                
            while (i < orderedSizes.Length && 
                   size >= orderedSizes[i])
            {
                i++;
            }

            return i == orderedSizes.Length ? 
                "More than Gigantic" : 
                sizeCategorizations[orderedSizes[i]];
        }

        public static void AddEmptyRow(this Table table)
        {
            table.AddRow(Array.Empty<string>());
        }

        public static string EscapeBrackets(this string str)
        {
            return str?.Replace("[", "[[")
                ?.Replace("]", "]]");
        }
    }
}