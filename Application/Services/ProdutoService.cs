using Application.Interfaces;
using Domain.Entities;
using Application.Services;
using Application.DTOs;

namespace Application.Services;

// TODO (Grupo Service): Implementar regras de negócio aqui.
// NÃO colocar detalhes de EF Core. Usar apenas a abstração IProdutoRepository.
// Integrar posteriormente com validações (FluentValidation) e Factory.
// Sugerido: lançar exceções de domínio específicas ou retornar Result Pattern (opcional, comentar no PR).
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repo;

    public ProdutoService(IProdutoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Produto>> ListarAsync(CancellationToken ct = default)
    {
        return await _repo.GetAllAsync(ct);
    }

    public async Task<ProdutoReadDto?> ObterAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
        {
            throw new ArgumentException("O Id do produto deve ser maior que zero.", nameof(id));
        }

        var produto = await _repo.GetByIdAsync(id, ct);
        if(produto == null)
        {
            throw new ArgumentException("Ppoduto não encontrado");
        }
        var produtoDTO = MappingExtensions.ToReadDto(produto);
        return produtoDTO;
    }

    public async Task<Produto> CriarAsync(string nome, string descricao, decimal preco, int estoque, CancellationToken ct = default)
    {
        // Integrar com ProdutoFactory.Criar e depois persistir via repository.
        // Trata regras: nome não vazio, preço > 0, estoque >= 0, trimming.
        
        var produto = ProdutoFactory.Criar(nome, descricao, preco, estoque);
        
        if (string.IsNullOrEmpty(produto.Nome))
        {
            throw new ArgumentException("O nome do produto não pode ser nulo ou vazio. ", nameof(nome));
        }
        
        if (produto.Preco < 0)
        {
            throw new ArgumentException("O preço do produto deve ser maior que zero.", nameof(preco));
        }
        
        if (produto.Estoque < 0)
        {
            throw new ArgumentException("O estoque do produto deve ser maior que zero.", nameof(estoque));
        }
        
        // Persistez via repository
        await _repo.AddAsync(produto, ct);
        await _repo.SaveChangesAsync(ct);
        
        return produto;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        // Busca, valida existência e remove

        var produto = await _repo.GetByIdAsync(id, ct);

        if (produto == null)
        {
            return false;
        }

        await _repo.RemoveAsync(produto, ct);
        await _repo.SaveChangesAsync(ct);

        return true;
    }

    public async Task<Produto> AtualizarAsync(int id, Produto produto, CancellationToken ct = default)
    {
        var produtoEncontrado = await _repo.GetByIdAsync(id, ct);
        if (produto == null)
        {
            throw new NotImplementedException("produto");
        }

        var produtoAtualizado = ProdutoFactory.Criar(produto.Nome, produto.Descricao, produto.Preco, produto.Estoque);
        await _repo.UpdateAsync(produtoAtualizado, ct);
        await _repo.SaveChangesAsync(ct);
        return produtoAtualizado;
    }

    public async Task<Produto> AtualizarParcialAsync(int id, Produto produto, CancellationToken ct = default)
    {
        var produtoEncontrado = await _repo.GetByIdAsync(id, ct);
        if (produtoEncontrado == null){
            throw new NotImplementedException("produto");
        }
        if (produto.Nome != null)
        {   produtoEncontrado.Nome = produto.Nome;
        }
        if (produto.Preco != null) {
            produtoEncontrado.Preco = produto.Preco;
        }
        if (produto.Descricao != null) {
            produtoEncontrado.Descricao = produto.Descricao;
        }
        if (produto.Estoque != null)
        {
            produtoEncontrado.Estoque = produto.Estoque;
        }       
        await _repo.UpdateAsync(produtoEncontrado, ct);
        await _repo.SaveChangesAsync(ct);
        return produtoEncontrado;
    }
}
