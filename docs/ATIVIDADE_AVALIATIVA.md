# 📝 ATIVIDADE AVALIATIVA - PUT/PATCH & Exception Handling

**Disciplina:** Desenvolvimento Backend  
**Professor:** Lucas Fogaça  
**Data de Entrega:** 11/11/2025  
**Valor:** 10,0 pontos  
**Modalidade:** Individual

---

## 🎯 OBJETIVOS DE APRENDIZAGEM

Ao completar esta atividade, você demonstrará capacidade de:
- ✅ Implementar endpoints PUT e PATCH corretamente
- ✅ Criar e utilizar exceções customizadas
- ✅ Aplicar tratamento centralizado de erros
- ✅ Diferenciar quando usar cada método HTTP
- ✅ Testar APIs REST de forma profissional

---

## 📋 PARTE 1: QUESTÕES TEÓRICAS (2,0 pontos)

### Questão 1 (0,5 pontos)
Explique com suas palavras a diferença entre PUT e PATCH. Dê um exemplo prático de quando usar cada um.

**Resposta esperada (mínimo 5 linhas):**

```
[Espaço para resposta]








```

---

### Questão 2 (0,5 pontos)
O que é idempotência? Por que é importante em APIs REST? Dê um exemplo de operação idempotente e uma não-idempotente.

**Resposta esperada (mínimo 4 linhas):**

```
[Espaço para resposta]







```

---

### Questão 3 (0,5 pontos)
Explique o padrão Problem Details (RFC 7807). Quais são os campos obrigatórios e por que este padrão é importante?

**Resposta esperada (mínimo 4 linhas):**

```
[Espaço para resposta]







```

---

### Questão 4 (0,5 pontos)
Por que centralizar tratamento de exceções em um middleware é melhor do que usar try-catch em cada endpoint? Liste 3 vantagens.

**Resposta:**

```
1. [Vantagem 1]

2. [Vantagem 2]

3. [Vantagem 3]
```

---

## 💻 PARTE 2: IMPLEMENTAÇÃO PRÁTICA (6,0 pontos)

### Contexto:
Você foi contratado para evoluir uma API de **Biblioteca**. A entidade `Livro` já existe:

```csharp
public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string ISBN { get; set; }
    public int AnoPublicacao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public bool Disponivel { get; set; }
    public DateTime DataCadastro { get; set; }
}
```

### Tarefa 2.1: Criar DTOs (1,0 ponto)

Crie os DTOs necessários para:
- **PUT** (atualização completa de livro)
- **PATCH** (atualização parcial de livro)

**Requisitos:**
- PUT: Todos campos obrigatórios (exceto Id e DataCadastro)
- PATCH: Todos campos opcionais
- Validações apropriadas com DataAnnotations

**Arquivo:** `Application/DTOs/LivroUpdateDto.cs` e `LivroPatchDto.cs`

---

### Tarefa 2.2: Criar Exceções (1,5 pontos)

Crie 3 exceções customizadas:

1. **`LivroNotFoundException`** (404)
   - Herda de `BusinessException`
   - Mensagem: "Livro com ID '{id}' não foi encontrado."

2. **`ISBNDuplicadoException`** (409)
   - Herda de `BusinessException`
   - Mensagem: "Já existe um livro cadastrado com ISBN '{isbn}'."

3. **`LivroIndisponivelException`** (422)
   - Herda de `BusinessException`
   - Mensagem: "Livro '{titulo}' está indisponível para empréstimo."

**Arquivo:** `Application/Exceptions/LivroNotFoundException.cs` (e demais)

---

### Tarefa 2.3: Implementar Service (2,0 pontos)

Implemente os métodos no `LivroService`:

```csharp
public async Task<Livro> AtualizarAsync(int id, LivroUpdateDto dto, CancellationToken ct)
{
    // TODO: Implementar PUT
    // 1. Buscar livro (lançar LivroNotFoundException se não existir)
    // 2. Validar ISBN duplicado (lançar ISBNDuplicadoException)
    // 3. Substituir TODOS os campos
    // 4. Persistir e retornar
}

public async Task<Livro> AtualizarParcialAsync(int id, LivroPatchDto dto, CancellationToken ct)
{
    // TODO: Implementar PATCH
    // 1. Buscar livro (lançar LivroNotFoundException se não existir)
    // 2. Atualizar APENAS campos enviados (não-null)
    // 3. Validar regras de negócio conforme campos atualizados
    // 4. Persistir e retornar
}
```

**Validações obrigatórias:**
- Título não pode ser vazio
- Ano de publicação entre 1500 e ano atual
- Preço maior que zero
- Quantidade disponível não negativa
- ISBN formato válido (13 dígitos)

**Arquivo:** `Application/Services/LivroService.cs`

---

### Tarefa 2.4: Criar Endpoints (1,0 ponto)

Adicione no `Program.cs`:

```csharp
// PUT /livros/{id} - Atualização completa
app.MapPut("/livros/{id}", async (...) => 
{
    // TODO: Implementar
})
.WithName("AtualizarLivroCompleto")
.WithOpenApi()
.WithSummary("Atualiza um livro completamente (PUT)")
.Produces<Livro>(200)
.Produces<ProblemDetails>(404)
.Produces<ProblemDetails>(400);

// PATCH /livros/{id} - Atualização parcial
app.MapPatch("/livros/{id}", async (...) => 
{
    // TODO: Implementar
})
.WithName("AtualizarLivroParcial")
.WithOpenApi()
.WithSummary("Atualiza um livro parcialmente (PATCH)")
.Produces<Livro>(200)
.Produces<ProblemDetails>(404)
.Produces<ProblemDetails>(400);
```

---

### Tarefa 2.5: Global Exception Handler (0,5 pontos)

Modifique o `GlobalExceptionHandlerMiddleware` para tratar as novas exceções:
- `LivroNotFoundException` → 404
- `ISBNDuplicadoException` → 409
- `LivroIndisponivelException` → 422

**Arquivo:** `Middleware/GlobalExceptionHandlerMiddleware.cs`

---

## 🧪 PARTE 3: TESTES (2,0 pontos)

### Tarefa 3: Criar Arquivo de Testes HTTP

Crie arquivo `Livros.http` com **mínimo 12 testes**:

**Obrigatórios:**
1. PUT com sucesso (todos campos)
2. PUT sem campo obrigatório (erro 400)
3. PUT com ano inválido (erro 400)
4. PUT livro inexistente (erro 404)
5. PUT com ISBN duplicado (erro 409)
6. PATCH apenas título
7. PATCH apenas preço
8. PATCH múltiplos campos
9. PATCH livro inexistente (erro 404)
10. PATCH com preço negativo (erro 400)
11. Idempotência PUT (executar 3x)
12. Idempotência PATCH (executar 3x)

**Formato esperado:**
```http
### 1. PUT com sucesso
PUT https://localhost:5001/livros/1
Content-Type: application/json

{
  "titulo": "Clean Code",
  "autor": "Robert C. Martin",
  "isbn": "9780132350884",
  "anoPublicacao": 2008,
  "preco": 89.90,
  "quantidadeDisponivel": 5,
  "disponivel": true
}

### Resultado esperado: 200 OK
```

---

## 📊 RUBRICA DE AVALIAÇÃO

### Parte 1: Questões Teóricas (2,0 pontos)
| Critério | Excelente (100%) | Bom (75%) | Regular (50%) | Insuficiente (25%) |
|----------|------------------|-----------|---------------|-------------------|
| Clareza | Explicação clara e precisa | Explicação adequada | Explicação confusa | Não respondeu |
| Profundidade | Aborda conceitos avançados | Aborda básico | Superficial | Incorreto |
| Exemplos | Exemplos relevantes | Exemplos genéricos | Sem exemplos | - |

### Parte 2: Implementação (6,0 pontos)
| Item | Pontos | Critérios de Avaliação |
|------|--------|------------------------|
| DTOs | 1,0 | ✅ Campos corretos<br>✅ Validações adequadas<br>✅ Nullable onde apropriado |
| Exceções | 1,5 | ✅ Herança correta<br>✅ Status codes apropriados<br>✅ Mensagens claras |
| Service | 2,0 | ✅ Lógica correta PUT/PATCH<br>✅ Validações implementadas<br>✅ Exceções lançadas corretamente |
| Endpoints | 1,0 | ✅ Assinaturas corretas<br>✅ Documentação OpenAPI<br>✅ Status codes |
| Middleware | 0,5 | ✅ Mapeamento exceções<br>✅ Problem Details |

### Parte 3: Testes (2,0 pontos)
| Critério | Pontos |
|----------|--------|
| Cobertura (12 testes) | 1,0 |
| Formato correto | 0,5 |
| Documentação (comentários) | 0,5 |

---

## 📦 ENTREGA

### Formato:
1. **Fork** do repositório `padroes-de-projeto-DB-Terca`
2. Criar branch: `atividade/seu-nome`
3. Implementar todas as tarefas
4. Criar arquivo `RESPOSTAS.md` com respostas da Parte 1
5. **Pull Request** para `main` com descrição detalhada

### O que entregar:
```
padroes-de-projeto-DB-Terca/
├── RESPOSTAS.md                          ← Parte 1 (questões teóricas)
├── Application/
│   ├── DTOs/
│   │   ├── LivroUpdateDto.cs            ← Tarefa 2.1
│   │   └── LivroPatchDto.cs             ← Tarefa 2.1
│   ├── Exceptions/
│   │   ├── LivroNotFoundException.cs    ← Tarefa 2.2
│   │   ├── ISBNDuplicadoException.cs    ← Tarefa 2.2
│   │   └── LivroIndisponivelException.cs ← Tarefa 2.2
│   ├── Services/
│   │   └── LivroService.cs              ← Tarefa 2.3
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs ← Tarefa 2.5 (modificado)
├── Program.cs                            ← Tarefa 2.4 (endpoints)
└── Livros.http                           ← Parte 3 (testes)
```

### Prazo:
- **Data limite:** 11/11/2025 às 23:59
- **Entregas atrasadas:** -1,0 ponto por dia

---

## ✅ CHECKLIST ANTES DE ENTREGAR

- [ ] Código compila sem erros (`dotnet build`)
- [ ] Todos os 12 testes funcionam
- [ ] Respostas teóricas completas (RESPOSTAS.md)
- [ ] Exceções tratadas corretamente
- [ ] PUT substitui TODOS os campos
- [ ] PATCH atualiza APENAS campos enviados
- [ ] Validações funcionando
- [ ] Problem Details formatado corretamente
- [ ] Código comentado onde necessário
- [ ] PR criado com descrição clara

---

## 🎁 BÔNUS (até +2,0 pontos)

### Bônus 1: Testes Unitários (+1,0 ponto)
Criar testes unitários com xUnit + Moq para:
- `LivroService.AtualizarAsync`
- `LivroService.AtualizarParcialAsync`
- Mínimo 80% de cobertura

### Bônus 2: Concorrência (+0,5 pontos)
Implementar Optimistic Locking:
- Adicionar campo `RowVersion` em `Livro`
- Retornar 409 Conflict se versão mudou

### Bônus 3: Auditoria (+0,5 pontos)
Registrar em log:
- Quem atualizou (usuário - pode mockar)
- Quando atualizou (timestamp)
- O que mudou (campos alterados)

---

## ❓ DÚVIDAS FREQUENTES

### P: Posso usar FluentValidation?
**R:** Sim, mas não é obrigatório. DataAnnotations são suficientes.

### P: Como testar ISBN duplicado se não tenho banco real?
**R:** Simule no service (hardcode temporário) ou use InMemoryDatabase.

### P: Preciso implementar CRUD completo de Livro?
**R:** Não, apenas PUT e PATCH. Assuma que GET/POST/DELETE já existem.

### P: Posso trabalhar em dupla?
**R:** Não, atividade **individual**. Mas podem discutir conceitos.

### P: Onde busco ajuda?
**R:** 
1. Documentação do projeto (README.md)
2. Issues do GitHub
3. Office Hours (Terças 14h-16h)
4. NÃO use ChatGPT para código (plagiarismo)

---

## 📧 CONTATO

**Dúvidas sobre requisitos:** Abrir Issue no GitHub  
**Problemas técnicos:** Email professor@faculdade.edu.br  
**Office Hours:** Terças 14h-16h (Sala 305)

---

## 🏆 CRITÉRIO DE EXCELÊNCIA

Para nota máxima (10,0 + bônus), seu código deve:
- ✅ Compilar sem warnings
- ✅ Seguir convenções C# (PascalCase, etc.)
- ✅ Ter código limpo e legível
- ✅ Validações completas
- ✅ Tratamento de erros correto
- ✅ Testes abrangentes
- ✅ Documentação clara

**Boa sorte! 🚀**

---

**Preparado por:** Prof. Lucas Fogaça  
**Data:** 04/11/2025  
**Versão:** 1.0
