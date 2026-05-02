using WEBArtigos.DTOs;
using WEBArtigos.Entities;

namespace WEBArtigos.Repositories;

public interface IArticleRepository
{
    Task<(IEnumerable<Article> Items, int TotalCount)> GetPagedAsync(ArticleQueryDto query);
    Task<Article?> GetByIdAsync(int id);
    Task<Article> CreateAsync(Article article);
    Task<Article> UpdateAsync(Article article);
    Task DeleteAsync(Article article);
}
