namespace Application.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> ListarAsync(CancellationToken ct = default);
    Task<ProdutoReadDto?> ObterAsync(int id, CancellationToken ct = default);
    Task<Produto> CriarAsync(string nome, string descricao, decimal preco, int estoque, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);

    Task<Produto> AtualizarAsync(int id, Produto produto, CancellationToken ct = default);

    Task<Produto> AtualizarParcialAsync(int id, Produto produto, CancellationToken ct = default);
}
