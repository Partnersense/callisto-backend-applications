using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Options
{
    /// <summary>
    /// Contains type converters and utilities for handling common configuration conversions
    /// </summary>
    public static class OptionsTypeConvertersExtension
    {

        /// <summary>
        /// Converts a pipe-separated string into a list of strings.
        /// Empty entries between pipe characters are removed.
        /// </summary>
        /// <param name="stringValue">The pipe-separated string to convert. Example: "value1|value2|value3"</param>
        /// <returns>
        /// A list of strings parsed from the input. Returns an empty list if input is null, empty, or whitespace.
        /// Whitespace is preserved in the individual values.
        /// </returns>
        public static List<string> ConvertPipeSeparatedStringList(string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue)
                    ? new List<string>()
                    : stringValue.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Converts a pipe-separated string into a list of integers.
        /// Invalid integer values and empty entries are filtered out.
        /// </summary>
        /// <param name="stringValue">The pipe-separated string of integers to convert. Example: "1|2|3"</param>
        /// <returns>
        /// A list of integers parsed from the input. Returns an empty list if input is null, empty, or whitespace.
        /// Any non-numeric or invalid integer values are filtered out from the result.
        /// </returns>
        public static List<int> ConvertPipeSeparatedIntList(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return new List<int>();

            return stringValue
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var num) ? num : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .ToList();

        }
    }
}
