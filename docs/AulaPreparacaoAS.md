# 🎯 AULA DE PREPARAÇÃO PARA AVALIAÇÃO SEMESTRAL (AS)

**Disciplina**: Desenvolvimento Backend  
**Data**: 18/11/2025  
**Objetivo**: Preparar os alunos para a AS através de revisão teórica e prática  
**Duração**: 3 horas

---

## 📋 AGENDA DA AULA

1. **Revisão dos Conceitos** (45 min)
2. **Análise do Projeto de Referência** (45 min)
3. **Demonstração Prática - Criando a API de Usuários** (60 min)
4. **Orientações sobre Documento e Vídeo** (30 min)

---

## 🎓 PARTE 1: REVISÃO DOS CONCEITOS (45 min)

### 1.1 Clean Architecture - Separação em Camadas

#### O que é?
Arquitetura em camadas que separa responsabilidades e cria código mais testável e manutenível.

#### Camadas do Projeto:

```
📁 Domain/          → Entidades de negócio (Usuario, Produto)
📁 Application/     → Lógica de aplicação (Services, DTOs, Validators)
📁 Infrastructure/  → Implementações técnicas (Repositórios, DbContext)
📄 Program.cs       → Configuração e Endpoints
```

#### Regra de Dependência:
```
Program.cs → Application → Domain
     ↓
Infrastructure → Domain
```

**Importante**: Domain não depende de nada! É o núcleo da aplicação.

---

### 1.2 Repository Pattern

#### Definição:
Padrão que abstrai o acesso a dados, isolando a lógica de persistência.

#### Por que usar?
- ✅ Facilita troca de banco de dados
- ✅ Melhora testabilidade
- ✅ Centraliza lógicas de consulta
- ✅ Separa responsabilidades

#### Estrutura:

```csharp
// Interface (Application/Interfaces)
public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(Usuario usuario, CancellationToken ct);
    Task UpdateAsync(Usuario usuario, CancellationToken ct);
    Task RemoveAsync(Usuario usuario, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}

// Implementação (Infrastructure/Repositories)
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Usuarios
            .Where(u => u.Ativo)
            .ToListAsync(ct);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id && u.Ativo, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        _context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        // Soft Delete
        usuario.Ativo = false;
        _context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
```

---

### 1.3 Service Pattern

#### Definição:
Camada que contém a lógica de negócio da aplicação.

#### Responsabilidades:
- Orquestrar operações entre repositórios
- Aplicar regras de negócio
- Transformar entidades em DTOs
- Coordenar validações

#### Estrutura:

```csharp
// Interface (Application/Interfaces)
public interface IUsuarioService
{
    Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct);
    Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct);
    Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct);
    Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct);
    Task<bool> RemoverAsync(int id, CancellationToken ct);
    Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct);
}

// Implementação (Application/Services)
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(u => u.ToReadDto());
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        return usuario?.ToReadDto();
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        // Regra de negócio: Email único
        if (await _repository.EmailExistsAsync(dto.Email, ct))
        {
            throw new InvalidOperationException("Email já cadastrado.");
        }

        // Regra de negócio: Hash da senha
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email.ToLower(), // Normalização
            Senha = senhaHash,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return usuario.ToReadDto();
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }

        // Verifica se email já existe (exceto o próprio usuário)
        var emailExistente = await _repository.GetByEmailAsync(dto.Email, ct);
        if (emailExistente != null && emailExistente.Id != id)
        {
            throw new InvalidOperationException("Email já cadastrado.");
        }

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email.ToLower();
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return usuario.ToReadDto();
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        
        if (usuario == null)
        {
            return false;
        }

        await _repository.RemoveAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        return await _repository.EmailExistsAsync(email, ct);
    }
}
```

---

### 1.4 DTO Pattern (Data Transfer Object)

#### Definição:
Objetos simples usados para transferir dados entre camadas, sem lógica de negócio.

#### Por que usar?
- ✅ Não expõe dados sensíveis (senha)
- ✅ Controla exatamente o que entra e sai da API
- ✅ Facilita versionamento da API
- ✅ Desacopla modelo de domínio da API

#### Exemplos:

```csharp
// Application/DTOs/UsuarioCreateDto.cs
public record UsuarioCreateDto(
    string Nome,
    string Email,
    string Senha,
    DateTime DataNascimento,
    string? Telefone
);

// Application/DTOs/UsuarioReadDto.cs
public record UsuarioReadDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao
);

// Application/DTOs/UsuarioUpdateDto.cs
public record UsuarioUpdateDto(
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo
);
```

#### Mapeamento (Extensions):

```csharp
// Application/Services/MappingExtensions.cs
public static class UsuarioMappingExtensions
{
    public static UsuarioReadDto ToReadDto(this Usuario usuario)
    {
        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }
}
```

---

### 1.5 FluentValidation

#### Definição:
Biblioteca para criar regras de validação de forma fluente e reutilizável.

#### Vantagens sobre DataAnnotations:
- ✅ Validações complexas e condicionais
- ✅ Validações assíncronas (consulta banco)
- ✅ Melhor testabilidade
- ✅ Separação de responsabilidades
- ✅ Mensagens de erro customizadas

#### Exemplo Completo:

```csharp
// Application/Validators/UsuarioCreateDtoValidator.cs
using FluentValidation;

public class UsuarioCreateDtoValidator : AbstractValidator<UsuarioCreateDto>
{
    private readonly IUsuarioRepository _repository;

    public UsuarioCreateDtoValidator(IUsuarioRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .MinimumLength(3)
            .WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório.")
            .EmailAddress()
            .WithMessage("O email não está em um formato válido.")
            .MustAsync(async (email, ct) => !await _repository.EmailExistsAsync(email, ct))
            .WithMessage("Este email já está cadastrado.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .WithMessage("A data de nascimento é obrigatória.")
            .Must(BeAtLeast18YearsOld)
            .WithMessage("É necessário ter pelo menos 18 anos.");

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
            .WithMessage("O telefone deve estar no formato (XX) XXXXX-XXXX.")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }

    private bool BeAtLeast18YearsOld(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        
        if (dataNascimento.Date > hoje.AddYears(-idade))
        {
            idade--;
        }
        
        return idade >= 18;
    }
}

// Application/Validators/UsuarioUpdateDtoValidator.cs
public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    private readonly IUsuarioRepository _repository;

    public UsuarioUpdateDtoValidator(IUsuarioRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .MinimumLength(3)
            .WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório.")
            .EmailAddress()
            .WithMessage("O email não está em um formato válido.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .WithMessage("A data de nascimento é obrigatória.")
            .Must(BeAtLeast18YearsOld)
            .WithMessage("É necessário ter pelo menos 18 anos.");

        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
            .WithMessage("O telefone deve estar no formato (XX) XXXXX-XXXX.")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }

    private bool BeAtLeast18YearsOld(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        
        if (dataNascimento.Date > hoje.AddYears(-idade))
        {
            idade--;
        }
        
        return idade >= 18;
    }
}
```

---

### 1.6 Dependency Injection

#### Definição:
Padrão onde as dependências são fornecidas externamente ao invés de criadas internamente.

#### Configuração no Program.cs:

```csharp
// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Configurar Repository Pattern
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Configurar Service Pattern
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
```

---

## 🔍 PARTE 2: ANÁLISE DO PROJETO DE REFERÊNCIA (45 min)

### 2.1 Estrutura do Projeto Produtos

Vamos analisar o projeto `padroes-de-projeto-DB-Terca` como referência:

```
APIProdutos/
├── Domain/
│   └── Entities/
│       └── Produto.cs              ← Entidade de domínio
│
├── Application/
│   ├── DTOs/
│   │   ├── ProdutoCreateDto.cs     ← DTO para criação
│   │   └── ProdutoReadDto.cs       ← DTO para leitura
│   │
│   ├── Interfaces/
│   │   ├── IProdutoRepository.cs   ← Contrato do Repository
│   │   └── IProdutoService.cs      ← Contrato do Service
│   │
│   ├── Services/
│   │   ├── MappingExtensions.cs    ← Mapeamento DTO ↔ Entity
│   │   └── ProdutoService.cs       ← Lógica de negócio
│   │
│   └── Validators/
│       ├── ProdutoCreateDtoValidator.cs  ← Validações
│       └── ProdutoUpdateValidator.cs
│
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs         ← Configuração EF Core
│   │
│   └── Repositories/
│       └── ProdutoRepository.cs    ← Implementação Repository
│
└── Program.cs                       ← Configuração e Endpoints
```

### 2.2 Pontos de Atenção

#### ✅ O que está CORRETO e deve ser seguido:
1. Separação em camadas
2. Uso de interfaces
3. Repository e Service implementados
4. DTOs separados por operação
5. FluentValidation configurado
6. Dependency Injection configurado

#### ⚠️ Diferenças para a AS:
1. **Entidade diferente**: Produto → Usuario
2. **Atributos diferentes**: Preço, Estoque → Email, Senha, DataNascimento
3. **Validações específicas**: Email único, idade >= 18 anos
4. **Soft Delete obrigatório**: Campo `Ativo`
5. **Hash de senha**: Usar BCrypt

---

## 💻 PARTE 3: DEMONSTRAÇÃO PRÁTICA (60 min)

### 3.1 Criando a Entidade Usuario

```csharp
// Domain/Entities/Usuario.cs
using System.ComponentModel.DataAnnotations;

namespace APIUsuarios.Domain.Entities;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public DateTime DataNascimento { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    public bool Ativo { get; set; } = true;

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; set; }
}
```

### 3.2 Configurando o DbContext

```csharp
// Infrastructure/Persistence/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            // Configurar índice único para email
            entity.HasIndex(e => e.Email).IsUnique();

            // Configurar valores padrão
            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("datetime('now')");
        });
    }
}
```

### 3.3 Configurando appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=usuarios.db"
  }
}
```

### 3.4 Criando as Migrations

```bash
# Instalar ferramenta (se necessário)
dotnet tool install --global dotnet-ef

# Criar migration
dotnet ef migrations add CriacaoInicial

# Aplicar ao banco
dotnet ef database update
```

### 3.5 Configurando Program.cs Completo

```csharp
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Configurar Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Configurar Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

// ==================== ENDPOINTS ====================

// GET /usuarios - Listar todos
app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    var usuarios = await service.ListarAsync(ct);
    return Results.Ok(usuarios);
})
.WithName("ListarUsuarios")
.WithTags("Usuarios")
.Produces<IEnumerable<UsuarioReadDto>>(200);

// GET /usuarios/{id} - Buscar por ID
app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    
    if (usuario == null)
    {
        return Results.NotFound(new { message = "Usuário não encontrado." });
    }
    
    return Results.Ok(usuario);
})
.WithName("ObterUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(200)
.Produces(404);

// POST /usuarios - Criar
app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IUsuarioService service,
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    // Validar
    var validationResult = await validator.ValidateAsync(dto, ct);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    try
    {
        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
})
.WithName("CriarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(201)
.Produces(400)
.Produces(409);

// PUT /usuarios/{id} - Atualizar
app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    CancellationToken ct) =>
{
    // Validar
    var validationResult = await validator.ValidateAsync(dto, ct);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    try
    {
        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { message = "Usuário não encontrado." });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
})
.WithName("AtualizarUsuario")
.WithTags("Usuarios")
.Produces<UsuarioReadDto>(200)
.Produces(400)
.Produces(404)
.Produces(409);

// DELETE /usuarios/{id} - Remover (Soft Delete)
app.MapDelete("/usuarios/{id:int}", async (
    int id,
    IUsuarioService service,
    CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    
    if (!removido)
    {
        return Results.NotFound(new { message = "Usuário não encontrado." });
    }
    
    return Results.NoContent();
})
.WithName("RemoverUsuario")
.WithTags("Usuarios")
.Produces(204)
.Produces(404);

app.Run();
```

### 3.6 Instalando Dependências Necessárias

```bash
# Entity Framework Core com SQLite
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# FluentValidation
dotnet add package FluentValidation.AspNetCore

# BCrypt para hash de senha
dotnet add package BCrypt.Net-Next

# Swagger
dotnet add package Swashbuckle.AspNetCore
```

---

## 📄 PARTE 4: ORIENTAÇÕES DOCUMENTO E VÍDEO (30 min)

### 4.1 Estrutura do Documento Acadêmico

#### Template ABNT no Word

1. **Baixar template**: Procure por "template ABNT Word" no Google
2. **Configurar margens**:
   - Superior: 3cm
   - Esquerda: 3cm
   - Inferior: 2cm
   - Direita: 2cm
3. **Fonte**: Times New Roman 12
4. **Espaçamento**: 1,5 linhas

#### Seções Obrigatórias

**CAPA**
```
[LOGO DA INSTITUIÇÃO]

NOME DA INSTITUIÇÃO
CURSO DE [SEU CURSO]

SEU NOME COMPLETO

API DE GERENCIAMENTO DE USUÁRIOS:
APLICAÇÃO DE PADRÕES DE PROJETO EM ASP.NET CORE

Cidade - Estado
2025
```

**RESUMO**
```
Este trabalho apresenta o desenvolvimento de uma API REST para
gerenciamento de usuários utilizando ASP.NET Core e padrões de
projeto. O objetivo é demonstrar a aplicação prática de Repository
Pattern, Service Pattern, DTO Pattern e FluentValidation em uma
arquitetura limpa e escalável. A metodologia envolveu...
[150-250 palavras]

Palavras-chave: ASP.NET Core. API REST. Padrões de Projeto.
Clean Architecture. FluentValidation.
```

**FUNDAMENTAÇÃO TEÓRICA - Estrutura de Tópico**
```
5.3.1 Repository Pattern

O Repository Pattern é um padrão de projeto que...

[Definição]
[Objetivo]
[Vantagens]
[Desvantagens]

Exemplo de implementação no projeto:

[CÓDIGO AQUI - usar fonte Courier New 10]

A implementação acima demonstra...
```

#### Exemplo de Referência ABNT

```
MICROSOFT. ASP.NET Core documentation. Disponível em: 
<https://docs.microsoft.com/aspnet/core>. Acesso em: 18 nov. 2025.

MARTIN, Robert C. Clean Architecture: A Craftsman's Guide to 
Software Structure and Design. Boston: Prentice Hall, 2017.

FOWLER, Martin. Patterns of Enterprise Application Architecture. 
Boston: Addison-Wesley, 2002.
```

### 4.2 Criando o Vídeo Demonstrativo

#### Roteiro Sugerido

**INTRODUÇÃO (1 min)**
```
"Olá, meu nome é [SEU NOME], RA [SEU RA], e este é o vídeo 
demonstrativo da minha API de Gerenciamento de Usuários 
desenvolvida para a Avaliação Semestral de Backend.

Neste vídeo vou mostrar a estrutura do projeto, explicar os 
principais padrões implementados e demonstrar os endpoints 
funcionando."
```

**ESTRUTURA (1-2 min)**
```
[Mostrando VS Code com estrutura de pastas]

"Aqui temos a estrutura do projeto seguindo Clean Architecture:

- Domain: contém a entidade Usuario
- Application: contém Services, DTOs e Validators
- Infrastructure: contém Repositories e DbContext
- Program.cs: configuração e endpoints

Vamos ver cada camada rapidamente..."
```

**CÓDIGO (2-3 min)**
```
[Mostrando Usuario.cs]
"Esta é a entidade Usuario com os campos obrigatórios..."

[Mostrando IUsuarioRepository e implementação]
"Aqui temos o Repository Pattern que abstrai o acesso a dados..."

[Mostrando UsuarioService]
"O Service contém a lógica de negócio, como validação de email 
único e hash de senha..."

[Mostrando UsuarioCreateDtoValidator]
"FluentValidation permite criar regras complexas como validação 
assíncrona de email e idade mínima..."
```

**DEMONSTRAÇÃO (2-3 min)**
```
[Abrindo Postman ou Swagger]

"Agora vamos testar os endpoints:

1. POST /usuarios - Criar usuário válido → 201 Created ✅
2. POST /usuarios - Email duplicado → 409 Conflict ❌
3. POST /usuarios - Dados inválidos → 400 Bad Request ❌
4. GET /usuarios - Listar todos → 200 OK ✅
5. GET /usuarios/1 - Buscar por ID → 200 OK ✅
6. PUT /usuarios/1 - Atualizar → 200 OK ✅
7. DELETE /usuarios/1 - Soft Delete → 204 No Content ✅

[Abrindo DB Browser]
Aqui no banco, vemos que o usuário deletado tem Ativo = false"
```

**CONCLUSÃO (30s)**
```
"Implementei com sucesso todos os requisitos da AS: Repository,
Service, DTO, FluentValidation e todos os endpoints funcionando.
Os principais aprendizados foram...

Obrigado!"
```

#### Ferramentas para Gravação

**OBS Studio (Gratuito e Completo)**
1. Download: https://obsproject.com
2. Configuração básica:
   - Source: Display Capture (tela inteira)
   - Mic/Aux: Seu microfone
   - Output: MP4, 1920x1080, 30fps
3. Gravar → Editar cortes básicos → Exportar

**Loom (Mais Simples)**
1. Extensão Chrome: chrome.google.com/webstore
2. Clique no ícone → Start Recording
3. Grave → Para → Gera link automaticamente

#### Checklist do Vídeo

- [ ] Duração: 5-10 minutos
- [ ] Áudio claro e sem ruídos
- [ ] Mostra estrutura de pastas
- [ ] Explica pelo menos 2 padrões com código
- [ ] Demonstra todos os endpoints (GET, POST, PUT, DELETE)
- [ ] Mostra validações funcionando (erro 400)
- [ ] Mostra email duplicado (erro 409)
- [ ] Mostra soft delete no banco
- [ ] Qualidade mínimo 720p
- [ ] Link público e funcionando

---

## ✅ CHECKLIST COMPLETO DA AS

### Antes de Começar
- [ ] Li todo o enunciado
- [ ] Entendi os requisitos
- [ ] Analisei o projeto de referência
- [ ] Criei repositório no GitHub
- [ ] Configurei ambiente de desenvolvimento

### Durante o Desenvolvimento
- [ ] Criei estrutura de pastas
- [ ] Implementei entidade Usuario
- [ ] Implementei Repository Pattern
- [ ] Implementei Service Pattern
- [ ] Criei DTOs (Create, Read, Update)
- [ ] Implementei FluentValidation
- [ ] Configurei Entity Framework + SQLite
- [ ] Criei e apliquei migrations
- [ ] Implementei todos os endpoints
- [ ] Testei cada endpoint
- [ ] Fiz commits frequentes (mínimo 10)

### Documento
- [ ] Usei template ABNT
- [ ] Criei capa
- [ ] Escrevi resumo (150-250 palavras)
- [ ] Fiz introdução
- [ ] Escrevi fundamentação teórica completa
- [ ] Expliquei cada padrão (Repository, Service, DTO)
- [ ] Incluí trechos de código
- [ ] Escrevi sobre desenvolvimento
- [ ] Apresentei resultados
- [ ] Escrevi conclusão
- [ ] Adicionei mínimo 8 referências ABNT
- [ ] Revisei ortografia
- [ ] Exportei para PDF

### Vídeo
- [ ] Preparei roteiro
- [ ] Testei gravação
- [ ] Gravei apresentação pessoal
- [ ] Mostrei estrutura do projeto
- [ ] Expliquei código (padrões)
- [ ] Demonstrei todos os endpoints
- [ ] Mostrei validações funcionando
- [ ] Mostrei soft delete no banco
- [ ] Duração: 5-10 min
- [ ] Upload concluído
- [ ] Link público funcionando

### Entrega
- [ ] README.md completo no GitHub
- [ ] Collection Postman no repositório
- [ ] Repositório público
- [ ] Código compila sem erros
- [ ] Todos os testes funcionam
- [ ] Criei arquivo ZIP
- [ ] Incluí Link_Repositorio.txt
- [ ] Incluí Documento_Academico.pdf
- [ ] Incluí Link_Video.txt
- [ ] Nome do ZIP correto: AS_Backend_[NOME]_[RA].zip
- [ ] Enviei no Blackboard/Moodle

---

## 🎯 DICAS FINAIS

### Do's ✅
- ✅ Comece HOJE! Não deixe para última hora
- ✅ Faça um pouco por dia (1-2 horas)
- ✅ Teste cada funcionalidade após implementar
- ✅ Faça commits descritivos frequentemente
- ✅ Consulte a documentação oficial
- ✅ Peça ajuda quando travar (mas não código pronto)
- ✅ Revise tudo antes de entregar

### Don'ts ❌
- ❌ Não copie código de colegas
- ❌ Não deixe para última semana
- ❌ Não pule as validações
- ❌ Não esqueça o soft delete
- ❌ Não deixe repositório privado
- ❌ Não faça vídeo sem roteiro
- ❌ Não entregue sem testar

### Cronograma Sugerido

**Semana 1 (18-24/11)**
- Estudar conceitos
- Analisar projeto de referência
- Criar estrutura básica
- Implementar entidade e DbContext

**Semana 2 (25/11-01/12)**
- Implementar Repository
- Implementar Service
- Criar DTOs
- Implementar FluentValidation

**Semana 3 (02-08/12)**
- Implementar endpoints
- Testar tudo
- Criar Collection Postman
- Escrever README

**Semana 4 (09-15/12)**
- Escrever documento acadêmico
- Gravar vídeo
- Revisar tudo
- Entregar (16/12 às 23:59)

---

## 📚 RECURSOS ADICIONAIS

### Documentação Oficial
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Entity Framework: https://docs.microsoft.com/ef/core
- FluentValidation: https://docs.fluentvalidation.net

### Tutoriais em Vídeo (YouTube)
- "Clean Architecture" - CodeOpinion
- "Repository Pattern" - Nick Chapsas
- "FluentValidation" - IAmTimCorey

### Geradores Online
- Template ABNT: https://www.fastformat.co
- Referências ABNT: http://www.more.ufsc.br

---

## 💬 PERGUNTAS E RESPOSTAS

**P: Posso usar AutoMapper?**
R: Sim, mas não é obrigatório. Mapping manual é suficiente.

**P: Preciso implementar JWT?**
R: Não. Foque nos padrões solicitados.

**P: Posso adicionar mais campos na entidade?**
R: Sim, desde que mantenha os obrigatórios.

**P: O que é "soft delete"?**
R: Ao invés de deletar do banco, apenas marca Ativo = false.

**P: Como faço hash da senha?**
R: Use BCrypt.Net: `BCrypt.Net.BCrypt.HashPassword(senha)`

**P: Preciso aparecer no vídeo?**
R: Não. Apenas sua voz narrando enquanto mostra o código.

---

## 🎓 RESUMO DA AULA

Hoje revisamos:

1. ✅ **Clean Architecture**: Separação em camadas (Domain, Application, Infrastructure)
2. ✅ **Repository Pattern**: Abstração do acesso a dados
3. ✅ **Service Pattern**: Lógica de negócio
4. ✅ **DTO Pattern**: Transferência de dados
5. ✅ **FluentValidation**: Validações robustas
6. ✅ **Dependency Injection**: Inversão de controle

Demonstramos:
- Criação da entidade Usuario
- Implementação de Repository e Service
- Configuração de FluentValidation
- Criação de endpoints
- Orientações sobre documento e vídeo

---

## 📝 EXERCÍCIO PARA CASA

**Tarefa**: Comece a implementar sua API de Usuários

1. Crie o repositório no GitHub
2. Configure a estrutura de pastas
3. Implemente a entidade Usuario
4. Configure o DbContext
5. Crie e aplique a primeira migration
6. Faça pelo menos 5 commits descritivos

**Próxima aula**: Tira-dúvidas e acompanhamento

---

**BOA SORTE NA AS! VOCÊ CONSEGUE! 🚀**

---

*Aula preparada por: Professor [Nome]*  
*Data: 18/11/2025*  
*Material complementar disponível no repositório do curso*
