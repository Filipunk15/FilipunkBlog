using FilipunkBlog.Application.Contracts;
using Markdig;

namespace FilipunkBlog.Application;

public class MarkdownService : IMarkdownService
{
    // Pipeline je thread-safe a drahá na sestavení → jednou, sdílená (služba je singleton).
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()      // tabulky, task listy, footnotes, autolinks, …
        .UseSoftlineBreakAsHardlineBreak()
        .UsePipeTables()
        .Build();

    public string ToHtml(string? markdown)
        => string.IsNullOrWhiteSpace(markdown)
            ? string.Empty
            : Markdown.ToHtml(markdown, Pipeline);
}
