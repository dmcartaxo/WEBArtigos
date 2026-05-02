using Microsoft.EntityFrameworkCore;
using WEBArtigos.Data;
using WEBArtigos.Entities;

namespace WEBArtigos.Repositories;

public class ArticleRepository(AppDbContext context) : IArticleRepository
{
    public async Task<IEnumerable<Article>> GetAllAsync()
        => await context.Articles
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

    public async Task<Article?> GetByIdAsync(int id)
        => await context.Articles.FindAsync(id);

    public async Task<IEnumerable<Article>> SearchAsync(string query)
    {
        var lower = query.ToLower();
        return await context.Articles
            .Where(a => a.Title.ToLower().Contains(lower) || a.Content.ToLower().Contains(lower))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Article> CreateAsync(Article article)
    {
        context.Articles.Add(article);
        await context.SaveChangesAsync();
        return article;
    }

    public async Task<Article> UpdateAsync(Article article)
    {
        context.Articles.Update(article);
        await context.SaveChangesAsync();
        return article;
    }

    public async Task DeleteAsync(Article article)
    {
        context.Articles.Remove(article);
        await context.SaveChangesAsync();
    }
}
