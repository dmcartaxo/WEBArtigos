using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBArtigos.Common;
using WEBArtigos.DTOs;
using WEBArtigos.Entities;
using WEBArtigos.Services;

namespace WEBArtigos.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ArticlesController(IArticleService service, ILogger<ArticlesController> logger) : ControllerBase
{
    /// <summary>Lista artigos com paginação, filtros e ordenação.</summary>
    /// <param name="query">Parâmetros de busca, filtro, ordenação e paginação</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ArticleResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ArticleQueryDto query)
    {
        var result = await service.GetPagedAsync(query);
        return Ok(ApiResponse<PagedResult<ArticleResponseDto>>.Ok(result));
    }

    /// <summary>Busca artigos por texto no título ou conteúdo.</summary>
    /// <param name="query">Texto a pesquisar</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10)</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ArticleResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(ApiResponse<object>.Fail("O parâmetro 'query' é obrigatório."));

        var queryDto = new ArticleQueryDto { Query = query, Page = page, PageSize = pageSize };
        var result = await service.GetPagedAsync(queryDto);
        return Ok(ApiResponse<PagedResult<ArticleResponseDto>>.Ok(result));
    }

    /// <summary>Obtém um artigo pelo ID.</summary>
    /// <param name="id">ID do artigo</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ArticleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var article = await service.GetByIdAsync(id);

        if (article is null)
            return NotFound(ApiResponse<object>.Fail($"Artigo com ID {id} não encontrado."));

        return Ok(ApiResponse<ArticleResponseDto>.Ok(article));
    }

    /// <summary>Cria um novo artigo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ArticleResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ArticleCreateDto dto)
    {
        var created = await service.CreateAsync(dto);
        logger.LogInformation("POST /articles — artigo {Id} criado", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<ArticleResponseDto>.Ok(created));
    }

    /// <summary>Atualiza um artigo existente. Requer role Admin.</summary>
    /// <param name="id">ID do artigo</param>
    /// <param name="dto">Dados para atualização</param>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ArticleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ArticleUpdateDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);

        if (updated is null)
            return NotFound(ApiResponse<object>.Fail($"Artigo com ID {id} não encontrado."));

        return Ok(ApiResponse<ArticleResponseDto>.Ok(updated));
    }

    /// <summary>Remove um artigo. Requer role Admin.</summary>
    /// <param name="id">ID do artigo</param>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Artigo com ID {id} não encontrado."));

        return Ok(ApiResponse<object>.Ok(new { message = $"Artigo {id} removido com sucesso." }));
    }
}
