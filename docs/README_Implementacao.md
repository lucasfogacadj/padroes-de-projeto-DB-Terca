# 🎓 Aula: PUT vs PATCH & Exception Handling

## 📚 Conteúdo Implementado

Esta implementação adiciona à API de Produtos:
1. ✅ Endpoints PUT e PATCH para atualização de produtos
2. ✅ Sistema centralizado de tratamento de exceções (Middleware)
3. ✅ Exceções customizadas de negócio
4. ✅ Padrão Problem Details (RFC 7807)
5. ✅ Logging estruturado

---

## 🔄 PUT vs PATCH - Diferenças Implementadas

### PUT - Substituição Total (`/produtos/{id}`)

**Características:**
- ✅ Todos os campos são **obrigatórios**
- ✅ Substitui **completamente** o recurso
- ✅ Se omitir campo → erro 400 Bad Request
- ✅ Idempotente (executar N vezes = mesmo resultado)

**Uso:**
```http
PUT /produtos/1
Content-Type: application/json

{
  "nome": "Notebook Dell XPS 15",
  "descricao": "Intel i7, 16GB RAM, SSD 512GB",
  "preco": 5500.00,
  "estoque": 8
}
```

**DTO Utilizado:** `ProdutoUpdateDto` (todos campos `[Required]`)

---

### PATCH - Atualização Parcial (`/produtos/{id}`)

**Características:**
- ✅ Todos os campos são **opcionais** (nullable)
- ✅ Atualiza **apenas** campos enviados
- ✅ Campos não enviados **permanecem inalterados**
- ✅ Idempotente (valores absolutos, não operações)

**Uso:**
```http
PATCH /produtos/1
Content-Type: application/json

{
  "preco": 5200.00
}
```

Resultado: Apenas `preco` é atualizado, `nome`, `descricao` e `estoque` permanecem iguais.

**DTO Utilizado:** `ProdutoPatchDto` (todos campos `nullable`)

---

## 🛡️ Exception Handling - Arquitetura

### Exceções Customizadas

Criadas na pasta `Application/Exceptions/`:

#### 1. `BusinessException` (Base)
```csharp
public abstract class BusinessException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
}
```

#### 2. `NotFoundException` (404)
```csharp
throw new NotFoundException("Produto", id);
// Resultado: HTTP 404 Not Found
```

#### 3. `ValidationException` (400)
```csharp
throw new ValidationException("preco", "O preço deve ser maior que zero.");
// Resultado: HTTP 400 Bad Request
```

#### 4. `DuplicateException` (409)
```csharp
throw new DuplicateException("Produto", "nome", "Notebook Dell");
// Resultado: HTTP 409 Conflict
```

---

### Global Exception Handler Middleware

Localização: `Middleware/GlobalExceptionHandlerMiddleware.cs`

**Responsabilidades:**
1. ✅ Capturar todas exceções não tratadas
2. ✅ Logar com nível apropriado (Error para 5xx, Warning para 4xx)
3. ✅ Converter exceções em respostas HTTP padronizadas
4. ✅ Retornar Problem Details (RFC 7807)
5. ✅ Incluir TraceId para rastreabilidade
6. ✅ Ocultar stack traces em produção

**Registro no Pipeline:**
```csharp
// Program.cs
app.UseGlobalExceptionHandler(); // ← PRIMEIRO middleware!
```

---

## 📋 Problem Details (RFC 7807)

Todas as respostas de erro seguem o padrão:

### Exemplo - 404 Not Found
```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Produto com ID '999' não foi encontrado.",
  "instance": "/produtos/999",
  "type": "https://httpstatuses.com/404",
  "traceId": "0HN1HKP8ASQQ4:00000001",
  "errorCode": "NOT_FOUND"
}
```

### Exemplo - 400 Validation Error
```json
{
  "status": 400,
  "title": "Erro de validação",
  "detail": "Erro de validação no campo 'preco': O preço deve ser maior que zero.",
  "instance": "/produtos",
  "type": "https://httpstatuses.com/400",
  "traceId": "0HN1HKP8ASQQ4:00000002",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "preco": ["O preço deve ser maior que zero."]
  }
}
```

### Exemplo - 500 Internal Error (DEV)
```json
{
  "status": 500,
  "title": "Erro interno do servidor",
  "detail": "Object reference not set to an instance of an object.",
  "instance": "/produtos/1",
  "type": "https://httpstatuses.com/500",
  "traceId": "0HN1HKP8ASQQ4:00000003",
  "errorCode": "INTERNAL_ERROR",
  "stackTrace": "at ...",
  "exceptionType": "NullReferenceException"
}
```

### Exemplo - 500 Internal Error (PROD)
```json
{
  "status": 500,
  "title": "Erro interno do servidor",
  "detail": "Ocorreu um erro inesperado. Nossa equipe foi notificada.",
  "instance": "/produtos/1",
  "type": "https://httpstatuses.com/500",
  "traceId": "0HN1HKP8ASQQ4:00000003",
  "errorCode": "INTERNAL_ERROR"
}
```

---

## 🏗️ Estrutura de Arquivos Adicionados

```
padroes-de-projeto-DB-Terca/
├── Application/
│   ├── DTOs/
│   │   ├── ProdutoUpdateDto.cs      ← PUT (campos obrigatórios)
│   │   └── ProdutoPatchDto.cs       ← PATCH (campos opcionais)
│   ├── Exceptions/
│   │   ├── BusinessException.cs     ← Base
│   │   ├── NotFoundException.cs     ← 404
│   │   ├── ValidationException.cs   ← 400
│   │   └── DuplicateException.cs    ← 409
│   ├── Interfaces/
│   │   ├── IProdutoService.cs       ← Atualizado (AtualizarAsync, AtualizarParcialAsync)
│   │   └── IProdutoRepository.cs    ← Atualizado (UpdateAsync)
│   └── Services/
│       └── ProdutoService.cs        ← Atualizado (métodos PUT/PATCH)
├── Infrastructure/
│   └── Repositories/
│       └── ProdutoRepository.cs     ← Atualizado (UpdateAsync)
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs  ← Novo!
├── docs/
│   ├── Aula_PUT_PATCH_ExceptionHandling.md  ← Slides da aula
│   └── README_Implementacao.md              ← Este arquivo
├── APIProdutos_Completo.http        ← 38 testes prontos!
└── Program.cs                       ← Atualizado (endpoints + middleware)
```

---

## 🧪 Como Testar

### Opção 1: VS Code REST Client

1. Instalar extensão: `REST Client` (humao.rest-client)
2. Abrir arquivo: `APIProdutos_Completo.http`
3. Clicar em "Send Request" acima de cada teste

### Opção 2: Postman

Importar coleção: `APIProdutos.postman_collection.json`

### Opção 3: Swagger/OpenAPI

1. Executar: `dotnet run`
2. Acessar: `https://localhost:5001/openapi/v1.json`

---

## 📊 Cenários de Teste Implementados

O arquivo `APIProdutos_Completo.http` contém **38 testes** organizados em:

### Parte 1: Operações Básicas
- Listar produtos
- Obter produto por ID
- Criar produto

### Parte 2: PUT - Atualização Completa
- ✅ PUT com sucesso (todos campos)
- ❌ PUT sem campo obrigatório (erro 400)
- ❌ PUT com preço inválido (erro 400)
- ❌ PUT produto inexistente (erro 404)

### Parte 3: PATCH - Atualização Parcial
- ✅ PATCH apenas preço
- ✅ PATCH nome e descrição
- ✅ PATCH apenas estoque
- ✅ PATCH múltiplos campos
- ❌ PATCH preço inválido (erro 400)
- ❌ PATCH nome vazio (erro 400)
- ❌ PATCH produto inexistente (erro 404)

### Parte 4: Exception Handling
- 404 - Produto não encontrado
- 400 - ID inválido
- 400 - Validações diversas (nome vazio, preço negativo, etc.)

### Parte 5: Comparação PUT vs PATCH
- Demonstra diferenças práticas entre os métodos

### Parte 6: Idempotência
- Testa execução múltipla do mesmo request

### Parte 7: Edge Cases
- Objeto vazio, valores limite, trim de strings

### Parte 8: DELETE
- Completude das operações CRUD

---

## 🎯 Objetivos Pedagógicos Atingidos

### 1. PUT vs PATCH
- ✅ Alunos entendem quando usar cada método
- ✅ Diferença entre substituição total vs parcial
- ✅ Implementação prática com DTOs diferentes
- ✅ Validações apropriadas para cada caso

### 2. Exception Handling
- ✅ Separação entre exceções de negócio (4xx) e infraestrutura (5xx)
- ✅ Tratamento centralizado (DRY - Don't Repeat Yourself)
- ✅ Padrão da indústria (RFC 7807)
- ✅ Logging estruturado
- ✅ Segurança (ocultar stack traces em produção)

### 3. Boas Práticas
- ✅ Separation of Concerns (Service → Repository)
- ✅ Single Responsibility Principle
- ✅ Idempotência
- ✅ Documentação com OpenAPI
- ✅ Status Codes HTTP corretos

---

## 🔍 Fluxo de Execução Completo

### Exemplo: PUT com Erro de Validação

```
1. Cliente envia: PUT /produtos/1 { preco: -100 }
       ↓
2. Program.cs → Endpoint Handler
       ↓
3. Chama: ProdutoService.AtualizarAsync(1, dto)
       ↓
4. Service valida: preco <= 0
       ↓
5. Lança: throw new ValidationException("preco", "Preço deve ser maior que zero")
       ↓
6. Exceção sobe até GlobalExceptionHandlerMiddleware
       ↓
7. Middleware:
   - Detecta ValidationException
   - Loga warning
   - StatusCode = 400
   - Monta Problem Details
       ↓
8. Retorna ao cliente:
   {
     "status": 400,
     "title": "Erro de validação",
     "detail": "Erro de validação no campo 'preco': O preço deve ser maior que zero.",
     "errors": { "preco": ["O preço deve ser maior que zero."] },
     "traceId": "..."
   }
```

---

## 💡 Conceitos Avançados para Próximas Aulas

### 1. Concorrência (Optimistic Locking)
- Adicionar campo `RowVersion` ou `ETag`
- Retornar 409 Conflict se versão mudou

### 2. FluentValidation
- Substituir validações manuais por validators reutilizáveis
- Mensagens de erro customizadas e localizadas

### 3. PATCH com JSON Patch (RFC 6902)
```json
PATCH /produtos/1
[
  { "op": "replace", "path": "/preco", "value": 100 },
  { "op": "add", "path": "/tags", "value": "promocao" }
]
```

### 4. Rate Limiting
- Prevenir abuso de endpoints de atualização

### 5. Audit Trail
- Registrar quem/quando/o quê foi alterado

---

## 📝 Exercícios para Alunos

### Exercício 1: Criar nova exceção
Implementar `InsufficientStockException` (422 Unprocessable Entity)

### Exercício 2: Adicionar validação
Não permitir atualizar preço para mais de 50% do valor atual (regra anti-erro)

### Exercício 3: Testar concorrência
Usar 2 clientes simulando atualização simultânea do mesmo produto

### Exercício 4: Logging avançado
Adicionar Serilog e logar em arquivo

### Exercício 5: Documentação
Adicionar exemplos de request/response no Swagger

---

## 🚀 Como Executar

```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Executar migrations (se necessário)
dotnet ef database update

# 3. Executar aplicação
dotnet run

# 4. Acessar Swagger (se habilitado)
https://localhost:5001/swagger

# 5. Testar endpoints
# Usar arquivo APIProdutos_Completo.http
```

---

## 📚 Referências

- [RFC 7231 - HTTP PUT](https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.4)
- [RFC 5789 - HTTP PATCH](https://datatracker.ietf.org/doc/html/rfc5789)
- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
- [ASP.NET Core Error Handling](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)

---

## ❓ Perguntas Frequentes

### Q: Por que usar exceções customizadas?
**R:** Para separar erros de negócio (esperados) de erros técnicos (inesperados) e facilitar tratamento específico.

### Q: PATCH pode enviar null explicitamente?
**R:** Depende da convenção. Nesta implementação, null = ignorar campo. Para "limpar" campo, use string vazia.

### Q: Por que não usar AutoMapper?
**R:** Para fins pedagógicos, mapping manual é mais claro. AutoMapper pode ser adicionado depois.

### Q: Middleware vs Filter vs IExceptionHandler?
**R:** 
- **Middleware**: Mais baixo nível, captura tudo
- **Filter**: Específico para MVC/API Controllers
- **IExceptionHandler**: Novo no .NET 8+, recomendado

### Q: Como testar o middleware?
**R:** Criar testes de integração com WebApplicationFactory mockando cenários de exceção.

---

**Implementado por:** Lucas Fogaça  
**Data:** 04/11/2025  
**Versão:** 1.0
