# 🚀 Guia Rápido - FluentValidation

## ⚡ Setup Rápido (Já Feito!)

```bash
✅ dotnet add package FluentValidation.AspNetCore
✅ Validadores criados
✅ DI configurado
✅ Endpoints integrados
```

## 📝 Como Criar um Novo Validador

### 1. Criar a classe do validador
```csharp
using FluentValidation;

public class MeuDtoValidator : AbstractValidator<MeuDto>
{
    public MeuDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório");
            
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email inválido");
    }
}
```

### 2. Registrar no DI (automático!)
O registro já está feito para toda a assembly:
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<ProdutoCreateDtoValidator>();
```

### 3. Usar no endpoint
```csharp
app.MapPost("/rota", async (
    MeuDto dto,
    IValidator<MeuDto> validator) =>
{
    var result = await validator.ValidateAsync(dto);
    
    if (!result.IsValid)
        return Results.ValidationProblem(result.ToDictionary());
    
    // Processar...
});
```

## 📚 Validadores Mais Comuns

```csharp
// Obrigatório
RuleFor(x => x.Nome).NotEmpty();
RuleFor(x => x.Nome).NotNull();

// Texto
RuleFor(x => x.Nome).MaximumLength(200);
RuleFor(x => x.Nome).MinimumLength(3);
RuleFor(x => x.Email).EmailAddress();

// Números
RuleFor(x => x.Idade).GreaterThan(0);
RuleFor(x => x.Idade).LessThanOrEqualTo(120);
RuleFor(x => x.Idade).InclusiveBetween(18, 65);

// Precisão decimal
RuleFor(x => x.Preco).PrecisionScale(10, 2, ignoreTrailingZeros: true);

// Regex
RuleFor(x => x.Telefone).Matches(@"^\d{10,11}$");

// Validação condicional
RuleFor(x => x.Descricao)
    .NotEmpty()
    .When(x => x.Tipo == "Premium");

// Validações customizadas
RuleFor(x => x.Idade)
    .Must(idade => idade >= 18)
    .WithMessage("Deve ser maior de idade");
```

## 🎯 Padrões de Uso

### Validação Simples
```csharp
RuleFor(p => p.Nome)
    .NotEmpty()
    .WithMessage("Nome obrigatório");
```

### Validação em Cadeia
```csharp
RuleFor(p => p.Nome)
    .NotEmpty().WithMessage("Nome obrigatório")
    .MaximumLength(100).WithMessage("Máximo 100 caracteres")
    .Must(nome => !nome.Contains("@")).WithMessage("Nome não pode conter @");
```

### Validação Condicional
```csharp
// Valida apenas quando condição é verdadeira
RuleFor(p => p.CNPJ)
    .NotEmpty()
    .Length(14)
    .When(p => p.TipoPessoa == "Juridica");
```

### Validação Customizada
```csharp
RuleFor(p => p.DataNascimento)
    .Must(BeAValidAge)
    .WithMessage("Idade deve estar entre 18 e 100 anos");

private bool BeAValidAge(DateTime dataNascimento)
{
    var idade = DateTime.Now.Year - dataNascimento.Year;
    return idade >= 18 && idade <= 100;
}
```

### Validação com Acesso a Outro Campo
```csharp
RuleFor(p => p.DataFim)
    .GreaterThan(p => p.DataInicio)
    .WithMessage("Data fim deve ser posterior à data início");
```

## 🧪 Testar Validadores

```csharp
using FluentValidation.TestHelper;

[Fact]
public void Deve_Validar_Nome_Obrigatorio()
{
    var validator = new ProdutoCreateDtoValidator();
    var dto = new ProdutoCreateDto("", "Desc", 10, 5);
    
    var result = validator.TestValidate(dto);
    
    result.ShouldHaveValidationErrorFor(p => p.Nome);
}

[Fact]
public void Deve_Aceitar_Dados_Validos()
{
    var validator = new ProdutoCreateDtoValidator();
    var dto = new ProdutoCreateDto("Produto", "Desc", 10, 5);
    
    var result = validator.TestValidate(dto);
    
    result.ShouldNotHaveAnyValidationErrors();
}
```

## ⚠️ Erros Comuns

### ❌ Não registrar o validador
```csharp
// ERRADO - esquecer de registrar
app.MapPost("/produtos", async (ProdutoCreateDto dto) => ...);
```

```csharp
// CERTO - injetar o validador
app.MapPost("/produtos", async (
    ProdutoCreateDto dto,
    IValidator<ProdutoCreateDto> validator) => ...);
```

### ❌ Não verificar o resultado
```csharp
// ERRADO
await validator.ValidateAsync(dto); // resultado ignorado!
```

```csharp
// CERTO
var result = await validator.ValidateAsync(dto);
if (!result.IsValid)
    return Results.ValidationProblem(result.ToDictionary());
```

### ❌ Validar entidade ao invés de DTO
```csharp
// EVITAR - validar entidade de domínio
IValidator<Produto> validator

// PREFERIR - validar DTO de entrada
IValidator<ProdutoCreateDto> validator
```

## 💡 Dicas Pro

### 1. Reutilizar Validadores
```csharp
public class EnderecoValidator : AbstractValidator<Endereco>
{
    // Validações de endereço
}

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(c => c.Endereco)
            .SetValidator(new EnderecoValidator());
    }
}
```

### 2. Validações Assíncronas
```csharp
RuleFor(x => x.Email)
    .MustAsync(async (email, ct) => 
    {
        return !await _repository.EmailExistsAsync(email, ct);
    })
    .WithMessage("Email já cadastrado");
```

### 3. Mensagens Dinâmicas
```csharp
RuleFor(x => x.Nome)
    .MaximumLength(100)
    .WithMessage(x => $"Nome '{x.Nome}' excede 100 caracteres");
```

### 4. Regras Complexas
```csharp
RuleFor(x => x)
    .Must(produto => produto.Preco > 0 || produto.Gratis)
    .WithMessage("Produto deve ter preço ou ser marcado como grátis");
```

## 🎯 Checklist de Implementação

- [ ] Criar classe herdando `AbstractValidator<T>`
- [ ] Definir regras no construtor com `RuleFor()`
- [ ] Adicionar mensagens de erro em português
- [ ] Registrar no DI (já configurado automaticamente)
- [ ] Injetar `IValidator<T>` no endpoint
- [ ] Chamar `ValidateAsync()` antes de processar
- [ ] Retornar `ValidationProblem()` se inválido
- [ ] Criar testes unitários

## 📖 Referências Rápidas

| Validador | Uso |
|-----------|-----|
| `NotEmpty()` | Campo não vazio |
| `NotNull()` | Campo não nulo |
| `MaximumLength(n)` | Máximo n caracteres |
| `MinimumLength(n)` | Mínimo n caracteres |
| `EmailAddress()` | Email válido |
| `GreaterThan(n)` | Maior que n |
| `LessThan(n)` | Menor que n |
| `InclusiveBetween(a,b)` | Entre a e b (inclusivo) |
| `Must(lambda)` | Condição customizada |
| `When(condition)` | Validação condicional |
| `WithMessage(msg)` | Mensagem de erro |

## 🔗 Links Úteis

- [Documentação Oficial](https://docs.fluentvalidation.net/)
- [Built-in Validators](https://docs.fluentvalidation.net/en/latest/built-in-validators.html)
- [Exemplos no Projeto](./ExemplosTestes.md)
- [README Completo](./README.md)

---

**Dúvidas?** Consulte `Application/Validators/README.md` para explicação detalhada!
