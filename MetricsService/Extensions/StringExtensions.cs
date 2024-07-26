using System.Text;
using System.Text.Json;

namespace MetricsService.Extensions;

/// <summary>
/// String Extensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Convert a string into Dot Notation. Ex: "This is a String." -> "this.is.a.string";
    /// </summary>
    /// <param name="s">Input String</param>
    /// <param name="ensureCamelCases">Make sure final result follows Dot Notation.  For example: "this.Is.A string" -> "this.Is.A.string" when false but "this.is.a.string" when true.</param>
    /// <returns>Output String in Dot-Notation.</returns>
    public static string ToDotNotation(this string s, bool ensureCamelCases = false)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        var split = s.Split(' ');
        string result;
        if (split.Length == 1)
            result = JsonNamingPolicy
                .CamelCase
                .ConvertName(s);
        else
        {
            var sb = new StringBuilder();
            for (var i = 0; i < split.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(split[i])) continue;
                sb.Append(JsonNamingPolicy
                    .CamelCase
                    .ConvertName(split[i]));
                if (i < split.Length - 1) sb.Append('.');
            }
            result = sb.ToString();
        }
        result = result.EndsWith(".")
            ? result[..^1]
            : result;
        return ensureCamelCases
            ? result.Replace(".", " ").ToDotNotation()
            : result;
    }
}