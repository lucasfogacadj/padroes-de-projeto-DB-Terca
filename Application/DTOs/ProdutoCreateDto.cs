namespace Application.DTOs;

// TODO: O grupo de DTO/Mapping deve avaliar se vai usar 'record', 'class' ou ambos.
// Incluir validações via FluentValidation (em outro grupo) e manter este DTO puro.
// Decidir se campos opcionais (ex: Estoque inicial) precisam de default.
// Adicionar comentários justificando escolhas no PR.


// Escolha de manter o 'record':
// - Imutabilidade: os DTOs de criação não devem ser alterados depois de criados.
// - Comparação por valor: Equals/GetHashCode consideram todos os campos, útil para testes ou logs.
// - Sintaxe concisa: menos código boilerplate que uma class tradicional.
// Nota: se precisássemos de mutabilidade ou integração direta com EF, usaríamos 'class'.

// Default em 'Estoque':
// - Permitimos criar produtos sem especificar estoque inicial, usando 0 como padrão.
// - Isso facilita cenários onde o estoque será ajustado posteriormente.

	
// Mantemos o DTO puro, sem lógica de negócio.


public record ProdutoCreateDto(
	string Nome,
	string Descricao,
	decimal Preco,
	int Estoque = 0
);
