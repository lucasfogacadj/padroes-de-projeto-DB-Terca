using FluentValidation;

public class ProdutoUpdateValidator : AbstractValidator <Produto>
{
    public ProdutoUpdateValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("o nome do produto é obrigatorio")
            .MaximumLength(200)
            .WithMessage("o nome pode ter ate 200 caracteres")
            .Must(nome => !string.IsNullOrWhiteSpace(nome))
            .WithMessage("o nome do produto nao pode conter apenas espaços em branco");

        RuleFor(p => p.Descricao)
            .MaximumLength(1000)
            .WithMessage("a descrição nao pode ter mais de 1000 caracteres")
            .When(p => !string.IsNullOrEmpty(p.Descricao));

        RuleFor(p => p.Preco)
            .GreaterThan(0)
            .WithMessage("O preço precisa ser maior que zero")
            .PrecisionScale(10, 2, ignoreTrailingZeros:true)
            .WithMessage("Preço deve ter no máximo duas casas decimais e dez dígitos no total");

        RuleFor(p => p.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo");
            

    }
}