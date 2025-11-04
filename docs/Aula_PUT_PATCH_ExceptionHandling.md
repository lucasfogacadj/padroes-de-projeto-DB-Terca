# 🎓 AULA: PUT vs PATCH & Exception Handling

**Disciplina**: Desenvolvimento Backend  
**Semestre**: 2º - Análise e Desenvolvimento de Sistemas  
**Data**: 04 de novembro de 2025  
**Duração**: 2-3 horas  
**Professor**: Lucas Fogaça

---

## 📋 OBJETIVOS DA AULA

Ao final desta aula, o aluno será capaz de:
1. Diferenciar PUT de PATCH e escolher adequadamente
2. Implementar endpoints de atualização com validações
3. Entender a importância do tratamento centralizado de exceções
4. Implementar middleware de tratamento de erros
5. Aplicar o padrão Problem Details (RFC 7807)
6. Distinguir exceções de negócio de exceções técnicas

---

# 🎯 PARTE 1: PUT vs PATCH

## SLIDE 1: Agenda da Parte 1
- O que são operações de atualização?
- HTTP PUT - Substituição Total
- HTTP PATCH - Atualização Parcial
- Quando usar cada um?
- Idempotência
- Implementação prática

---

## SLIDE 2: O Problema - Cenário Real

### 🏢 Contexto de Negócio
**E-commerce - Atualização de Produto**

```
Produto Atual no Banco:
{
  "id": 1,
  "nome": "Notebook Dell",
  "descricao": "I5 8GB RAM",
  "preco": 3500.00,
  "estoque": 10,
  "dataCriacao": "2025-01-15"
}
```

### 📝 Cenários Comuns:
1. **Cliente 1**: Quer atualizar APENAS o preço (promoção)
2. **Cliente 2**: Quer atualizar TUDO (reformular produto)
3. **Cliente 3**: Quer atualizar nome e descrição, manter resto

**❓ Pergunta**: Como implementar isso de forma RESTful?

---

## SLIDE 3: HTTP PUT - Conceito

### 📖 Definição (RFC 7231)
> "O método PUT substitui TODAS as representações atuais do recurso alvo com o payload da requisição."

### ✅ Características:
- **Substituição TOTAL** do recurso
- Cliente deve enviar **TODOS** os campos
- Se omitir campo → campo vira `null` ou valor padrão
- **Idempotente**: executar N vezes = mesmo resultado

### 📦 Request PUT Completo:
```json
PUT /produtos/1
Content-Type: application/json

{
  "id": 1,
  "nome": "Notebook Dell XPS",
  "descricao": "I7 16GB RAM SSD 512GB",
  "preco": 4500.00,
  "estoque": 15
}
```

### ⚠️ O que acontece se omitir campo?
```json
PUT /produtos/1
{
  "nome": "Notebook Dell XPS",
  "preco": 4500.00
}

Resultado no Banco:
{
  "id": 1,
  "nome": "Notebook Dell XPS",
  "descricao": null,  // ❌ PERDEU O VALOR!
  "preco": 4500.00,
  "estoque": 0        // ❌ RESETOU!
}
```

---

## SLIDE 4: HTTP PATCH - Conceito

### 📖 Definição (RFC 5789)
> "O método PATCH aplica modificações PARCIAIS em um recurso."

### ✅ Características:
- **Atualização PARCIAL** do recurso
- Cliente envia **APENAS** os campos que quer mudar
- Campos não enviados **permanecem inalterados**
- **Idempotente** (se bem implementado)

### 📦 Request PATCH Parcial:
```json
PATCH /produtos/1
Content-Type: application/json

{
  "preco": 3200.00
}

Resultado no Banco:
{
  "id": 1,
  "nome": "Notebook Dell",      // ✅ Manteve
  "descricao": "I5 8GB RAM",     // ✅ Manteve
  "preco": 3200.00,              // ✅ Atualizou
  "estoque": 10,                 // ✅ Manteve
  "dataCriacao": "2025-01-15"    // ✅ Manteve
}
```

---

## SLIDE 5: Comparação Visual PUT vs PATCH

```
┌─────────────────────────────────────────────────────────────┐
│                    PRODUTO ORIGINAL                         │
├─────────────────────────────────────────────────────────────┤
│ id: 1                                                       │
│ nome: "Notebook Dell"                                       │
│ descricao: "I5 8GB RAM"                                     │
│ preco: 3500.00                                              │
│ estoque: 10                                                 │
└─────────────────────────────────────────────────────────────┘

📤 Request: { "preco": 3000.00 }

┌─────────────────────┬───────────────────────────────────────┐
│      PUT            │           PATCH                       │
├─────────────────────┼───────────────────────────────────────┤
│ ❌ ERRO 400         │ ✅ SUCESSO 200                        │
│ "Campos            │                                       │
│ obrigatórios       │ id: 1                                 │
│ ausentes"          │ nome: "Notebook Dell"                 │
│                    │ descricao: "I5 8GB RAM"               │
│                    │ preco: 3000.00  ← MUDOU               │
│                    │ estoque: 10                           │
└─────────────────────┴───────────────────────────────────────┘
```

---

## SLIDE 6: Quando Usar PUT?

### ✅ Use PUT quando:

1. **Substituição total faz sentido no negócio**
   - Exemplo: Upload de arquivo (substitui completamente)
   
2. **Cliente tem TODOS os dados do recurso**
   - Frontend já carregou tudo e vai reenviar
   
3. **Modelo de dados é simples e estável**
   - Poucos campos, sem crescimento previsto

4. **Idempotência é crítica**
   - Retry automático não pode causar problemas

### 💼 Casos de Uso Reais:
```
✅ Atualizar configuração de usuário (perfil completo)
✅ Substituir documento inteiro
✅ Atualizar status de pedido (máquina de estados)
```

---

## SLIDE 7: Quando Usar PATCH?

### ✅ Use PATCH quando:

1. **Atualização frequente de campos específicos**
   - Exemplo: Curtir post (só incrementa contador)
   
2. **Recurso tem muitos campos**
   - Produtos, Perfis detalhados, Configurações
   
3. **Economia de banda é importante**
   - Mobile apps, APIs públicas
   
4. **Evitar conflitos de atualização concorrente**
   - Dois usuários editando campos diferentes

### 💼 Casos de Uso Reais:
```
✅ Atualizar preço de produto (e-commerce)
✅ Incrementar contador de views
✅ Marcar notificação como lida
✅ Atualizar status de tarefa (todo list)
```

---

## SLIDE 8: Idempotência - Conceito Crítico

### 📖 Definição:
> Operação **idempotente** pode ser executada múltiplas vezes sem efeitos colaterais além da primeira execução.

### 🔄 PUT é Naturalmente Idempotente:
```json
PUT /produtos/1 { "nome": "X", "preco": 100 }

Execução 1: produto.nome = "X", preco = 100
Execução 2: produto.nome = "X", preco = 100  ← Mesmo resultado
Execução 3: produto.nome = "X", preco = 100  ← Mesmo resultado
```

### ⚠️ PATCH Pode NÃO Ser (se mal implementado):
```json
// ❌ MAL: Incremento relativo
PATCH /produtos/1 { "estoque": "+5" }

Execução 1: estoque = 10 + 5 = 15
Execução 2: estoque = 15 + 5 = 20  ← Diferente!
Execução 3: estoque = 20 + 5 = 25  ← Problema!

// ✅ BOM: Valor absoluto
PATCH /produtos/1 { "estoque": 15 }

Execução 1: estoque = 15
Execução 2: estoque = 15  ← Idempotente!
Execução 3: estoque = 15  ← Seguro para retry!
```

---

## SLIDE 9: Status Codes Corretos

### PUT:
```
200 OK              → Atualização bem-sucedida (retorna recurso)
204 No Content      → Atualização bem-sucedida (sem corpo)
201 Created         → Criou recurso (PUT como upsert)
400 Bad Request     → Payload inválido/incompleto
404 Not Found       → Recurso não existe
409 Conflict        → Conflito de versão
422 Unprocessable   → Validação de negócio falhou
```

### PATCH:
```
200 OK              → Atualização parcial bem-sucedida
204 No Content      → Atualização sem retornar corpo
400 Bad Request     → Campo inválido
404 Not Found       → Recurso não existe
409 Conflict        → Conflito de versão (ETag)
422 Unprocessable   → Campo não pode ser modificado
```

---

## SLIDE 10: DTOs para PUT vs PATCH

### 🏗️ Estratégias de Design:

#### **Opção 1: DTOs Separados (Recomendado)**
```csharp
// DTO para PUT (todos campos obrigatórios)
public class ProdutoUpdateDto
{
    [Required]
    public string Nome { get; set; }
    
    [Required]
    public string Descricao { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Preco { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Estoque { get; set; }
}

// DTO para PATCH (todos campos opcionais)
public class ProdutoPatchDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal? Preco { get; set; }
    public int? Estoque { get; set; }
}
```

#### **Opção 2: DTO Único com Nullable**
```csharp
public class ProdutoUpdateDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal? Preco { get; set; }
    public int? Estoque { get; set; }
}
```

---

## SLIDE 11: Implementação PUT - Service Layer

```csharp
public async Task<Produto> AtualizarAsync(
    int id, 
    ProdutoUpdateDto dto, 
    CancellationToken ct = default)
{
    // 1. Buscar recurso existente
    var produto = await _repo.GetByIdAsync(id, ct);
    if (produto == null)
        throw new NotFoundException($"Produto {id} não encontrado");
    
    // 2. Validar dados de entrada
    if (string.IsNullOrWhiteSpace(dto.Nome))
        throw new ValidationException("Nome é obrigatório");
    
    if (dto.Preco <= 0)
        throw new ValidationException("Preço deve ser maior que zero");
    
    if (dto.Estoque < 0)
        throw new ValidationException("Estoque não pode ser negativo");
    
    // 3. Substituir TODOS os campos
    produto.Nome = dto.Nome.Trim();
    produto.Descricao = dto.Descricao.Trim();
    produto.Preco = dto.Preco;
    produto.Estoque = dto.Estoque;
    // DataCriacao NÃO é atualizada (campo auditoria)
    
    // 4. Persistir
    await _repo.UpdateAsync(produto, ct);
    await _repo.SaveChangesAsync(ct);
    
    return produto;
}
```

---

## SLIDE 12: Implementação PATCH - Service Layer

```csharp
public async Task<Produto> AtualizarParcialAsync(
    int id, 
    ProdutoPatchDto dto, 
    CancellationToken ct = default)
{
    // 1. Buscar recurso existente
    var produto = await _repo.GetByIdAsync(id, ct);
    if (produto == null)
        throw new NotFoundException($"Produto {id} não encontrado");
    
    // 2. Atualizar APENAS campos enviados (não-null)
    if (dto.Nome != null)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ValidationException("Nome não pode ser vazio");
        produto.Nome = dto.Nome.Trim();
    }
    
    if (dto.Descricao != null)
    {
        produto.Descricao = dto.Descricao.Trim();
    }
    
    if (dto.Preco.HasValue)
    {
        if (dto.Preco.Value <= 0)
            throw new ValidationException("Preço deve ser maior que zero");
        produto.Preco = dto.Preco.Value;
    }
    
    if (dto.Estoque.HasValue)
    {
        if (dto.Estoque.Value < 0)
            throw new ValidationException("Estoque não pode ser negativo");
        produto.Estoque = dto.Estoque.Value;
    }
    
    // 3. Persistir
    await _repo.UpdateAsync(produto, ct);
    await _repo.SaveChangesAsync(ct);
    
    return produto;
}
```

---

## SLIDE 13: Endpoints - Minimal API

```csharp
// PUT - Substituição Total
app.MapPut("/produtos/{id}", async (
    int id, 
    ProdutoUpdateDto dto, 
    IProdutoService service, 
    CancellationToken ct) =>
{
    var produto = await service.AtualizarAsync(id, dto, ct);
    return Results.Ok(produto);
})
.WithName("AtualizarProdutoCompleto")
.WithOpenApi()
.Produces<Produto>(200)
.Produces(404)
.Produces(400);

// PATCH - Atualização Parcial
app.MapPatch("/produtos/{id}", async (
    int id, 
    ProdutoPatchDto dto, 
    IProdutoService service, 
    CancellationToken ct) =>
{
    var produto = await service.AtualizarParcialAsync(id, dto, ct);
    return Results.Ok(produto);
})
.WithName("AtualizarProdutoParcial")
.WithOpenApi()
.Produces<Produto>(200)
.Produces(404)
.Produces(400);
```

---

## SLIDE 14: Repository - Método Update

```csharp
public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public Task UpdateAsync(Produto produto, CancellationToken ct = default)
    {
        // EF Core rastreia mudanças automaticamente
        // Se entidade já está sendo rastreada (FindAsync)
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }
    
    // Alternativa: Marcar estado explicitamente
    public Task UpdateAsync(Produto produto, CancellationToken ct = default)
    {
        _context.Entry(produto).State = EntityState.Modified;
        return Task.CompletedTask;
    }
    
    // SaveChanges ainda é responsabilidade do chamador
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
```

---

## SLIDE 15: Testando PUT vs PATCH

### 🧪 Teste Manual com REST Client:

```http
### Cenário 1: PUT com todos campos
PUT https://localhost:5001/produtos/1
Content-Type: application/json

{
  "nome": "Notebook Dell XPS 15",
  "descricao": "I7 11ª geração, 16GB RAM, SSD 512GB",
  "preco": 5500.00,
  "estoque": 8
}

### Cenário 2: PUT sem campo (deve dar erro)
PUT https://localhost:5001/produtos/1
Content-Type: application/json

{
  "nome": "Notebook Dell XPS 15",
  "preco": 5500.00
}
# ❌ Esperado: 400 Bad Request

### Cenário 3: PATCH apenas preço
PATCH https://localhost:5001/produtos/1
Content-Type: application/json

{
  "preco": 5200.00
}
# ✅ Esperado: 200 OK, apenas preço muda

### Cenário 4: PATCH múltiplos campos
PATCH https://localhost:5001/produtos/1
Content-Type: application/json

{
  "preco": 5200.00,
  "estoque": 15
}
```

---

## SLIDE 16: Desafios Comuns e Soluções

### ❌ Problema 1: PATCH enviando `null` explicitamente
```json
PATCH /produtos/1
{
  "nome": null  // Intenção: limpar o nome?
}
```
**Solução**: Definir convenção (null = ignorar OU null = limpar?)

### ❌ Problema 2: PUT sem validar campos obrigatórios
**Solução**: Usar `[Required]` nos DTOs + FluentValidation

### ❌ Problema 3: Concorrência (two updates ao mesmo tempo)
**Solução**: Optimistic Locking (ETag/RowVersion) - próxima aula

### ❌ Problema 4: Campos calculados sendo atualizados
```json
PUT /produtos/1
{
  "dataCriacao": "2025-11-04"  // ❌ Não deveria mudar!
}
```
**Solução**: DTO não contém campos imutáveis

---

## SLIDE 17: Boas Práticas - Checklist

### ✅ Design de API:
- [ ] Documentar claramente se endpoint é PUT ou PATCH
- [ ] PUT exige TODOS os campos obrigatórios
- [ ] PATCH todos campos opcionais (nullable)
- [ ] Retornar 404 se recurso não existe
- [ ] Retornar recurso atualizado no body (ou 204)

### ✅ Validação:
- [ ] Validar regras de negócio ANTES de persistir
- [ ] Mensagens de erro claras
- [ ] Não permitir atualização de campos imutáveis (ID, DataCriacao)

### ✅ Performance:
- [ ] Buscar entidade com tracking no PUT/PATCH
- [ ] Não fazer `SELECT` depois do `UPDATE` (já tem objeto)
- [ ] Usar `AsNoTracking()` apenas em leituras

---

---

# 🛡️ PARTE 2: EXCEPTION HANDLING

## SLIDE 18: Agenda da Parte 2
- Por que tratar exceções centralizadamente?
- Tipos de exceções (Negócio vs Infraestrutura)
- Middleware Pipeline no ASP.NET Core
- Problem Details (RFC 7807)
- Global Exception Handler
- Logging de exceções
- Implementação prática

---

## SLIDE 19: O Problema - Código Atual

### ❌ Sem Tratamento Centralizado:

```csharp
app.MapGet("/produtos/{id}", async (int id, IProdutoService service) =>
{
    try
    {
        var produto = await service.ObterAsync(id);
        return Results.Ok(produto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem("Erro interno");
    }
});

app.MapPost("/produtos", async (ProdutoCreateDto dto, IProdutoService service) =>
{
    try
    {
        var produto = await service.CriarAsync(...);
        return Results.Created($"/produtos/{produto.Id}", produto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem("Erro interno");
    }
});

// ❌ DUPLICAÇÃO em TODOS os endpoints!
```

---

## SLIDE 20: Problemas do Código Duplicado

### 🔴 Problemas:

1. **Duplicação Massiva**
   - Mesmo código try-catch em 10, 20, 50 endpoints

2. **Inconsistência**
   - Um endpoint retorna 400, outro 422 para mesmo erro
   - Mensagens de erro diferentes

3. **Falta de Rastreabilidade**
   - Sem logging estruturado
   - Difícil debugar em produção

4. **Violação de SRP**
   - Endpoint tem que saber COMO tratar cada tipo de exceção

5. **Dificuldade de Manutenção**
   - Mudar formato de erro = alterar N endpoints

---

## SLIDE 21: A Solução - Middleware de Exceções

### 🎯 Conceito:
> Centralizar tratamento de exceções em UM único lugar usando Middleware

```
┌─────────────────────────────────────────────────┐
│              REQUEST HTTP                       │
└────────────────┬────────────────────────────────┘
                 │
    ┌────────────▼──────────────┐
    │  Exception Middleware     │  ← AQUI!
    │  (try-catch global)       │
    └────────────┬──────────────┘
                 │
    ┌────────────▼──────────────┐
    │  Outros Middlewares       │
    │  (Auth, Logging, etc)     │
    └────────────┬──────────────┘
                 │
    ┌────────────▼──────────────┐
    │  Endpoints                │
    │  (sem try-catch!)         │
    └────────────┬──────────────┘
                 │
         ❌ Exception aqui?
                 │
    ┌────────────▼──────────────┐
    │  Exception Middleware     │  ← CAPTURA!
    │  (converte para JSON)     │
    └────────────┬──────────────┘
                 │
    ┌────────────▼──────────────┐
    │  RESPONSE HTTP (JSON)     │
    └───────────────────────────┘
```

---

## SLIDE 22: ASP.NET Core Middleware Pipeline

### 🔗 Como Funciona:

```csharp
var app = WebApplication.CreateBuilder(args).Build();

// Middlewares são executados em ORDEM
app.UseExceptionHandler("/error");  // ← 1º (captura exceções)
app.UseHttpsRedirection();           // ← 2º
app.UseAuthentication();             // ← 3º
app.UseAuthorization();              // ← 4º

app.MapControllers();                // ← 5º (endpoints)

app.Run();
```

### 📊 Fluxo:
```
Request  → Middleware 1 → Middleware 2 → Endpoint
Response ← Middleware 1 ← Middleware 2 ← 
```

Se exceção ocorre no Endpoint, "sobe" até ser capturada!

---

## SLIDE 23: Tipos de Exceções - Arquitetura

### 🏗️ Classificação:

```
                    Exception
                        │
        ┌───────────────┴───────────────┐
        │                               │
  Domain Exceptions            Infrastructure Exceptions
  (Regras de Negócio)         (Problemas Técnicos)
        │                               │
   ┌────┴─────┐                   ┌────┴─────┐
   │          │                   │          │
NotFound  Validation         DbException  HttpException
Exception Exception           Exception   Exception
```

### 💡 Exemplos:

#### **Domain/Business Exceptions** (4xx):
```csharp
// Cliente errou
NotFoundException          → 404 Not Found
ValidationException        → 400 Bad Request
DuplicateException        → 409 Conflict
UnauthorizedAccessException → 403 Forbidden
```

#### **Infrastructure Exceptions** (5xx):
```csharp
// Servidor errou
DbUpdateException         → 500 Internal Server Error
TimeoutException          → 504 Gateway Timeout
OutOfMemoryException      → 500 Internal Server Error
```

---

## SLIDE 24: Criando Exceções Customizadas

### 📁 Estrutura de Pastas:
```
Application/
  Exceptions/
    NotFoundException.cs
    ValidationException.cs
    BusinessException.cs  ← Base
```

### 🛠️ Implementação:

```csharp
// Base para exceções de negócio
public abstract class BusinessException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BusinessException(
        string message, 
        int statusCode, 
        string errorCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

// Exceção específica
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) 
        : base(message, StatusCodes.Status404NotFound, "NOT_FOUND")
    {
    }
}

public class ValidationException : BusinessException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("Validação falhou", StatusCodes.Status400BadRequest, "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}
```

---

## SLIDE 25: RFC 7807 - Problem Details

### 📖 Padrão da IETF para Erros HTTP

```json
{
  "type": "https://api.exemplo.com/erros/produto-nao-encontrado",
  "title": "Produto não encontrado",
  "status": 404,
  "detail": "O produto com ID 123 não existe no sistema.",
  "instance": "/produtos/123",
  "traceId": "0HN1HKP8ASQQ4:00000001"
}
```

### 🔑 Campos:

| Campo | Descrição | Obrigatório |
|-------|-----------|-------------|
| `type` | URI que identifica o tipo de erro | Não |
| `title` | Resumo curto, legível | Sim |
| `status` | Código HTTP | Sim |
| `detail` | Explicação específica desta ocorrência | Não |
| `instance` | URI da requisição que causou o erro | Não |
| **extensões** | Campos customizados | Não |

---

## SLIDE 26: Problem Details - Exemplos

### ✅ Erro de Validação (400):
```json
{
  "type": "https://api.exemplo.com/erros/validacao",
  "title": "Erros de validação",
  "status": 400,
  "detail": "Um ou mais campos são inválidos.",
  "instance": "/produtos",
  "traceId": "0HN1HKP8ASQQ4:00000001",
  "errors": {
    "preco": ["Preço deve ser maior que zero"],
    "nome": ["Nome é obrigatório", "Nome deve ter no máximo 100 caracteres"]
  }
}
```

### ❌ Erro Interno (500):
```json
{
  "type": "https://api.exemplo.com/erros/erro-interno",
  "title": "Erro interno do servidor",
  "status": 500,
  "detail": "Ocorreu um erro inesperado. Nossa equipe foi notificada.",
  "instance": "/produtos/1",
  "traceId": "0HN1HKP8ASQQ4:00000002"
}
```

---

## SLIDE 27: Implementação - Global Exception Handler

### 🛠️ Opção 1: Middleware Customizado

```csharp
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);  // Chama próximo middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Determinar status code e mensagem
        var (statusCode, errorCode, title, detail) = exception switch
        {
            NotFoundException notFound => 
                (404, "NOT_FOUND", "Recurso não encontrado", notFound.Message),
            
            ValidationException validation => 
                (400, "VALIDATION_ERROR", "Erro de validação", validation.Message),
            
            BusinessException business => 
                (business.StatusCode, business.ErrorCode, "Erro de negócio", business.Message),
            
            _ => 
                (500, "INTERNAL_ERROR", "Erro interno", "Ocorreu um erro inesperado")
        };

        // Logar exceção
        if (statusCode >= 500)
            _logger.LogError(exception, "Erro interno: {Message}", exception.Message);
        else
            _logger.LogWarning("Erro de cliente: {Message}", exception.Message);

        // Montar Problem Details
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://api.exemplo.com/erros/{errorCode.ToLower()}"
        };

        // Adicionar TraceId
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        // Se for ValidationException, adicionar erros
        if (exception is ValidationException validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Errors;
        }

        // Enviar resposta
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

---

## SLIDE 28: Registrando o Middleware

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddProblemDetails();  // Suporte nativo a Problem Details

var app = builder.Build();

// ⚠️ ORDEM IMPORTA! Exception handler PRIMEIRO
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Outros middlewares
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/produtos/{id}", async (int id, IProdutoService service) =>
{
    // ✅ SEM try-catch! Exceções sobem para o middleware
    var produto = await service.ObterAsync(id);
    return Results.Ok(produto);
});

app.Run();
```

---

## SLIDE 29: Opção 2 - IExceptionHandler (.NET 8+)

### 🆕 Interface Nativa do .NET 8

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // Logar
        _logger.LogError(exception, "Exceção capturada: {Message}", exception.Message);

        // Determinar resposta
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (404, "Não encontrado", exception.Message),
            ValidationException => (400, "Validação falhou", exception.Message),
            BusinessException bex => (bex.StatusCode, "Erro de negócio", bex.Message),
            _ => (500, "Erro interno", "Erro inesperado no servidor")
        };

        // Criar Problem Details
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        // Escrever resposta
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;  // Exceção tratada
    }
}

// Registrar no Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// No pipeline
app.UseExceptionHandler();  // ← Usa handlers registrados
```

---

## SLIDE 30: Logging Estruturado

### 📝 Boas Práticas:

```csharp
private async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    // ✅ Log estruturado com contexto
    _logger.LogError(
        exception,
        "Erro ao processar requisição {Method} {Path}. UserId: {UserId}, TraceId: {TraceId}",
        context.Request.Method,
        context.Request.Path,
        context.User?.FindFirst("sub")?.Value ?? "anonymous",
        context.TraceIdentifier
    );

    // ❌ Evitar:
    // _logger.LogError(exception.ToString());  // Sem contexto
    // Console.WriteLine(exception.Message);     // Não estruturado
}
```

### 🔍 Níveis de Log por Tipo de Exceção:

```csharp
var logLevel = exception switch
{
    BusinessException => LogLevel.Warning,      // Cliente errou
    NotFoundException => LogLevel.Information,  // Comum, não é erro grave
    ValidationException => LogLevel.Warning,    // Entrada inválida
    _ => LogLevel.Error                         // Erro real do servidor
};

_logger.Log(logLevel, exception, "Exceção capturada");
```

---

## SLIDE 31: Diferenciando Ambientes

### 🔒 Produção vs Desenvolvimento

```csharp
private Task HandleExceptionAsync(
    HttpContext context, 
    Exception exception, 
    IWebHostEnvironment env)
{
    var statusCode = GetStatusCode(exception);
    
    var problemDetails = new ProblemDetails
    {
        Status = statusCode,
        Title = GetTitle(exception),
        Instance = context.Request.Path,
        Type = $"https://httpstatuses.com/{statusCode}"
    };

    // ⚠️ Em DEV: mostrar stack trace
    if (env.IsDevelopment())
    {
        problemDetails.Detail = exception.Message;
        problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        problemDetails.Extensions["innerException"] = exception.InnerException?.Message;
    }
    // 🔒 Em PROD: mensagem genérica
    else
    {
        problemDetails.Detail = statusCode >= 500
            ? "Ocorreu um erro interno. Nossa equipe foi notificada."
            : exception.Message;  // Erros 4xx são seguros de expor
    }

    // ...
}
```

---

## SLIDE 32: Testando Exception Handling

### 🧪 Cenários de Teste:

```http
### 1. Produto não encontrado (404)
GET https://localhost:5001/produtos/999

Esperado:
{
  "status": 404,
  "title": "Produto não encontrado",
  "detail": "Produto 999 não encontrado",
  "instance": "/produtos/999",
  "traceId": "..."
}

### 2. Validação falhou (400)
POST https://localhost:5001/produtos
Content-Type: application/json

{
  "nome": "",
  "preco": -10
}

Esperado:
{
  "status": 400,
  "title": "Erros de validação",
  "errors": {
    "nome": ["Nome é obrigatório"],
    "preco": ["Preço deve ser maior que zero"]
  }
}

### 3. Erro interno (500) - forçar erro
GET https://localhost:5001/produtos/crash

Esperado:
{
  "status": 500,
  "title": "Erro interno do servidor",
  "detail": "Ocorreu um erro inesperado"
}
```

---

## SLIDE 33: Vantagens do Tratamento Centralizado

### ✅ Benefícios:

1. **DRY (Don't Repeat Yourself)**
   - Código de tratamento em 1 lugar
   
2. **Consistência**
   - Todos endpoints retornam erros no mesmo formato
   
3. **Manutenibilidade**
   - Mudar formato de erro = 1 alteração
   
4. **Rastreabilidade**
   - Logs centralizados com TraceId
   
5. **Segurança**
   - Ocultar stack traces em produção facilmente
   
6. **Padrões da Indústria**
   - Problem Details (RFC 7807)
   
7. **Monitoramento**
   - Integrar com Sentry, Application Insights, etc.

---

## SLIDE 34: Fluxo Completo - Diagrama

```
┌─────────────────────────────────────────────────┐
│ Cliente: POST /produtos { nome: "", preco: -5 } │
└───────────────────┬─────────────────────────────┘
                    │
        ┌───────────▼──────────────┐
        │  Exception Middleware    │
        │  (try-catch global)      │
        └───────────┬──────────────┘
                    │
        ┌───────────▼──────────────┐
        │  Endpoint Handler        │
        │  (chama Service)         │
        └───────────┬──────────────┘
                    │
        ┌───────────▼──────────────┐
        │  ProdutoService.Criar    │
        │  Validações...           │
        │  ❌ throw ValidationEx   │
        └───────────┬──────────────┘
                    │ Exception sobe!
        ┌───────────▼──────────────┐
        │  Exception Middleware    │
        │  Captura ValidationEx    │
        │  - Loga warning          │
        │  - Monta Problem Details │
        │  - StatusCode = 400      │
        └───────────┬──────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│ Response: 400 Bad Request                       │
│ {                                               │
│   "status": 400,                                │
│   "title": "Erro de validação",                 │
│   "errors": {                                   │
│     "nome": ["Nome é obrigatório"],             │
│     "preco": ["Preço deve ser maior que zero"]  │
│   }                                             │
│ }                                               │
└─────────────────────────────────────────────────┘
```

---

## SLIDE 35: Checklist de Implementação

### ✅ Exception Handling Completo:

- [ ] Criar exceções customizadas (BusinessException base)
- [ ] Implementar GlobalExceptionHandler (Middleware ou IExceptionHandler)
- [ ] Registrar no pipeline ANTES de outros middlewares
- [ ] Configurar Problem Details (RFC 7807)
- [ ] Adicionar logging estruturado
- [ ] Diferenciar DEV (stack trace) vs PROD (mensagem genérica)
- [ ] Incluir TraceId em todas respostas de erro
- [ ] Remover try-catch dos endpoints
- [ ] Documentar códigos de erro na API
- [ ] Testar todos cenários (404, 400, 500)

---

## SLIDE 36: Exercícios Práticos

### 🎯 Desafios para Alunos:

#### **Exercício 1: Implementar PUT/PATCH**
1. Criar DTOs (ProdutoUpdateDto e ProdutoPatchDto)
2. Implementar métodos no Service
3. Adicionar endpoints no Program.cs
4. Testar com REST Client

#### **Exercício 2: Exceções Customizadas**
1. Criar DuplicateProductException (409)
2. Criar InsufficientStockException (422)
3. Lançar exceções apropriadas no Service

#### **Exercício 3: Global Exception Handler**
1. Implementar middleware customizado
2. Mapear exceções para status codes
3. Retornar Problem Details
4. Adicionar logging

#### **Exercício 4: Testes**
1. Testar PUT com payload completo
2. Testar PATCH com campo único
3. Forçar erro 404, 400, 500
4. Validar formato Problem Details

---

## SLIDE 37: Recursos Adicionais

### 📚 Leitura Obrigatória:
- [RFC 7231 - HTTP PUT](https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.4)
- [RFC 5789 - HTTP PATCH](https://datatracker.ietf.org/doc/html/rfc5789)
- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [ASP.NET Core Error Handling](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)

### 🎥 Vídeos Recomendados:
- REST API Best Practices (PUT vs PATCH)
- Exception Handling in ASP.NET Core
- Problem Details Standard

### 💻 Repositório:
- Código completo: [github.com/lucasfogacadj/padroes-de-projeto-DB-Terca]

---

## SLIDE 38: Próxima Aula - Preview

### 🔮 O que vem por aí:

1. **FluentValidation**
   - Validações complexas e reutilizáveis
   - Integração com ASP.NET Core

2. **Logging Avançado (Serilog)**
   - Structured logging
   - Sinks (arquivo, console, cloud)

3. **Paginação e Filtros**
   - Query parameters
   - Performance com grandes datasets

4. **Testes Unitários**
   - xUnit + Moq
   - Testar Services isoladamente

---

## SLIDE 39: Perguntas Frequentes

### ❓ PUT pode criar recurso se não existir?
**R:** Sim, é chamado de "upsert". Retorne 201 se criou, 200 se atualizou.

### ❓ PATCH deve ser sempre idempotente?
**R:** Sim! Use valores absolutos, não operações relativas (+=, -=).

### ❓ Posso misturar PUT e PATCH na mesma API?
**R:** Sim! Muitas APIs oferecem ambos. Documente claramente.

### ❓ Middleware vs IExceptionHandler?
**R:** IExceptionHandler é mais novo (.NET 8+) e recomendado. Middleware para .NET < 8.

### ❓ Devo retornar stack trace em produção?
**R:** NUNCA! É risco de segurança. Use apenas em DEV.

### ❓ Como lidar com exceções assíncronas?
**R:** Middleware captura automaticamente. Use async/await corretamente.

---

## SLIDE 40: Encerramento

### 🎯 O que aprendemos hoje:

✅ Diferença entre PUT (total) e PATCH (parcial)  
✅ Quando usar cada método HTTP  
✅ Idempotência e suas implicações  
✅ Tipos de exceções (negócio vs infraestrutura)  
✅ Middleware pipeline no ASP.NET Core  
✅ Problem Details (RFC 7807)  
✅ Global Exception Handler  
✅ Logging estruturado  

### 📝 Tarefa de Casa:
1. Implementar PUT e PATCH na API de Produtos
2. Criar 3 exceções customizadas
3. Implementar Global Exception Handler
4. Testar todos cenários com REST Client
5. Commitar no branch `feature/update-exception-handling`

### 📅 Próxima Aula: FluentValidation + Testes Unitários

---

**Dúvidas?** 🙋‍♂️

**Contato:** professor@faculdade.edu.br  
**Office Hours:** Terças 14h-16h

---

# FIM 🎉
