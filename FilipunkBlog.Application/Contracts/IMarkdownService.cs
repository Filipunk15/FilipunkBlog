namespace FilipunkBlog.Application.Contracts;

public interface IMarkdownService
{
    /// <summary>Převede Markdown na HTML. Prázdný/null vstup → prázdný řetězec.</summary>
    string ToHtml(string? markdown);
}
