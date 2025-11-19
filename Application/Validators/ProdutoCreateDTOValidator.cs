using System.Data;
using Application.DTOs;
using FluentValidation;

public class ProdutoCreateDtoValidator : AbstractValidator<ProdutoCreateDto>
{
    public ProdutoCreateDtoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome do produto é obrigatório")
            .MaximumLength(200)
            .WithMessage("O nome do produto não pode ter mais do que 200 caracteres");

        RuleFor(p => p.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição não pode ter mais de 1000 caracteres");

        RuleFor(p => p.Preco)
            .GreaterThan(0)
            .WithMessage("Preço deve ser maior que 0")
            .PrecisionScale(10, 2, ignoreTrailingZeros:true)
            .WithMessage("Preço deve ter no máximo duas casas decimais e 10 dígitos no total.");

        RuleFor(p => p.Estoque)
           .GreaterThanOrEqualTo(0)
           .WithMessage("Estoque deve ser maior que 0");
            
    }
}