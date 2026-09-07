using System.Globalization;

namespace FilipunkBlog.Infrastructure;

/// <summary>Aktuální jazyk požadavku (z URL prefixu / cookie přes RequestLocalization).</summary>
internal static class CultureContext
{
    public static string Current => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    public static bool IsEnglish => Current == "en";
}
