namespace WEBArtigos.Services;

public interface IDocumentTextExtractor
{
    Task<string> ExtractTextAsync(IFormFile file);
}
