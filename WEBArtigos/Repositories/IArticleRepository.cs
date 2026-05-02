using WEBArtigos.Entities;

namespace WEBArtigos.Repositories;

public interface IArticleRepository
{
    Task<IEnumerable<Article>> GetAllAsync();
    Task<Article?> GetByIdAsync(int id);
    Task<IEnumerable<Article>> SearchAsync(string query);
    Task<Article> CreateAsync(Article article);
    Task<Article> UpdateAsync(Article article);
    Task DeleteAsync(Article article);
}
