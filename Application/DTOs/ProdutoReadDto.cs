namespace Application.DTOs;

// TODO: Confirmar se todos os campos devem ser expostos ao cliente.
// Possíveis extensões: adicionar campo calculado (ex: ValorEstoque = Preco * Estoque) via mapping.
// Decidir padrão de nome (PascalCase / camelCase) conforme política de serialização.

// Mantido como 'record' para imutabilidade e comparação por valor.
// DTO puro: sem lógica de negócio, usado apenas para transporte de dados.
// Campo opcional 'ValorEstoque' adicionado para conveniência na leitura (calculado via mapping).

public record ProdutoReadDto(
	int Id,
	string Nome,
	string Descricao,
	decimal Preco,
	int Estoque,
	DateTime DataCriacao,
	decimal? ValorEstoque = null // Calculado (Preco * Estoque), não armazenado no banco
);

