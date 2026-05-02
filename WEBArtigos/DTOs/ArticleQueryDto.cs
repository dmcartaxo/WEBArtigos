namespace WEBArtigos.DTOs;

public class ArticleQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    /// <summary>Campo de ordenação: "title" ou "createdAt" (padrão)</summary>
    public string? SortBy { get; set; } = "createdAt";

    /// <summary>true = decrescente (padrão), false = crescente</summary>
    public bool SortDesc { get; set; } = true;

    /// <summary>Filtrar por autor exato</summary>
    public string? Author { get; set; }

    /// <summary>Data inicial do intervalo (UTC)</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>Data final do intervalo (UTC)</summary>
    public DateTime? DateTo { get; set; }

    /// <summary>Texto livre para buscar em título e conteúdo</summary>
    public string? Query { get; set; }
}
