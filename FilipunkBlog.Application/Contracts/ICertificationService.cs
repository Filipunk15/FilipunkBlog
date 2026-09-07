using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ICertificationService
{
    Task<List<CertificationViewModel>> GetAllAsync();
    Task CreateAsync(CertificationViewModel vm);
    Task UpdateAsync(CertificationViewModel vm);
    Task DeleteAsync(Guid id);
}
