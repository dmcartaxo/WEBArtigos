using WEBArtigos.Common;
using WEBArtigos.DTOs;
using WEBArtigos.Entities;
using WEBArtigos.Repositories;

namespace WEBArtigos.Services;

public class ArticleService(IArticleRepository repository, ILogger<ArticleService> logger) : IArticleService
{
    public async Task<PagedResult<ArticleResponseDto>> GetPagedAsync(ArticleQueryDto query)
    {
        // Garante valores seguros mesmo sem validação no DTO de query
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 1, 100);

        logger.LogInformation(
            "Listando artigos — página {Page}, tamanho {PageSize}, sortBy={SortBy}, sortDesc={SortDesc}",
            query.Page, query.PageSize, query.SortBy, query.SortDesc);

        var (items, totalCount) = await repository.GetPagedAsync(query);

        return new PagedResult<ArticleResponseDto>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ArticleResponseDto?> GetByIdAsync(int id)
    {
        var article = await repository.GetByIdAsync(id);

        if (article is null)
        {
            logger.LogWarning("Artigo {Id} não encontrado", id);
            return null;
        }

        return MapToResponse(article);
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
        logger.LogInformation("Artigo criado: ID={Id}, Título={Title}", created.Id, created.Title);
        return MapToResponse(created);
    }

    public async Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto)
    {
        var article = await repository.GetByIdAsync(id);

        if (article is null)
        {
            logger.LogWarning("Tentativa de atualização: artigo {Id} não encontrado", id);
            return null;
        }

        article.Title = dto.Title.Trim();
        article.Content = dto.Content.Trim();
        article.Author = dto.Author.Trim();

        var updated = await repository.UpdateAsync(article);
        logger.LogInformation("Artigo atualizado: ID={Id}", updated.Id);
        return MapToResponse(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var article = await repository.GetByIdAsync(id);

        if (article is null)
        {
            logger.LogWarning("Tentativa de exclusão: artigo {Id} não encontrado", id);
            return false;
        }

        await repository.DeleteAsync(article);
        logger.LogInformation("Artigo excluído: ID={Id}", id);
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
