namespace Application.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAllAsync(CancellationToken ct = default);
    Task<Produto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Produto produto, CancellationToken ct = default);
    Task RemoveAsync(Produto produto, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);

    Task UpdateAsync(Produto produto, CancellationToken ct = default); 
}
