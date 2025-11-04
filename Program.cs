using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Adicionar suporte a Problem Details
builder.Services.AddProblemDetails();

var app = builder.Build();

// ⚠️ IMPORTANTE: Global Exception Handler deve ser o PRIMEIRO middleware
app.UseGlobalExceptionHandler();

//Get Listar todos os produtos.
app.MapGet("/produtos", async (IProdutoService service, CancellationToken ct) =>
{
    var produtos = await service.ListarAsync(ct);
    return Results.Ok(produtos);
})
.WithName("ListarProdutos")
.WithOpenApi()
.WithSummary("Lista todos os produtos")
.Produces<IEnumerable<Produto>>(200);

// Get que busca por id.
app.MapGet("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.ObterAsync(id, ct);
    return Results.Ok(produto);
})
.WithName("ObterProduto")
.WithOpenApi()
.WithSummary("Obtém um produto por ID")
.Produces<ProdutoReadDto>(200)
.Produces<ProblemDetails>(404);

//post criar produto
app.MapPost("/produtos", async (ProdutoCreateDto dto, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.CriarAsync(dto.Nome, dto.Descricao, dto.Preco, dto.Estoque, ct);
    return Results.Created($"/produtos/{produto.Id}", produto);
})
.WithName("CriarProduto")
.WithOpenApi()
.WithSummary("Cria um novo produto")
.Produces<Produto>(201)
.Produces<ProblemDetails>(400);

// PUT - Atualização completa (substituição total)
app.MapPut("/produtos/{id}", async (int id, ProdutoUpdateDto dto, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.AtualizarAsync(id, dto, ct);
    return Results.Ok(produto);
})
.WithName("AtualizarProdutoCompleto")
.WithOpenApi()
.WithSummary("Atualiza um produto completamente (PUT) - todos os campos são obrigatórios")
.WithDescription("Substitui TODOS os dados do produto. Todos os campos devem ser fornecidos.")
.Produces<Produto>(200)
.Produces<ProblemDetails>(404)
.Produces<ProblemDetails>(400);

// PATCH - Atualização parcial
app.MapPatch("/produtos/{id}", async (int id, ProdutoPatchDto dto, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.AtualizarParcialAsync(id, dto, ct);
    return Results.Ok(produto);
})
.WithName("AtualizarProdutoParcial")
.WithOpenApi()
.WithSummary("Atualiza um produto parcialmente (PATCH) - campos opcionais")
.WithDescription("Atualiza apenas os campos fornecidos. Campos não enviados permanecem inalterados.")
.Produces<Produto>(200)
.Produces<ProblemDetails>(404)
.Produces<ProblemDetails>(400);

// Delete produto
app.MapDelete("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    if (!removido)
        return Results.NotFound();

    return Results.NoContent();
})
.WithName("RemoverProduto")
.WithOpenApi()
.WithSummary("Remove um produto")
.Produces(204)
.Produces<ProblemDetails>(404);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

