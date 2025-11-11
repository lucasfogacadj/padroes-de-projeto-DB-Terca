# Exemplos de Testes para Validadores

## 🧪 Como Criar Projeto de Testes

```bash
# Na raiz da solução
dotnet new xunit -n APIProdutos.Tests
dotnet add APIProdutos.Tests reference APIProdutos.csproj
dotnet add APIProdutos.Tests package FluentAssertions
dotnet add APIProdutos.Tests package Moq
dotnet sln add APIProdutos.Tests
```

## 📝 Exemplo de Teste - ProdutoCreateDtoValidator

Crie o arquivo `APIProdutos.Tests/Validators/ProdutoCreateDtoValidatorTests.cs`:

```csharp
using Application.DTOs;
using Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace APIProdutos.Tests.Validators;

public class ProdutoCreateDtoValidatorTests
{
    private readonly ProdutoCreateDtoValidator _validator;

    public ProdutoCreateDtoValidatorTests()
    {
        _validator = new ProdutoCreateDtoValidator();
    }

    [Fact]
    public void Deve_Ser_Valido_Quando_Todos_Campos_Corretos()
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Notebook Dell",
            Descricao: "Notebook com 16GB RAM",
            Preco: 3500.99m,
            Estoque: 10
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Falhar_Quando_Nome_Invalido(string nomeInvalido)
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: nomeInvalido,
            Descricao: "Descrição válida",
            Preco: 100.00m,
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Nome);
    }

    [Fact]
    public void Deve_Falhar_Quando_Nome_Muito_Longo()
    {
        // Arrange
        var nomeLongo = new string('A', 201); // 201 caracteres
        var dto = new ProdutoCreateDto(
            Nome: nomeLongo,
            Descricao: "Descrição válida",
            Preco: 100.00m,
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Nome)
            .WithErrorMessage("O nome do produto não pode ter mais de 200 caracteres.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Deve_Falhar_Quando_Preco_Menor_Ou_Igual_Zero(decimal precoInvalido)
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: "Descrição válida",
            Preco: precoInvalido,
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Preco)
            .WithErrorMessage("O preço deve ser maior que zero.");
    }

    [Fact]
    public void Deve_Falhar_Quando_Preco_Tem_Mais_De_Duas_Casas_Decimais()
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: "Descrição válida",
            Preco: 100.999m, // 3 casas decimais
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Preco);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Deve_Falhar_Quando_Estoque_Negativo(int estoqueInvalido)
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: "Descrição válida",
            Preco: 100.00m,
            Estoque: estoqueInvalido
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Estoque)
            .WithErrorMessage("O estoque não pode ser negativo.");
    }

    [Fact]
    public void Deve_Aceitar_Estoque_Zero()
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: "Descrição válida",
            Preco: 100.00m,
            Estoque: 0
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(p => p.Estoque);
    }

    [Fact]
    public void Deve_Aceitar_Descricao_Vazia()
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: "",
            Preco: 100.00m,
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(p => p.Descricao);
    }

    [Fact]
    public void Deve_Falhar_Quando_Descricao_Muito_Longa()
    {
        // Arrange
        var descricaoLonga = new string('B', 1001); // 1001 caracteres
        var dto = new ProdutoCreateDto(
            Nome: "Produto Teste",
            Descricao: descricaoLonga,
            Preco: 100.00m,
            Estoque: 5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Descricao)
            .WithErrorMessage("A descrição não pode ter mais de 1000 caracteres.");
    }

    [Fact]
    public void Deve_Ter_Multiplos_Erros_Quando_Varios_Campos_Invalidos()
    {
        // Arrange
        var dto = new ProdutoCreateDto(
            Nome: "",
            Descricao: "Ok",
            Preco: -10.00m,
            Estoque: -5
        );

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Nome);
        result.ShouldHaveValidationErrorFor(p => p.Preco);
        result.ShouldHaveValidationErrorFor(p => p.Estoque);
        result.Errors.Should().HaveCountGreaterOrEqualTo(3);
    }
}
```

## 🏃 Como Executar os Testes

```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --logger "console;verbosity=detailed"

# Executar apenas testes de validadores
dotnet test --filter "FullyQualifiedName~Validators"

# Ver cobertura de código (requer pacote coverlet.collector)
dotnet test /p:CollectCoverage=true
```

## 📊 Resultado Esperado

```
Aprovado!  – Com falha:     0, Aprovado:    11, Ignorado:     0, Total:    11, Duração: < 1 s
```

## 💡 Dicas de Testes

### ✅ Boas Práticas
1. **Arrange-Act-Assert**: Estruture claramente os testes
2. **Um assert por teste**: Teste uma coisa por vez (quando possível)
3. **Nomes descritivos**: `Deve_Falhar_Quando_Nome_Vazio`
4. **Use Theory**: Para testar múltiplos valores similares
5. **Teste casos extremos**: Valores limites, nulos, vazios

### 🎯 O que Testar
- ✅ Casos válidos (happy path)
- ✅ Cada regra de validação individualmente
- ✅ Valores limites (0, máximo, mínimo)
- ✅ Valores nulos/vazios
- ✅ Múltiplos erros simultâneos
- ✅ Mensagens de erro corretas

### ❌ O que NÃO Testar nos Validadores
- ❌ Lógica de banco de dados
- ❌ Comportamento do FluentValidation (já testado pela biblioteca)
- ❌ Integração com outros componentes

## 🔧 Pacotes Úteis

```xml
<ItemGroup>
  <PackageReference Include="xunit" Version="2.6.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
  <PackageReference Include="FluentAssertions" Version="6.12.0" />
  <PackageReference Include="FluentValidation.TestHelper" Version="11.11.0" />
  <PackageReference Include="Moq" Version="4.20.0" />
  <PackageReference Include="coverlet.collector" Version="6.0.0" />
</ItemGroup>
```

## 📚 Recursos

- [xUnit Docs](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [FluentValidation Testing](https://docs.fluentvalidation.net/en/latest/testing.html)
- [Moq Quickstart](https://github.com/moq/moq4)

## 🎓 Exercícios para Alunos

1. Criar testes para `ProdutoUpdateValidator`
2. Criar testes para `ProdutoPatchValidator`
3. Adicionar teste que verifica se a mensagem de erro está em português
4. Criar teste parametrizado com múltiplos cenários válidos
5. Atingir 100% de cobertura nos validadores

## 🚀 Próximo Nível

Após dominar testes de validadores:
1. Testar Services (com mocks de Repository)
2. Testes de integração (com banco in-memory)
3. Testes de performance
4. Mutation testing (Stryker.NET)
