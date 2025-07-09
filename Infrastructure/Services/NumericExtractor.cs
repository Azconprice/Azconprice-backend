using Application.Services;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

/// <summary>
/// Extracts (number, unit) pairs from Azerbaijani text.
/// Recognises “50 mm”, “d=50 mm”, “3 × 4 mm2”, “Ø32mm” …
/// </summary>
public sealed class NumericExtractor : INumericExtractor
{
    // mm² / mm2 first so "mm2" wins over "mm"
    private const string UNIT_RE = @"(mm2|mm|cm|m(?:\(2\)|2)?|ton)";

    /// <remarks>
    ///  1) optional “d=” prefix<br/>
    ///  2) first number (captured)<br/>
    ///  3) optional “×” second number (ignored – backend ignores it)<br/>
    ///  4) unit
    /// </remarks>
    private static readonly Regex Pattern = new(
        $@"(?:\b|d\s*=)\s*
           (\d+(?:[.,]\d+)?)          # 1st num
           (?:\s*[x×]\s*
               (\d+(?:[.,]\d+)?))?    # 2nd num (ignored)
           \s*{UNIT_RE}",
        RegexOptions.IgnoreCase |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    public List<(double Number, string Unit)> Extract(string input)
    {
        var list = new List<(double, string)>();
        if (string.IsNullOrWhiteSpace(input)) return list;

        foreach (Match m in Pattern.Matches(input))
        {
            var num = double.Parse(
                m.Groups[1].Value.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture);

            var unit = m.Groups[3].Value.ToLowerInvariant();   // group-index 3 = UNIT_RE
            list.Add((num, unit));
        }
        return list;
    }
}
