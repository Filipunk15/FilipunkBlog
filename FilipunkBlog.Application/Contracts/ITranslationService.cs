namespace FilipunkBlog.Application.Contracts;

/// <summary>
/// Strojový překlad textů z češtiny do angličtiny. Používá se pouze jako pomoc adminovi
/// při vyplňování překladů – nikdy neukládá nic automaticky.
/// </summary>
public interface ITranslationService
{
    /// <summary>Je nakonfigurován API klíč? Když ne, tlačítko „Přeložit“ se v adminu nezobrazí.</summary>
    bool IsConfigured { get; }

    /// <summary>Přeloží jeden text z CS do EN. Prázdný vstup vrací prázdný výstup.</summary>
    Task<string> TranslateAsync(string text, CancellationToken ct = default);

    /// <summary>Přeloží více textů najednou (zachová pořadí). Prázdné položky zůstávají prázdné.</summary>
    Task<IReadOnlyList<string>> TranslateManyAsync(IReadOnlyList<string> texts, CancellationToken ct = default);
}
