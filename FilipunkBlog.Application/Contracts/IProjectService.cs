using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Models;

namespace FilipunkBlog.Application.Contracts;

public interface IProjectService
{
    Task<PagedResult<ProjectViewModel>> GetProjectsAsync(int page = 1, int pageSize = 3);
    Task<ProjectViewModel?> GetProjectBySlugAsync(string slug);
    Task<List<ProjectViewModel>> GetProjectsByTagAsync(string tagSlug);
    Task<List<ProjectViewModel>> GetAllProjectsAsync();
    Task CreateProjectAsync(ProjectViewModel project);
    Task UpdateProjectAsync(ProjectViewModel project);
    Task DeleteProjectAsync(Guid id);
    Task<List<ProjectViewModel>> SearchProjectsAsync(string query);
}
