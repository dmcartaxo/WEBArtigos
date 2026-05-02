using System.Text;
using UglyToad.PdfPig;

namespace WEBArtigos.Services;

public class PdfService(ILogger<PdfService> logger) : IPdfService, IDocumentTextExtractor
{
    public async Task<string> ExtractTextAsync(IFormFile file)
    {
        logger.LogInformation("Extraindo texto do PDF: {FileName} ({Size} bytes)", file.FileName, file.Length);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        string text;
        try
        {
            using var document = PdfDocument.Open(bytes);
            var sb = new StringBuilder();

            foreach (var page in document.GetPages())
                sb.AppendLine(page.Text);

            text = sb.ToString().Trim();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao abrir o PDF: {FileName}", file.FileName);
            throw new InvalidOperationException("O arquivo não é um PDF válido ou está corrompido.", ex);
        }

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException(
                "O PDF não contém texto extraível. Pode ser um PDF de imagem (scaneado).");

        logger.LogInformation("Texto extraído com sucesso: {Chars} caracteres", text.Length);
        return text;
    }
}
