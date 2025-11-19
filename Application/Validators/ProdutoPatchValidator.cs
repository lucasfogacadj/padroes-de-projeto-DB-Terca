using FluentValidation;

public class ProdutoPatchValidator : AbstractValidator<Produto>
{
    public ProdutoPatchValidator()
    {
        RuleFor(p => p.Nome)
            .MaximumLength(200)
            .WithMessage("O nome do produto não pode ter mais de 200 caracteres")
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
            .WithMessage("O nome do produto não pode conter apenas espaços em branco")
            .When(p => !string.IsNullOrWhiteSpace(p.Nome));

        RuleFor(p => p.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição não pode ter mais de 1000 caracteres")
            .When(p => !string.IsNullOrEmpty(p.Descricao));

        RuleFor(p => p.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que 0")
            .PrecisionScale(10, 2, ignoreTrailingZeros:true)
            .WithMessage("O preço deve ter até 2 casas decimais de 10 digitos no total.")
            .When(p => p.Preco > 0);

        RuleFor(p => p.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo")
            .When(p => p.Estoque >= 0);

    }
}