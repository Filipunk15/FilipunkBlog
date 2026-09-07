namespace FilipunkBlog.Infrastructure.Options;

public class DeepLOptions
{
    public const string SectionName = "DeepL";

    /// <summary>API klíč z user-secrets / proměnných prostředí (<c>DeepL:AuthKey</c>). Klíč končící na <c>:fx</c> je zdarma.</summary>
    public string? AuthKey { get; set; }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(AuthKey);

    /// <summary>Free klíče (<c>:fx</c>) míří na api-free.deepl.com, placené na api.deepl.com.</summary>
    public string Endpoint => AuthKey?.TrimEnd().EndsWith(":fx", StringComparison.OrdinalIgnoreCase) == true
        ? "https://api-free.deepl.com/v2/translate"
        : "https://api.deepl.com/v2/translate";
}
