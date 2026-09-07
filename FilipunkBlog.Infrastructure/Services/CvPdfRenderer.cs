using FilipunkBlog.Application.Contracts;
using FilipunkBlog.Application.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FilipunkBlog.Infrastructure.Services;

/// <summary>Vykreslí životopis do PDF přes QuestPDF – dvousloupcové rozvržení podle vzoru.</summary>
public class CvPdfRenderer : ICvPdfRenderer
{
    private const string Ink = "#1F2937";       // hlavní text
    private const string Muted = "#6B7280";     // šedá (organizace, popisky)
    private const string LabelCol = "#9CA3AF";  // nadpisy sekcí v levém sloupci
    private const float LabelWidth = 96f;
    private const float Gap = 16f;

    public byte[] Render(CvDocumentModel model)
    {
        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(40);
                page.MarginHorizontal(45);
                page.DefaultTextStyle(t => t.FontFamily(Fonts.Lato).FontSize(9.5f).FontColor(Ink).LineHeight(1.35f));

                page.Content().Column(col =>
                {
                    col.Spacing(18);

                    col.Item().Column(head =>
                    {
                        head.Item().Text(model.FullName).FontSize(22).SemiBold().FontColor(Ink);
                        head.Item().PaddingTop(2).Text((model.Headline ?? string.Empty).ToUpperInvariant())
                            .FontSize(10).LetterSpacing(0.12f).FontColor(Muted);
                    });

                    if (!string.IsNullOrWhiteSpace(model.Summary))
                        col.Item().Text(model.Summary).FontColor(Muted);

                    Section(col, model.Labels.Contact, c => ContactBlock(c, model.Contact));

                    if (model.Experience.Count > 0)
                        Section(col, model.Labels.Experience, c => EntryList(c, model.Experience));

                    if (model.Education.Count > 0)
                        Section(col, model.Labels.Education, c => EntryList(c, model.Education));

                    if (model.Certifications.Count > 0)
                        Section(col, model.Labels.Certificates, c => CertBlock(c, model.Certifications));

                    if (model.Projects.Count > 0)
                        Section(col, model.Labels.Projects, c => ProjectBlock(c, model.Projects));
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.DefaultTextStyle(s => s.FontSize(7.5f).FontColor(LabelCol));
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void Section(ColumnDescriptor col, string label, Action<IContainer> content)
    {
        col.Item().Row(row =>
        {
            row.ConstantItem(LabelWidth).PaddingTop(1).Text(label.ToUpperInvariant())
                .FontSize(9).Bold().LetterSpacing(0.06f).FontColor(LabelCol);
            row.ConstantItem(Gap);
            content(row.RelativeItem());
        });
    }

    private static void ContactBlock(IContainer c, List<CvContactLine> lines) =>
        c.Column(col =>
        {
            col.Spacing(3);
            foreach (var line in lines)
                col.Item().Text(t =>
                {
                    t.Span($"{line.Label}: ").FontColor(Muted);
                    t.Span(line.Value);
                });
        });

    private static void EntryList(IContainer c, List<CvEntry> entries) =>
        c.Column(col =>
        {
            col.Spacing(12);
            foreach (var e in entries)
                col.Item().Column(item =>
                {
                    item.Item().Text(t =>
                    {
                        t.Span(e.Title).SemiBold();
                        if (!string.IsNullOrWhiteSpace(e.PeriodLabel))
                            t.Span($"   |   {e.PeriodLabel}").FontColor(Muted);
                    });
                    if (!string.IsNullOrWhiteSpace(e.Organization))
                        item.Item().PaddingBottom(2).Text(e.Organization).FontColor(Muted);
                    foreach (var bullet in e.Bullets)
                        item.Item().Row(r =>
                        {
                            r.ConstantItem(10).Text("•").FontColor(Muted);
                            r.RelativeItem().Text(bullet);
                        });
                });
        });

    private static void CertBlock(IContainer c, List<CvCertification> certs) =>
        c.Column(col =>
        {
            col.Spacing(4);
            foreach (var cert in certs)
                col.Item().Text(t =>
                {
                    t.Span(cert.Name).SemiBold();
                    t.Span($"  —  {cert.Issuer}").FontColor(Muted);
                    if (cert.Year is { } y) t.Span($"  ({y})").FontColor(Muted);
                });
        });

    private static void ProjectBlock(IContainer c, List<CvProject> projects) =>
        c.Column(col =>
        {
            col.Spacing(9);
            foreach (var p in projects)
                col.Item().Column(item =>
                {
                    item.Item().Text(p.Title).SemiBold();
                    if (!string.IsNullOrWhiteSpace(p.Description))
                        item.Item().Text(p.Description).FontColor(Muted);
                });
        });
}
