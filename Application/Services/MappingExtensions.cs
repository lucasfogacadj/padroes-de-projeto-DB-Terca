using Application.DTOs;

namespace Application.Services;

// TODO (Grupo DTO/Mapping): Implementar conversão de Produto -> ProdutoReadDto.
// Discutir se mapping manual é suficiente ou se vão demonstrar o uso de AutoMapper (opcional).
// Possível extensão: adicionar campo calculado no DTO.
public static class MappingExtensions
{
    public static ProdutoReadDto? ToReadDto(this Produto p)
    {
        // TODO: retornar nova instância de ProdutoReadDto usando p (mapping manual inicial).
        // Verifica se o produto é nulo para evitar NullReferenceException.
        
        // Isso garante maior segurança em chamadas onde o produto pode não ter sido encontrado.
        if (p == null) return null;    
        // Retorna uma nova instância de ProdutoReadDto, preenchendo os campos manualmente a partir da entidade Produto.

            // A opção por um "mapeamento manual" foi feita por ser mais transparente neste estágio do projeto:  preenchendo os campos manualmente a partir da entidade Produto.
            // - Permite compreender claramente como os dados são transferidos entre camadas,
            // - Evita dependências externas (como AutoMapper) enquanto a arquitetura ainda está sendo estruturada.
            // - Facilita o controle sobre quais campos são expostos ao cliente da API.
            //
            // Posteriormente, esse método pode ser substituído ou complementado pelo uso do AutoMapper
            // para reduzir código repetido e simplificar o mapeamento de estruturas maiores.
            return new ProdutoReadDto(
                Id: p.Id,                  // Identificador único do produto
                Nome: p.Nome,              // Nome comercial do produto
                Descricao: p.Descricao,    // Descrição detalhada
                Preco: p.Preco,            // Preço unitário
                Estoque: p.Estoque,        // Quantidade disponível em estoque
                DataCriacao: p.DataCriacao, // Data de cadastro do produto no sistema
                ValorEstoque: p.Preco * p.Estoque // Calcula automaticamente o valor total em estoque
            );
    }
}
