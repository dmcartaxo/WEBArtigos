# URLs de Acesso - WEBArtigos

## Aplicação Frontend
**URL**: http://localhost:5173

- Página de Login: http://localhost:5173/login
- Lista de Artigos: http://localhost:5173/
- Novo Artigo: http://localhost:5173/create
- Upload PDF/DOCX: http://localhost:5173/upload
- Editar Artigo: http://localhost:5173/edit/{id}
- Visualizar Artigo: http://localhost:5173/article/{id}

### Credenciais Padrão
- **Email**: admin@webartigos.com
- **Senha**: Admin@123

---

## API Backend

### URLs Base
**HTTP**: http://localhost:5237  
**HTTPS**: https://localhost:5237 (se usar HTTPS)

### Swagger/OpenAPI Documentation
**URL**: http://localhost:5237/ (raiz da aplicação)

> **Nota**: A configuração usa `RoutePrefix = string.Empty`, colocando o Swagger UI na raiz da API, não em `/swagger`.

### Endpoints Principais

#### Autenticação
```
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@webartigos.com",
  "password": "Admin@123"
}
```

#### Artigos
```
GET  /api/v1/articles                    - Listar artigos (com paginação)
GET  /api/v1/articles/{id}               - Obter artigo por ID
POST /api/v1/articles                    - Criar novo artigo
PUT  /api/v1/articles/{id}               - Atualizar artigo (Admin)
DELETE /api/v1/articles/{id}             - Deletar artigo (Admin)

POST /api/v1/articles/search             - Buscar artigos por texto
POST /api/v1/articles/upload             - Upload de PDF/DOCX
POST /api/v1/articles/{id}/resummarize   - Regenerar resumo (Admin)
```

---

## Configuração Utilizada

### Portas
- **Frontend (Vite)**: 5173
- **Backend (ASP.NET Core)**: 5237

### Banco de Dados
- **Tipo**: SQLite
- **Arquivo**: `artigos.db` (gerado automaticamente)
- **Localização**: Mesmo diretório da aplicação

### Autenticação
- **Tipo**: JWT (JSON Web Tokens)
- **Método**: Bearer Token no header `Authorization`
- **Expiração**: 8 horas (configurável em appsettings.json)

### Arquivo de Configuração
**Arquivo**: `WEBArtigos/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=artigos.db"
  },
  "Jwt": {
    "Secret": "W3bArt1g0s-S3cr3t-K3y-P@r4-JWT-M1n-32Ch!",
    "Issuer": "WEBArtigos",
    "Audience": "WEBArtigos",
    "ExpirationHours": 8
  },
  "Upload": {
    "MaxFileSizeMb": 10
  },
  "Anthropic": {
    "ApiKey": "sua-chave-aqui",
    "Model": "claude-sonnet-4-6"
  }
}
```

---

## Fluxo Completo de Teste

### 1. Inicie a Aplicação
```bash
# Backend (abra um terminal)
cd WEBArtigos
dotnet run

# Frontend (abra outro terminal)
cd WEBArtigos-Frontend
npm run dev
```

### 2. Acesse a Interface
- Abra http://localhost:5173 no navegador
- Faça login com admin@webartigos.com / Admin@123

### 3. Teste Funcionalidades
- **Listar Artigos**: Clique em "Artigos" no menu
- **Novo Artigo**: Clique em "Novo Artigo", preencha e envie
- **Upload PDF**: Clique em "Upload PDF/DOCX", selecione um PDF ou DOCX
- **Editar Artigo**: Clique "Editar" em um artigo (Admin)
- **Deletar Artigo**: Clique "Deletar" em um artigo (Admin)
- **Buscar**: Digite na caixa de busca para filtrar artigos

### 4. Visualize a API
- Abra http://localhost:5237 no navegador
- Clique em um endpoint para ver detalhes
- Use "Try it out" para testar (precisa estar autenticado)

---

## Troubleshooting

### Frontend não conecta à API
- Verifique se a porta 5237 está correta em `.env`:
  ```
  VITE_API_BASE_URL=http://localhost:5237/api/v1
  ```
- Reinicie o servidor frontend

### Erro de CORS
- Backend está configurado com CORS para `http://localhost:5173`
- Se a porta for diferente, atualize `Program.cs`:
  ```csharp
  .WithOrigins("http://localhost:NOVA_PORTA")
  ```

### Swagger não aparece
- Acesse a raiz: http://localhost:5237/
- Não tente acessar `/swagger` (está configurado como raiz)

### Erro ao fazer upload
- Verifique tamanho: máximo 10MB
- Verifique formato: apenas .pdf ou .docx
- Verifique se há texto extraível (não é PDF de imagem)

### Token JWT expirado
- Faça logout e login novamente
- Token tem validade de 8 horas por padrão

---

## Informações Técnicas

### Tecnologias Utilizadas

**Backend**:
- ASP.NET Core 8.0
- Entity Framework Core com SQLite
- FluentValidation
- JWT Authentication with BCrypt
- UglyToad.PdfPig (PDF)
- DocumentFormat.OpenXml (DOCX)
- Anthropic Claude API (Resumos)

**Frontend**:
- React 19.2.5
- Vite 8.0.10
- Axios para requisições HTTP
- React Router para navegação
- CSS3 com gradientes

### Estrutura do Projeto

```
WEBArtigos/
├── Controllers/         # Endpoints HTTP
├── Services/           # Lógica de negócio
├── Repositories/       # Acesso a dados
├── Entities/          # Modelos de dados
├── DTOs/              # Data Transfer Objects
├── Validators/        # FluentValidation
├── Middleware/        # Exception handling
├── Data/              # Database context
└── Program.cs         # Configuração

WEBArtigos-Frontend/
├── src/
│   ├── pages/         # Páginas da aplicação
│   ├── components/    # Componentes reutilizáveis
│   ├── services/      # Cliente HTTP (Axios)
│   ├── context/       # React Context (Auth)
│   ├── App.jsx        # Configuração de rotas
│   └── main.jsx       # Ponto de entrada
└── package.json       # Dependências
```

---

## Próximas Melhorias Sugeridas

- [ ] Adicionar testes automatizados
- [ ] Implementar cache de artigos
- [ ] Adicionar paginação no frontend
- [ ] Melhorar tratamento de erros
- [ ] Adicionar logging mais detalhado
- [ ] Implementar rate limiting
- [ ] Adicionar suporte para mais formatos (TXT, XLSX, PPTX)
