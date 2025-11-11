using Domain.Entities;
using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Validador para atualização parcial de produtos (PATCH).
/// Valida apenas os campos que foram fornecidos.
/// </summary>
public class ProdutoPatchValidator : AbstractValidator<Produto>
{
    public ProdutoPatchValidator()
    {
        // Para PATCH, validamos apenas se os campos fornecidos são válidos
        // Não exigimos que todos os campos estejam presentes
        
        RuleFor(p => p.Nome)
            .MaximumLength(200)
            .WithMessage("O nome do produto não pode ter mais de 200 caracteres.")
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
            .WithMessage("O nome do produto não pode conter apenas espaços em branco.")
            .When(p => !string.IsNullOrEmpty(p.Nome));

        RuleFor(p => p.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição não pode ter mais de 1000 caracteres.")
            .When(p => !string.IsNullOrEmpty(p.Descricao));

        RuleFor(p => p.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, ignoreTrailingZeros: true)
            .WithMessage("O preço deve ter no máximo 2 casas decimais e 10 dígitos no total.")
            .When(p => p.Preco > 0);

        RuleFor(p => p.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.")
            .When(p => p.Estoque >= 0);
    }
}
