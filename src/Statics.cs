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

        public static string? EscapeBrackets(this string? str)
        {
            return str?.Replace("[", "[[").Replace("]", "]]");
        }

        /// <summary>
        /// Gets the median value from an array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="sourceArray">The source array</param>
        /// <param name="cloneArray">If it doesn't matter if the source array is sorted, you can pass false to improve performance</param>
        /// <returns></returns>
        public static T GetMedian<T>(this T[] sourceArray, bool cloneArray = true) where T : IComparable<T>
        {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceArray == null || sourceArray.Length == 0)
                throw new ArgumentException("Median of empty array not defined.");

            //make sure the list is sorted, but use a new array
            T[] sortedArray = cloneArray ? (T[])sourceArray.Clone() : sourceArray;
            Array.Sort(sortedArray);

            //get the median
            var size = sortedArray.Length;
            var mid = size / 2;
            if (size % 2 != 0)
                return sortedArray[mid];

            dynamic value1 = sortedArray[mid];
            dynamic value2 = sortedArray[mid - 1];
            return (value1 + value2) * 0.5;
        }
    }
}