# 🛡️ Exceções Customizadas

## 📋 Visão Geral

Esta pasta contém as exceções customizadas de domínio/negócio da aplicação. Seguindo boas práticas, separamos exceções de negócio (esperadas, resultado de validações) de exceções técnicas (inesperadas, bugs).

---

## 🏗️ Hierarquia de Exceções

```
Exception (System)
    │
    └── BusinessException (Base - Abstract)
            │
            ├── NotFoundException (404)
            ├── ValidationException (400)
            └── DuplicateException (409)
```

---

## 📦 Exceções Disponíveis

### 1. `BusinessException` (Abstract Base Class)

**Propósito:** Classe base para todas as exceções de negócio.

**Propriedades:**
- `StatusCode` (int): Código HTTP correspondente
- `ErrorCode` (string): Código único do erro (ex: "NOT_FOUND")

**Quando usar:** Não usar diretamente. Criar subclasses específicas.

**Exemplo:**
```csharp
public class MinhaExcecaoCustomizada : BusinessException
{
    public MinhaExcecaoCustomizada(string message) 
        : base(message, 422, "CUSTOM_ERROR")
    {
    }
}
```

---

### 2. `NotFoundException` - HTTP 404

**Propósito:** Indica que um recurso solicitado não foi encontrado.

**Status Code:** 404 Not Found  
**Error Code:** `NOT_FOUND`

**Quando usar:**
- Buscar produto por ID que não existe
- Tentar atualizar/deletar recurso inexistente
- Relação (foreign key) não encontrada

**Exemplos de Uso:**

```csharp
// Opção 1: Mensagem customizada
throw new NotFoundException("O produto solicitado não foi encontrado.");

// Opção 2: Template com nome do recurso e chave
throw new NotFoundException("Produto", 123);
// Mensagem gerada: "Produto com ID '123' não foi encontrado."
```

**Resposta HTTP:**
```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Produto com ID '123' não foi encontrado.",
  "errorCode": "NOT_FOUND",
  "traceId": "..."
}
```

---

### 3. `ValidationException` - HTTP 400

**Propósito:** Indica que a validação de entrada falhou.

**Status Code:** 400 Bad Request  
**Error Code:** `VALIDATION_ERROR`

**Quando usar:**
- Campo obrigatório ausente
- Valor fora do range permitido
- Formato inválido (email, CPF, etc.)
- Regras de negócio violadas (ex: preço <= 0)

**Propriedades Especiais:**
- `Errors` (Dictionary<string, string[]>): Mapa de campo → erros

**Exemplos de Uso:**

```csharp
// Opção 1: Mensagem simples
throw new ValidationException("Dados inválidos fornecidos.");

// Opção 2: Campo + mensagem específica
throw new ValidationException("preco", "O preço deve ser maior que zero.");

// Opção 3: Múltiplos erros
var errors = new Dictionary<string, string[]>
{
    { "nome", new[] { "Nome é obrigatório", "Nome deve ter entre 3-100 caracteres" } },
    { "preco", new[] { "Preço deve ser maior que zero" } }
};
throw new ValidationException(errors);
```

**Resposta HTTP:**
```json
{
  "status": 400,
  "title": "Erro de validação",
  "detail": "Erro de validação no campo 'preco': O preço deve ser maior que zero.",
  "errorCode": "VALIDATION_ERROR",
  "traceId": "...",
  "errors": {
    "preco": ["O preço deve ser maior que zero."]
  }
}
```

---

### 4. `DuplicateException` - HTTP 409

**Propósito:** Indica tentativa de criar recurso que já existe (violação de unicidade).

**Status Code:** 409 Conflict  
**Error Code:** `DUPLICATE`

**Quando usar:**
- Tentar criar produto com nome duplicado
- Violação de UNIQUE constraint
- Email já cadastrado
- SKU/código já existe

**Exemplos de Uso:**

```csharp
// Opção 1: Mensagem customizada
throw new DuplicateException("Já existe um produto com este nome.");

// Opção 2: Template com recurso, campo e valor
throw new DuplicateException("Produto", "nome", "Notebook Dell");
// Mensagem gerada: "Produto com nome 'Notebook Dell' já existe."
```

**Resposta HTTP:**
```json
{
  "status": 409,
  "title": "Recurso duplicado",
  "detail": "Produto com nome 'Notebook Dell' já existe.",
  "errorCode": "DUPLICATE",
  "traceId": "..."
}
```

---

## 🎯 Quando Usar Cada Exceção

### ✅ Use `NotFoundException` quando:
- [ ] `GetById()` retorna null
- [ ] Foreign key não encontrada
- [ ] Recurso deletado ou inexistente

### ✅ Use `ValidationException` quando:
- [ ] Dados de entrada inválidos
- [ ] Regra de negócio violada
- [ ] Campo obrigatório ausente
- [ ] Valor fora do range

### ✅ Use `DuplicateException` quando:
- [ ] Violação de UNIQUE constraint
- [ ] Tentativa de criar recurso já existente
- [ ] Conflito de chave natural

### ❌ NÃO use Business Exceptions para:
- Erros de banco de dados (DbException)
- Timeout de rede
- OutOfMemoryException
- NullReferenceException (é bug!)
- Erros de autenticação/autorização (use UnauthorizedAccessException nativo)

---

## 🔄 Fluxo de Tratamento

```
1. Service Layer lança exceção
        │
        ↓
2. Exceção sobe pelo pipeline
        │
        ↓
3. GlobalExceptionHandlerMiddleware captura
        │
        ↓
4. Middleware verifica tipo da exceção
        │
        ↓
5. Converte para Problem Details
        │
        ↓
6. Loga com nível apropriado
        │
        ↓
7. Retorna JSON padronizado ao cliente
```

---

## 💡 Boas Práticas

### ✅ DO:
- Lançar exceções específicas (não `BusinessException` direta)
- Incluir mensagem clara e acionável
- Usar construtores apropriados
- Documentar quando/por que exceção é lançada
- Testar cenários de exceção

### ❌ DON'T:
- Usar exceções para fluxo de controle normal
- Lançar `Exception` genérica
- Expor detalhes internos (stack traces, SQL, etc.)
- Logar exceção E relançar (duplicação de logs)
- Criar exceção para cada regra de negócio (reuse!)

---

## 🧪 Exemplos de Uso no Service

### Exemplo Completo: ProdutoService

```csharp
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repo;

    public async Task<Produto> ObterAsync(int id, CancellationToken ct)
    {
        // Validação de entrada
        if (id <= 0)
            throw new ValidationException("id", "ID deve ser maior que zero.");

        // Buscar no repositório
        var produto = await _repo.GetByIdAsync(id, ct);
        
        // Verificar existência
        if (produto == null)
            throw new NotFoundException("Produto", id);

        return produto;
    }

    public async Task<Produto> CriarAsync(ProdutoCreateDto dto, CancellationToken ct)
    {
        // Verificar duplicação
        var existe = await _repo.ExisteComNomeAsync(dto.Nome, ct);
        if (existe)
            throw new DuplicateException("Produto", "nome", dto.Nome);

        // Validar regras de negócio
        if (dto.Preco <= 0)
            throw new ValidationException("preco", "Preço deve ser maior que zero.");

        if (dto.Estoque < 0)
            throw new ValidationException("estoque", "Estoque não pode ser negativo.");

        // Criar e persistir
        var produto = ProdutoFactory.Criar(dto);
        await _repo.AddAsync(produto, ct);
        await _repo.SaveChangesAsync(ct);

        return produto;
    }
}
```

---

## 📊 Mapeamento Status Code → Exceção

| Status | Exceção | Significado | Cliente pode resolver? |
|--------|---------|-------------|------------------------|
| 400 | ValidationException | Entrada inválida | ✅ Sim (corrigir dados) |
| 404 | NotFoundException | Recurso não existe | ✅ Sim (usar ID válido) |
| 409 | DuplicateException | Recurso já existe | ✅ Sim (usar outro valor) |
| 422 | BusinessException | Regra de negócio | ✅ Talvez (depende da regra) |
| 500 | Exception (genérica) | Erro do servidor | ❌ Não (problema interno) |

---

## 🔮 Próximas Exceções Sugeridas

### `InsufficientStockException` (422)
```csharp
public class InsufficientStockException : BusinessException
{
    public InsufficientStockException(int disponivelQty, int solicitadoQty)
        : base(
            $"Estoque insuficiente. Disponível: {disponivelQty}, Solicitado: {solicitadoQty}",
            StatusCodes.Status422UnprocessableEntity,
            "INSUFFICIENT_STOCK")
    {
    }
}
```

### `UnauthorizedOperationException` (403)
```csharp
public class UnauthorizedOperationException : BusinessException
{
    public UnauthorizedOperationException(string operation)
        : base(
            $"Você não tem permissão para: {operation}",
            StatusCodes.Status403Forbidden,
            "UNAUTHORIZED_OPERATION")
    {
    }
}
```

### `RateLimitExceededException` (429)
```csharp
public class RateLimitExceededException : BusinessException
{
    public RateLimitExceededException(int retryAfterSeconds)
        : base(
            $"Limite de requisições excedido. Tente novamente em {retryAfterSeconds}s.",
            StatusCodes.Status429TooManyRequests,
            "RATE_LIMIT_EXCEEDED")
    {
    }
}
```

---

## 📚 Referências

- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [HTTP Status Codes](https://httpstatuses.com/)
- [Exception Best Practices - Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

---

## 🎓 Exercícios para Alunos

1. **Criar exceção customizada**: Implementar `InvalidOperationException` (400) para operações inválidas em produto (ex: ativar produto já ativo)

2. **Múltiplos erros**: Modificar `CriarAsync` para acumular TODOS os erros de validação antes de lançar `ValidationException`

3. **Testes unitários**: Criar testes que verificam se exceções corretas são lançadas

4. **Localização**: Adicionar suporte a múltiplos idiomas nas mensagens de erro

---

**Atualizado em:** 04/11/2025
