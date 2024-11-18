using System.Globalization;
using System.Text.RegularExpressions;

namespace SharedLib.Util;
public static class FormatUtil
{
    public static string ToCamelCase(string originalString)
    {
        var lowercaseString = originalString.ToLower();

        var cleanedString = Regex.Replace(lowercaseString, @"[^\w\s]", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        var words = cleanedString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        for (var i = 1; i < words.Length; i++)
        {
            words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
        }

        var camelCaseString = string.Join("", words);

        return camelCaseString;
    }
}
