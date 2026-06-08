using FilipunkBlog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilipunkBlog.Infrastructure.Persistence.Repositories;

public class CommentRepository(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<List<Comment>> GetApprovedByPostAsync(Guid postId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Comments
            .Where(x => x.BlogPostId == postId && x.IsApproved)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetPendingAsync()
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Comments
            .Include(x => x.BlogPost)
            .Where(x => !x.IsApproved)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Comment comment)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }

    public async Task ApproveAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return;
        comment.IsApproved = true;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = await factory.CreateDbContextAsync();
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return;
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }
}