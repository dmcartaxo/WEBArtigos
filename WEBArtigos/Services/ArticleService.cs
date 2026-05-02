using WEBArtigos.DTOs;
using WEBArtigos.Entities;
using WEBArtigos.Repositories;

namespace WEBArtigos.Services;

public class ArticleService(IArticleRepository repository) : IArticleService
{
    public async Task<IEnumerable<ArticleResponseDto>> GetAllAsync()
    {
        var articles = await repository.GetAllAsync();
        return articles.Select(MapToResponse);
    }

    public async Task<ArticleResponseDto?> GetByIdAsync(int id)
    {
        var article = await repository.GetByIdAsync(id);
        return article is null ? null : MapToResponse(article);
    }

    public async Task<IEnumerable<ArticleResponseDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var articles = await repository.SearchAsync(query.Trim());
        return articles.Select(MapToResponse);
    }

    public async Task<ArticleResponseDto> CreateAsync(ArticleCreateDto dto)
    {
        var article = new Article
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Author = dto.Author.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(article);
        return MapToResponse(created);
    }

    public async Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto)
    {
        var article = await repository.GetByIdAsync(id);
        if (article is null) return null;

        article.Title = dto.Title.Trim();
        article.Content = dto.Content.Trim();
        article.Author = dto.Author.Trim();

        var updated = await repository.UpdateAsync(article);
        return MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var article = await repository.GetByIdAsync(id);
        if (article is null) return false;

        await repository.DeleteAsync(article);
        return true;
    }

    private static ArticleResponseDto MapToResponse(Article article) => new()
    {
        Id = article.Id,
        Title = article.Title,
        Content = article.Content,
        Author = article.Author,
        CreatedAt = article.CreatedAt
    };
}
