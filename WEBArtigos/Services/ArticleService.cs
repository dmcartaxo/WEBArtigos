using WEBArtigos.Common;
using WEBArtigos.DTOs;
using WEBArtigos.Entities;
using WEBArtigos.Repositories;

namespace WEBArtigos.Services;

public class ArticleService(
    IArticleRepository repository,
    IFileService fileService,
    IPdfService pdfService,
    IAiService aiService,
    ILogger<ArticleService> logger) : IArticleService
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

    public async Task<ArticleResponseDto> UploadAsync(ArticleUploadDto dto)
    {
        // 1. Valida arquivo (tipo, tamanho, extensão)
        fileService.ValidateFile(dto.File);

        // 2. Extrai texto do PDF
        var content = await pdfService.ExtractTextAsync(dto.File);

        // 3. Gera resumo via IA
        logger.LogInformation("Gerando resumo para: {Title}", dto.Title.Trim());
        var summary = await aiService.SummarizeAsync(content);

        // 4. Persiste o artigo
        var article = new Article
        {
            Title = dto.Title.Trim(),
            Content = content,
            Author = dto.Author.Trim(),
            Summary = summary,
            OriginalFileName = dto.File.FileName,
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(article);
        logger.LogInformation("Artigo criado via upload: ID={Id}, Arquivo={File}", created.Id, dto.File.FileName);
        return MapToResponse(created);
    }

    public async Task<ArticleResponseDto?> ResummarizeAsync(int id)
    {
        var article = await repository.GetByIdAsync(id);

        if (article is null)
        {
            logger.LogWarning("Reprocessamento: artigo {Id} não encontrado", id);
            return null;
        }

        if (string.IsNullOrWhiteSpace(article.Content))
            throw new InvalidOperationException("Artigo não possui conteúdo para gerar resumo.");

        logger.LogInformation("Reprocessando resumo para artigo ID={Id}", id);
        article.Summary = await aiService.SummarizeAsync(article.Content);

        var updated = await repository.UpdateAsync(article);
        logger.LogInformation("Resumo reprocessado: ID={Id}", updated.Id);
        return MapToResponse(updated);
    }

    private static ArticleResponseDto MapToResponse(Article article) => new()
    {
        Id = article.Id,
        Title = article.Title,
        Content = article.Content,
        Author = article.Author,
        CreatedAt = article.CreatedAt,
        Summary = article.Summary,
        OriginalFileName = article.OriginalFileName
    };
}
