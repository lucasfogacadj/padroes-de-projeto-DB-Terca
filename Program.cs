using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

builder.Services.AddScoped<IProdutoService, ProdutoService>();


var app = builder.Build();

//Get Listar todos os produtos.
app.MapGet("/produtos", async (IProdutoService service, CancellationToken ct) =>
{
    var produtos = await service.ListarAsync(ct);
    return Results.Ok(produtos);
});
// Get que busca por id.
app.MapGet("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.ObterAsync(id, ct);
    return produto != null ? Results.Ok(produto) : Results.NotFound();
});
//post criar produto
app.MapPost("/produtos", async (ProdutoCreateDto dto, IProdutoService service, CancellationToken ct) =>
{
    var produto = await service.CriarAsync(dto.Nome, dto.Descricao, dto.Preco, dto.Estoque, ct);
    return Results.Created($"/produtos/{produto.Id}", produto);
});

//PUT Atualização completa do produto
app.MapPut("/produtos/{id}", async (int id, Produto produto, IProdutoService service, CancellationToken ct) =>
{
    var produtoAtualizado = await service.AtualizarAsync(id, produto, ct);
    return Results.Ok(produtoAtualizado);
});

app.MapPatch("/produtos/{id}", async (int id, Produto produto, IProdutoService service, CancellationToken ct) =>
{
    var produtoAtualizadoParcialmente = await service.AtualizarParcialAsync(id, produto, ct);
    return Results.Ok(produtoAtualizadoParcialmente);
});
// Delete produto
app.MapDelete("/produtos/{id}", async (int id, IProdutoService service, CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    if (!removido)
        return Results.NotFound();

    return Results.NoContent();
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();


app.Run();

