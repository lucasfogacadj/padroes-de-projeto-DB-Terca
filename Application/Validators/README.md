# Validators (FluentValidation)

## 📋 Visão Geral
Esta pasta contém os validadores de entrada da aplicação usando **FluentValidation**.

## 🎯 Objetivo
Separar as **validações de entrada** (input validation) da lógica de negócio, mantendo o código:
- Legível e declarativo
- Reutilizável
- Testável isoladamente
- Centralizado em um único local

## 🔍 Validadores Implementados

### `ProdutoCreateDtoValidator`
Valida a criação de produtos através do DTO `ProdutoCreateDto`.

**Regras**:
- ✅ Nome: obrigatório, máx 200 caracteres, não pode ser apenas espaços
- ✅ Descrição: opcional, máx 1000 caracteres
- ✅ Preço: obrigatório, maior que zero, máx 2 casas decimais
- ✅ Estoque: obrigatório, não pode ser negativo

### `ProdutoUpdateValidator`
Valida a atualização completa (PUT) de produtos.

**Regras**:
- ✅ Mesmas regras do Create
- ✅ Todos os campos são obrigatórios (atualização completa)

### `ProdutoPatchValidator`
Valida a atualização parcial (PATCH) de produtos.

**Regras**:
- ✅ Valida apenas campos fornecidos
- ✅ Não exige que todos os campos estejam presentes
- ✅ Regras condicionais com `.When()`

## 🔧 Como Funciona

### 1. Registro no Container DI
Os validadores são registrados automaticamente no `Program.cs`:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<ProdutoCreateDtoValidator>();
```

### 2. Uso nos Endpoints
Os validadores são invocados manualmente ou via filtros:

```csharp
app.MapPost("/produtos", async (ProdutoCreateDto dto, IValidator<ProdutoCreateDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    // Continuar com a lógica...
});
```

## 🆚 Validação vs Invariantes de Domínio

| Aspecto | Validação (FluentValidation) | Invariantes (Domain) |
|---------|------------------------------|----------------------|
| **Onde** | Controllers/Endpoints | Entidades/Value Objects |
| **Quando** | Entrada de dados externos | Sempre (criação/modificação) |
| **Propósito** | Verificar formato/sintaxe | Garantir consistência do negócio |
| **Exemplo** | "Nome não pode ser vazio" | "Produto ativo deve ter estoque > 0" |
| **Exceções** | `ValidationException` | `DomainException` |

## 💡 Boas Práticas

### ✅ Fazer
- Usar mensagens de erro claras e em português
- Validar apenas o formato/tipo dos dados
- Criar validadores específicos por DTO
- Testar validadores isoladamente
- Usar `.When()` para validações condicionais

### ❌ Evitar
- Colocar lógica de negócio nos validadores
- Acessar banco de dados diretamente
- Duplicar validações entre validador e serviço
- Usar validações genéricas demais

## 🧪 Testando Validadores

Exemplo de teste unitário:

```csharp
public class ProdutoCreateDtoValidatorTests
{
    private readonly ProdutoCreateDtoValidator _validator = new();

    [Fact]
    public void Deve_Falhar_Quando_Nome_Vazio()
    {
        // Arrange
        var dto = new ProdutoCreateDto("", "Descrição", 10.00m, 5);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }
}
```

## 🔄 Validação Assíncrona

Para validações que precisam acessar dados externos (ex: verificar se nome já existe):

```csharp
RuleFor(p => p.Nome)
    .MustAsync(async (nome, cancellation) => 
    {
        return !await _repository.ExisteComNomeAsync(nome, cancellation);
    })
    .WithMessage("Já existe um produto com este nome.");
```

**⚠️ Cuidado**: Isso cria acoplamento. Considere mover para o Service.

## 📚 Recursos

- [Documentação FluentValidation](https://docs.fluentvalidation.net/)
- [Built-in Validators](https://docs.fluentvalidation.net/en/latest/built-in-validators.html)
- [Custom Validators](https://docs.fluentvalidation.net/en/latest/custom-validators.html)

## 🎓 Quando NÃO Usar FluentValidation?

1. **Validações simples**: Se a validação cabe em uma linha, DataAnnotations pode ser suficiente
2. **Regras de domínio**: Use Value Objects ou métodos na entidade
3. **Validações dependentes de estado**: Melhor no Service
4. **Projetos muito pequenos**: Pode ser over-engineering

## 🚀 Próximos Passos

- [ ] Adicionar validação de ID para operações de atualização
- [ ] Criar validador para filtros de busca (query params)
- [ ] Implementar validações customizadas reutilizáveis
- [ ] Adicionar testes para todos os validadores
- [ ] Integrar com Problem Details para respostas padronizadas

## 📝 Notas para Alunos

**Reflexão**: Por que separamos validação de input da validação de domínio?

**Resposta**: Validação de input garante que os dados chegam no formato correto. Validação de domínio garante que as regras de negócio são respeitadas. São responsabilidades diferentes!

**Exemplo**:
- Input: "O preço deve ser um número decimal" ← FluentValidation
- Domínio: "Produtos em promoção devem ter desconto mínimo de 10%" ← Domain Logic
