using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FilipunkBlog.Application;

/// <summary>
/// Vytváří URL slug z libovolného textu – odstraní diakritiku, převede na malá písmena
/// a nahradí nealfanumerické znaky pomlčkou.
/// </summary>
public static partial class SlugHelper
{
    public static string Slugify(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var decomposed = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder(decomposed.Length);
        foreach (var ch in decomposed)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(ch))
            {
                case UnicodeCategory.NonSpacingMark:
                    continue;
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    sb.Append(ch);
                    break;
                default:
                    sb.Append('-');
                    break;
            }
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC);
        return MultiDashRegex().Replace(slug, "-").Trim('-');
    }

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultiDashRegex();
}
