using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// DTO para atualização PARCIAL de produto (PATCH).
/// Todos os campos são opcionais (nullable) - apenas campos enviados serão atualizados.
/// </summary>
public class ProdutoPatchDto
{
    /// <summary>
    /// Nome do produto (opcional).
    /// Se fornecido, deve ter entre 3 e 100 caracteres.
    /// </summary>
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string? Nome { get; set; }

    /// <summary>
    /// Descrição do produto (opcional).
    /// Se fornecida, deve ter no máximo 500 caracteres.
    /// </summary>
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço do produto (opcional).
    /// Se fornecido, deve ser maior que zero.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal? Preco { get; set; }

    /// <summary>
    /// Quantidade em estoque (opcional).
    /// Se fornecida, não pode ser negativa.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int? Estoque { get; set; }
}
