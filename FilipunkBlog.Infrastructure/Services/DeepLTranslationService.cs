using System.Net.Http.Headers;
using System.Text.Json;
using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FilipunkBlog.Infrastructure.Services;

/// <summary>Překlad přes DeepL API. Bez klíče je <see cref="IsConfigured"/> false a metody vrací vstup beze změny.</summary>
public class DeepLTranslationService(
    HttpClient httpClient,
    IOptions<DeepLOptions> options,
    ILogger<DeepLTranslationService> logger) : ITranslationService
{
    private readonly DeepLOptions _options = options.Value;

    public bool IsConfigured => _options.IsConfigured;

    public async Task<string> TranslateAsync(string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var result = await TranslateManyAsync([text], ct);
        return result.Count > 0 ? result[0] : text;
    }

    public async Task<IReadOnlyList<string>> TranslateManyAsync(IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        if (!IsConfigured || texts.Count == 0)
            return texts;

        // Prázdné položky nepřekládáme, ale držíme pozice.
        var indexed = texts.Select((t, i) => (t, i)).Where(x => !string.IsNullOrWhiteSpace(x.t)).ToList();
        if (indexed.Count == 0) return texts;

        var form = new List<KeyValuePair<string, string>>
        {
            new("target_lang", "EN"),
            new("source_lang", "CS"),
            new("tag_handling", "html"),
        };
        form.AddRange(indexed.Select(x => new KeyValuePair<string, string>("text", x.t)));

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint)
        {
            Content = new FormUrlEncodedContent(form),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", _options.AuthKey!.Trim());

        using var response = await httpClient.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("DeepL překlad selhal: {Status}", response.StatusCode);
            return texts;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var translations = doc.RootElement.GetProperty("translations");

        var output = texts.ToArray();
        for (var k = 0; k < indexed.Count && k < translations.GetArrayLength(); k++)
            output[indexed[k].i] = translations[k].GetProperty("text").GetString() ?? indexed[k].t;

        return output;
    }
}
