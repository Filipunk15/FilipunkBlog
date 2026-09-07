using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

/// <summary>Poskládá kompletní podklad životopisu z CMS dat (profil, zkušenosti, certifikáty, projekty).</summary>
public interface ICvService
{
    Task<CvDocumentModel> BuildAsync();
}

/// <summary>Vykreslí <see cref="CvDocumentModel"/> do PDF. Implementace v Infrastructure (QuestPDF).</summary>
public interface ICvPdfRenderer
{
    byte[] Render(CvDocumentModel model);
}
