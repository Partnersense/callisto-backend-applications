using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Parses a delimited string of market-specific URLs into key-value pairs.
        /// Format: "en-US;www.example.com/en/path|sv-SE;www.example.com/sv/path"
        /// </summary>
        /// <param name="urlString">The delimited string containing market URLs</param>
        /// <param name="traceId">Optional trace ID for logging and debugging purposes</param>
        /// <returns>A Dictionary with market codes as keys and URLs as values</returns>
        /// <exception cref="ArgumentException">Thrown when input string is malformed</exception>
        /// <example>
        /// Input: "en-US;www.example.com/en/path|sv-SE;www.example.com/sv/path"
        /// Output: Dictionary { {"en-US", "www.example.com/en/path"}, {"sv-SE", "www.example.com/sv/path"} }
        /// </example>
        public static Dictionary<string, string> ConvertPipeAndQuotesListToDictionary(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString))
                return new Dictionary<string, string>();

            var marketUrls = new Dictionary<string, string>();

            // Split by market segments
            var marketSegments = urlString.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in marketSegments)
            {
                // Split each segment into market code and URL
                var parts = segment.Split(';', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    throw new ArgumentException($"Invalid market URL format in segment: {segment}");

                var marketCode = parts[0].Trim();
                var url = parts[1].Trim();

                // Validate market code format (e.g., en-US)
                if (!IsValidMarketCode(marketCode))
                    throw new ArgumentException($"Invalid market code format: {marketCode}");

                marketUrls[marketCode] = url;
            }

            return marketUrls;
        }

        /// <summary>
        /// Validates the format of a market code.
        /// Valid format: two lowercase letters, followed by hyphen and two uppercase letters (e.g., en-US)
        /// </summary>
        /// <param name="marketCode">The market code to validate</param>
        /// <returns>True if the market code is valid, false otherwise</returns>
        private static bool IsValidMarketCode(string marketCode)
        {
            return Regex.IsMatch(marketCode, @"^[a-z]{2}-[A-Z]{2}$");
        }
    }
}
