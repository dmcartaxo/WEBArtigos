using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;

namespace WEBArtigos.Services;

public class DocxService(ILogger<DocxService> logger) : IDocumentTextExtractor
{
    public async Task<string> ExtractTextAsync(IFormFile file)
    {
        logger.LogInformation("Extraindo texto do DOCX: {FileName} ({Size} bytes)", file.FileName, file.Length);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        string text;
        try
        {
            using (var document = WordprocessingDocument.Open(ms, false))
            {
                if (document.MainDocumentPart?.Document?.Body is null)
                    throw new InvalidOperationException("Documento DOCX inválido ou sem conteúdo.");

                var sb = new StringBuilder();
                var body = document.MainDocumentPart.Document.Body;

                // Extrai texto de parágrafos
                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    var paragraphText = ExtractParagraphText(paragraph);
                    if (!string.IsNullOrEmpty(paragraphText))
                        sb.AppendLine(paragraphText);
                }

                // Extrai texto de tabelas
                foreach (var table in body.Elements<Table>())
                {
                    foreach (var row in table.Elements<TableRow>())
                    {
                        var rowTexts = new List<string>();
                        foreach (var cell in row.Elements<TableCell>())
                        {
                            var cellText = ExtractCellText(cell);
                            if (!string.IsNullOrEmpty(cellText))
                                rowTexts.Add(cellText);
                        }
                        if (rowTexts.Count > 0)
                            sb.AppendLine(string.Join(" | ", rowTexts));
                    }
                }

                text = sb.ToString().Trim();
            }
        }
        catch (OpenXmlPackageException ex)
        {
            logger.LogError(ex, "Arquivo DOCX inválido ou corrompido: {FileName}", file.FileName);
            throw new InvalidOperationException("O arquivo não é um DOCX válido ou está corrompido.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao processar DOCX: {FileName}", file.FileName);
            throw new InvalidOperationException("Erro ao extrair texto do DOCX.", ex);
        }

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("O DOCX não contém texto extraível.");

        logger.LogInformation("Texto extraído com sucesso: {Chars} caracteres", text.Length);
        return text;
    }

    private static string ExtractParagraphText(Paragraph paragraph)
    {
        var sb = new StringBuilder();

        foreach (var run in paragraph.Elements<Run>())
        {
            foreach (var text in run.Elements<Text>())
                sb.Append(text.Text);

            // Extrai texto de tabs
            foreach (var tab in run.Elements<TabChar>())
                sb.Append("\t");

            // Extrai texto de quebras de linha
            foreach (var br in run.Elements<Break>())
                sb.Append("\n");
        }

        return sb.ToString();
    }

    private static string ExtractCellText(TableCell cell)
    {
        var sb = new StringBuilder();

        foreach (var paragraph in cell.Elements<Paragraph>())
        {
            var paragraphText = ExtractParagraphText(paragraph);
            if (!string.IsNullOrEmpty(paragraphText))
                sb.Append(paragraphText);
        }

        return sb.ToString().Trim();
    }
}
