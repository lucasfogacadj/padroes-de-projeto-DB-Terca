using Application.Interfaces;
using Domain.Entities;
using Application.Services;
using Application.DTOs;
using Application.Exceptions;

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
            throw new ValidationException("id", "O Id do produto deve ser maior que zero.");
        }

        var produto = await _repo.GetByIdAsync(id, ct);
        if(produto == null)
        {
            throw new NotFoundException("Produto", id);
        }
        var produtoDTO = MappingExtensions.ToReadDto(produto);
        return produtoDTO;
    }

    public async Task<Produto> CriarAsync(string nome, string descricao, decimal preco, int estoque, CancellationToken ct = default)
    {
        // Integrar com ProdutoFactory.Criar e depois persistir via repository.
        // Trata regras: nome não vazio, preço > 0, estoque >= 0, trimming.
        
        var produto = ProdutoFactory.Criar(nome, descricao, preco, estoque);
        
        if (string.IsNullOrWhiteSpace(produto.Nome))
        {
            throw new ValidationException("nome", "O nome do produto não pode ser nulo ou vazio.");
        }
        
        if (produto.Preco <= 0)
        {
            throw new ValidationException("preco", "O preço do produto deve ser maior que zero.");
        }
        
        if (produto.Estoque < 0)
        {
            throw new ValidationException("estoque", "O estoque do produto não pode ser negativo.");
        }
        
        // Persistez via repository
        await _repo.AddAsync(produto, ct);
        await _repo.SaveChangesAsync(ct);
        
        return produto;
    }

    public async Task<Produto> AtualizarAsync(int id, ProdutoUpdateDto dto, CancellationToken ct = default)
    {
        // PUT - Substituição total do recurso
        // Todos os campos devem ser fornecidos
        
        // 1. Buscar produto existente
        var produto = await _repo.GetByIdAsync(id, ct);
        if (produto == null)
        {
            throw new NotFoundException("Produto", id);
        }

        // 2. Validar dados (já validados pelos DataAnnotations, mas reforçar regras de negócio)
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ValidationException("nome", "O nome do produto é obrigatório.");
        }

        if (dto.Preco <= 0)
        {
            throw new ValidationException("preco", "O preço deve ser maior que zero.");
        }

        if (dto.Estoque < 0)
        {
            throw new ValidationException("estoque", "O estoque não pode ser negativo.");
        }

        // 3. Substituir TODOS os campos (exceto ID e DataCriacao que são imutáveis)
        produto.Nome = dto.Nome.Trim();
        produto.Descricao = dto.Descricao.Trim();
        produto.Preco = dto.Preco;
        produto.Estoque = dto.Estoque;
        // DataCriacao permanece inalterada

        // 4. Persistir
        await _repo.UpdateAsync(produto, ct);
        await _repo.SaveChangesAsync(ct);

        return produto;
    }

    public async Task<Produto> AtualizarParcialAsync(int id, ProdutoPatchDto dto, CancellationToken ct = default)
    {
        // PATCH - Atualização parcial do recurso
        // Apenas campos fornecidos (não-null) serão atualizados
        
        // 1. Buscar produto existente
        var produto = await _repo.GetByIdAsync(id, ct);
        if (produto == null)
        {
            throw new NotFoundException("Produto", id);
        }

        // 2. Atualizar APENAS campos fornecidos
        if (dto.Nome != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new ValidationException("nome", "O nome não pode ser vazio.");
            }
            produto.Nome = dto.Nome.Trim();
        }

        if (dto.Descricao != null)
        {
            produto.Descricao = dto.Descricao.Trim();
        }

        if (dto.Preco.HasValue)
        {
            if (dto.Preco.Value <= 0)
            {
                throw new ValidationException("preco", "O preço deve ser maior que zero.");
            }
            produto.Preco = dto.Preco.Value;
        }

        if (dto.Estoque.HasValue)
        {
            if (dto.Estoque.Value < 0)
            {
                throw new ValidationException("estoque", "O estoque não pode ser negativo.");
            }
            produto.Estoque = dto.Estoque.Value;
        }

        // 3. Persistir
        await _repo.UpdateAsync(produto, ct);
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
}
