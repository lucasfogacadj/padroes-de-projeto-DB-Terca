using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Validador para criação de produtos.
/// Centraliza regras de validação de entrada, separando-as da lógica de negócio.
/// </summary>
public class ProdutoCreateDtoValidator : AbstractValidator<ProdutoCreateDto>
{
    public ProdutoCreateDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do produto não pode ter mais de 200 caracteres.")
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
            .WithMessage("O nome do produto não pode conter apenas espaços em branco.");

        RuleFor(p => p.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição não pode ter mais de 1000 caracteres.")
            .When(p => !string.IsNullOrEmpty(p.Descricao));

        RuleFor(p => p.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, ignoreTrailingZeros: true)
            .WithMessage("O preço deve ter no máximo 2 casas decimais e 10 dígitos no total.");

        RuleFor(p => p.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");
    }
}
