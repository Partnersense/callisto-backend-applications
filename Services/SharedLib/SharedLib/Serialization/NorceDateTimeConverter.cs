using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Serilog;

namespace SharedLib.Serialization
{
    /// <summary>
    /// Custom JsonConverter for handling Norce's specific date format
    /// </summary>
    public class NorceDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var dateStr = reader.GetString();
            if (string.IsNullOrEmpty(dateStr))
                return null;

            // Handle "/Date(1731440148857+0000)/" format
            if (dateStr.StartsWith("/Date(") && dateStr.EndsWith(")/"))
            {
                try
                {
                    var dateContent = dateStr.Substring(6, dateStr.Length - 8); // Remove "/Date(" and ")/"
                    var plusIndex = dateContent.IndexOf('+');

                    // If there's no "+" or it's at an invalid position, log and return null
                    if (plusIndex <= 0)
                    {
                        Log.Logger.Warning("Invalid date format - missing or misplaced timezone offset: {dateStr}", dateStr);
                        return null;
                    }

                    var ticksPart = dateContent.Substring(0, plusIndex);

                    if (long.TryParse(ticksPart, out long milliseconds))
                    {
                        try
                        {
                            // Convert Unix timestamp (milliseconds) to UTC DateTime
                            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Warning(ex, "Failed to convert Unix timestamp to DateTime: {dateStr}", dateStr);
                            return null;
                        }
                    }
                    else
                    {
                        Log.Logger.Warning("Failed to parse milliseconds from date string: {dateStr}", dateStr);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Warning(ex, "Failed to parse Norce date format: {dateStr}", dateStr);
                    return null;
                }
            }

            // Try parsing as ISO date
            if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime result))
                return DateTime.SpecifyKind(result, DateTimeKind.Utc);

            Log.Logger.Warning("Failed to parse date string: {dateStr}", dateStr);
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.Value.ToString("O")); // ISO 8601 format
        }
    }
}
