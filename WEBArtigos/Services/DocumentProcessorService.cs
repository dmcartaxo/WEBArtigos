namespace WEBArtigos.Services;

public class DocumentProcessorService(
    PdfService pdfService,
    DocxService docxService,
    ILogger<DocumentProcessorService> logger) : IDocumentProcessorService
{
    public async Task<string> ExtractTextAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        logger.LogInformation(
            "Processando documento: {FileName}, extensão: {Extension}",
            file.FileName,
            extension);

        var extractor = extension switch
        {
            ".pdf" => pdfService as IDocumentTextExtractor,
            ".docx" => docxService as IDocumentTextExtractor,
            _ => throw new InvalidOperationException($"Formato de arquivo não suportado: {extension}")
        };

        return await extractor.ExtractTextAsync(file);
    }
}
