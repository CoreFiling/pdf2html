using System.Text.RegularExpressions;

namespace Pdf2Html.Settings;

public class ConversionOptions(IConfiguration configuration)
{
    public string CommandLineArguments { get; } = ToCommandLineArguments(configuration.GetSection("ConversionOptions").AsEnumerable());

    internal static string ToCommandLineArguments(IEnumerable<KeyValuePair<string, string?>> options) =>
        string.Join(' ', options.Where(kvp => kvp.Value != null).Select(kvp => $"--{ToKebabCase(kvp.Key.Replace("ConversionOptions:", ""))}={ValueToString(kvp.Value!)}"));

    private static string ValueToString(string value) => bool.TryParse(value, out var boolValue) ? (boolValue ? "1" : "0") : value;

    private static string ToKebabCase(string value) =>
        Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", "-$1", RegexOptions.Compiled).Trim().ToLower();
}