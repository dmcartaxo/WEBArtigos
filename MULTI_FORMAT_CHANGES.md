# Evolução Multi-Formato - Alterações Implementadas

## Resumo Executivo
Implementado suporte para múltiplos formatos de arquivo (PDF e DOCX) no endpoint de upload de artigos. O sistema agora pode processar tanto PDF quanto arquivos Word com extração automática de texto.

## Arquitetura Implementada

### Fluxo Completo
```
Upload (PDF/DOCX) 
  ↓
FileService (validação)
  ↓
DocumentProcessorService (roteamento)
  ├─→ .pdf → PdfService (IDocumentTextExtractor)
  └─→ .docx → DocxService (IDocumentTextExtractor)
  ↓
Texto extraído
  ↓
AiService (resumo)
  ↓
Repository (persistência)
```

## Novos Arquivos Criados

### 1. **IDocumentTextExtractor.cs**
- Interface comum para todos os extractores de texto
- Contrato: `Task<string> ExtractTextAsync(IFormFile file)`
- Implementado por: PdfService e DocxService

### 2. **DocxService.cs**
- Extrai texto de arquivos DOCX usando `DocumentFormat.OpenXml`
- Suporta:
  - Parágrafos
  - Tabelas (com separação de colunas por " | ")
  - Tabs e quebras de linha
- Validações:
  - Documento inválido → erro descriptivo
  - Sem texto extraível → erro específico
  - Logging detalhado de extração

### 3. **IDocumentProcessorService.cs**
- Interface para o serviço orquestrador
- Responsabilidade: rotear arquivo para extrator correto

### 4. **DocumentProcessorService.cs**
- Orquestra a extração de múltiplos formatos
- Detecta tipo por extensão (.pdf, .docx)
- Injeta PdfService e DocxService
- Lança erro descritivo para formatos não suportados
- Logging de tipo de documento processado

## Arquivos Modificados

### 1. **PdfService.cs**
```diff
- public class PdfService(ILogger<PdfService> logger) : IPdfService
+ public class PdfService(ILogger<PdfService> logger) : IPdfService, IDocumentTextExtractor
```
- Agora implementa `IDocumentTextExtractor` além de `IPdfService`
- Mantém backward compatibility com código existente
- Nenhuma lógica alterada (apenas novo contrato)

### 2. **FileService.cs**
```diff
- private static readonly string[] AllowedContentTypes = ["application/pdf", ...];
+ Adicionado "application/vnd.openxmlformats-officedocument.wordprocessingml.document"

- if (extension != ".pdf")
+ if (!AllowedExtensions.Contains(extension))
  
- AllowedExtensions = [".pdf", ".docx"]
```
- Extensões permitidas: `.pdf` e `.docx`
- Content-Types permitidos estendidos
- Mensagens de erro atualizadas

### 3. **ArticleService.cs**
```diff
- public class ArticleService(
-   IArticleRepository repository,
-   IFileService fileService,
-   IPdfService pdfService,
-   ...

+ public class ArticleService(
+   IArticleRepository repository,
+   IFileService fileService,
+   IDocumentProcessorService documentProcessor,
+   ...

- var content = await pdfService.ExtractTextAsync(dto.File);
+ var content = await documentProcessor.ExtractTextAsync(dto.File);
```
- Injeção: `IPdfService` → `IDocumentProcessorService`
- Uso: `pdfService` → `documentProcessor`
- Método `UploadAsync` agora agnóstico a formato

### 4. **Program.cs**
```diff
+ builder.Services.AddScoped<PdfService>();
+ builder.Services.AddScoped<DocxService>();
+ builder.Services.AddScoped<IPdfService>(sp => sp.GetRequiredService<PdfService>());
+ builder.Services.AddScoped<IDocumentProcessorService, DocumentProcessorService>();
```
- PdfService registrado como Scoped (sem interface)
- DocxService novo registrado como Scoped
- IPdfService agora factory que retorna PdfService
- DocumentProcessorService registrado como IDocumentProcessorService

### 5. **UploadPdf.jsx** (Frontend)
- Nome do componente: mantido como "UploadPdf" (pode renomear depois)
- Título: "Upload de PDF" → "Upload de Documento"
- Descrição: "Envie um PDF..." → "Envie um PDF ou DOCX..."
- Accept: `.pdf` → `.pdf,.docx`
- Validação: apenas PDF → PDF ou DOCX
- Label: "Arquivo PDF" → "Arquivo (PDF ou DOCX)"
- Placeholder: "Selecione um arquivo PDF" → "Selecione um arquivo PDF ou DOCX"
- Botão: "Enviar PDF" → "Enviar Documento"
- Instruções atualizadas

## Validações Implementadas

### Backend (FileService)
- ✅ Extensão: .pdf ou .docx
- ✅ Content-Type: PDF ou DOCX ou application/octet-stream
- ✅ Tamanho: máximo 10MB (configurável)
- ✅ Arquivo não nulo

### Extração (Serviços)
- ✅ PDF válido (PdfService - já existia)
- ✅ PDF com texto extraível (não é imagem)
- ✅ DOCX válido (DocxService - novo)
- ✅ Documento não vazio (ambos)

### Frontend (UploadPdf.jsx)
- ✅ Content-Type: PDF ou DOCX
- ✅ Tamanho: máximo 10MB
- ✅ Campos obrigatórios (título, autor, arquivo)

## Padrões e Boas Práticas

✅ **DRY (Don't Repeat Yourself)**
- Interface comum (IDocumentTextExtractor)
- Orquestrador centralizado (DocumentProcessorService)

✅ **Dependency Injection**
- Todas as dependências injetadas
- Sem acoplamento direto
- Fácil de testar

✅ **Separation of Concerns**
- FileService: validação apenas
- DocumentProcessorService: roteamento apenas
- PdfService/DocxService: extração apenas
- ArticleService: orquestração apenas

✅ **Async/Await**
- Todos os métodos assincronos
- Operações I/O não bloqueiam

✅ **Logging**
- Cada serviço registra suas operações
- Nível INFO para fluxo normal
- Nível ERROR para exceções

✅ **Error Handling**
- Exceções descritivas
- Mensagens traduzidas (PT-BR)
- Distingue erro de validação vs. erro técnico

## Extensibilidade para Futuro

Adicionar novo formato (ex: TXT) requer apenas:

1. **Criar TxtService.cs**
   ```csharp
   public class TxtService(ILogger<TxtService> logger) : IDocumentTextExtractor
   {
       public async Task<string> ExtractTextAsync(IFormFile file) { ... }
   }
   ```

2. **Registrar no Program.cs**
   ```csharp
   builder.Services.AddScoped<TxtService>();
   ```

3. **Adicionar case no DocumentProcessorService**
   ```csharp
   ".txt" => txtService as IDocumentTextExtractor,
   ```

4. **Atualizar validação no FileService**
   ```csharp
   AllowedExtensions = [".pdf", ".docx", ".txt"]
   ```

Nenhuma alteração necessária no ArticleService, Controller, ou lógica existente!

## Testes Recomendados

### Funcionalidade PDF
- ✅ Upload PDF normal
- ✅ PDF com múltiplas páginas
- ✅ PDF com tabelas
- ❌ PDF corrompido (deve rejeitar)
- ❌ PDF sem texto (imagem)

### Funcionalidade DOCX
- ✅ Upload DOCX simples
- ✅ DOCX com parágrafos múltiplos
- ✅ DOCX com tabelas
- ✅ DOCX com caracteres especiais
- ❌ DOCX corrompido (deve rejeitar)

### Validações
- ❌ Arquivo TXT (deve rejeitar)
- ❌ Arquivo > 10MB (deve rejeitar)
- ❌ Arquivo vazio (deve rejeitar)
- ❌ Wrong MIME type (deve rejeitar)

### Integração
- ✅ Texto extraído correto
- ✅ Artigo criado no banco
- ✅ OriginalFileName preenchido
- ✅ Resumo IA gerado (quando habilitado)

## Compatibilidade

✅ **Backward Compatible**
- IPdfService continua funcionando
- PdfService pode ser injetado diretamente se necessário
- Fluxo PDF idêntico ao anterior

✅ **Sem Breaking Changes**
- ArticleUploadDto inalterado
- ArticleResponseDto inalterado
- Endpoint /api/v1/articles/upload inalterado
- Database inalterada

## Notas de Implementação

1. **DocumentFormat.OpenXml**
   - Versão: 3.5.1 (já estava nas dependências)
   - Nenhuma nova package necessária
   - Namespace: DocumentFormat.OpenXml.Wordprocessing

2. **Estratégia de Injeção**
   - PdfService registrado sem interface para ser injetado no DocumentProcessorService
   - DocxService similar
   - Ambos também implementam IDocumentTextExtractor

3. **Extraction de Tabelas**
   - DOCX: tabelas separadas por " | " entre colunas
   - Mantém estrutura legível em texto puro
   - PDF não tem suporte específico para tabelas (PdfPig extrai como texto)

4. **Performance**
   - Ambas operações assincronias
   - Arquivo carregado em MemoryStream (até 10MB)
   - Sem impacto significativo

## Commits Necessários

```bash
git add .
git commit -m "feat: add multi-format document support (PDF and DOCX)

- Create IDocumentTextExtractor common interface
- Implement DocxService for DOCX text extraction
- Create DocumentProcessorService to route files
- Extend FileService to validate multiple formats
- Update PdfService to implement new interface
- Update ArticleService to use processor service
- Register new services in dependency injection
- Update frontend to accept DOCX files

Supports PDF and DOCX formats with automatic routing.
Maintains backward compatibility with existing code.

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```
