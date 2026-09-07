using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface ICvProfileService
{
    Task<CvProfileViewModel> GetAsync();
    Task UpdateAsync(CvProfileViewModel vm);
}
