namespace FilipunkBlog.Application.Models;

/// <summary>
/// Překlad obsahu do jednoho jazyka. <see cref="Summary"/> = Úvod (článek) / Popis (projekt),
/// <see cref="Body"/> = Obsah (článek) / Detail (projekt).
/// </summary>
public class ContentTranslationViewModel
{
    public string Culture { get; set; } = "en";
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Title) &&
        string.IsNullOrWhiteSpace(Summary) &&
        string.IsNullOrWhiteSpace(Body);
}
