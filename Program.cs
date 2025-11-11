using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));

// Registra os repositórios
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// Registra os serviços de aplicação
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Registra os validadores do FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ProdutoCreateDtoValidator>();


var app = builder.Build();

//Get Listar todos os produtos.
app.MapGet("/produtos", async (IProdutoService service, CancellationToken ct) =>
{
    var produtos = await service.ListarAsync(ct);
    return Results.Ok(produtos);
})
.WithName("ListarProdutos")
.WithOpenApi();

// Get que busca por id.
app.MapGet("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.ObterAsync(id, ct);
    return produto != null ? Results.Ok(produto) : Results.NotFound();
})
.WithName("ObterProduto")
.WithOpenApi();
//post criar produto
app.MapPost("/produtos", async (
    ProdutoCreateDto dto, 
    IProdutoService service, 
    IValidator<ProdutoCreateDto> validator,
    CancellationToken ct) =>
{
    // Valida o DTO antes de processar
    var validationResult = await validator.ValidateAsync(dto, ct);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var produto = await service.CriarAsync(dto.Nome, dto.Descricao, dto.Preco, dto.Estoque, ct);
    return Results.Created($"/produtos/{produto.Id}", produto);
})
.WithName("CriarProduto")
.WithOpenApi();

//PUT Atualização completa do produto
app.MapPut("/produtos/{id}", async (
    int id, 
    Produto produto, 
    IProdutoService service,
    IValidator<Produto> validator,
    CancellationToken ct) =>
{
    // Valida o produto antes de atualizar
    var validationResult = await validator.ValidateAsync(produto, ct);
    
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var produtoAtualizado = await service.AtualizarAsync(id, produto, ct);
    return Results.Ok(produtoAtualizado);
})
.WithName("AtualizarProduto")
.WithOpenApi();

app.MapPatch("/produtos/{id}", async (
    int id, 
    Produto produto, 
    IProdutoService service, 
    CancellationToken ct) =>
{
    // Para PATCH, a validação é feita no serviço pois depende dos campos fornecidos
    var produtoAtualizadoParcialmente = await service.AtualizarParcialAsync(id, produto, ct);
    return Results.Ok(produtoAtualizadoParcialmente);
})
.WithName("AtualizarParcialProduto")
.WithOpenApi();

// Delete produto
app.MapDelete("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    if (!removido)
        return Results.NotFound();

    return Results.NoContent();
})
.WithName("RemoverProduto")
.WithOpenApi();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();


app.Run();

