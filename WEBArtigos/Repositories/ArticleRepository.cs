using Microsoft.EntityFrameworkCore;
using WEBArtigos.Data;
using WEBArtigos.DTOs;
using WEBArtigos.Entities;

namespace WEBArtigos.Repositories;

public class ArticleRepository(AppDbContext context) : IArticleRepository
{
    public async Task<(IEnumerable<Article> Items, int TotalCount)> GetPagedAsync(ArticleQueryDto query)
    {
        var q = context.Articles.AsQueryable();

        // Busca por texto em título ou conteúdo
        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var lower = query.Query.Trim().ToLower();
            q = q.Where(a => a.Title.ToLower().Contains(lower) || a.Content.ToLower().Contains(lower));
        }

        // Filtro por autor (exato, case-insensitive)
        if (!string.IsNullOrWhiteSpace(query.Author))
        {
            var author = query.Author.Trim().ToLower();
            q = q.Where(a => a.Author.ToLower() == author);
        }

        // Filtro por intervalo de datas
        if (query.DateFrom.HasValue)
            q = q.Where(a => a.CreatedAt >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            q = q.Where(a => a.CreatedAt <= query.DateTo.Value);

        var totalCount = await q.CountAsync();

        // Ordenação
        q = query.SortBy?.ToLower() switch
        {
            "title" => query.SortDesc
                ? q.OrderByDescending(a => a.Title)
                : q.OrderBy(a => a.Title),
            _ => query.SortDesc
                ? q.OrderByDescending(a => a.CreatedAt)
                : q.OrderBy(a => a.CreatedAt)
        };

        // Paginação
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Article?> GetByIdAsync(int id)
        => await context.Articles.FindAsync(id);

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
