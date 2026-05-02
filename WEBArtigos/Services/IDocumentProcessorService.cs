namespace WEBArtigos.Services;

public interface IDocumentProcessorService
{
    Task<string> ExtractTextAsync(IFormFile file);
}
