using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// DTO para atualização COMPLETA de produto (PUT).
/// Todos os campos são obrigatórios - representa substituição total do recurso.
/// </summary>
public class ProdutoUpdateDto
{
    /// <summary>
    /// Nome do produto (obrigatório).
    /// </summary>
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do produto (obrigatório).
    /// </summary>
    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Preço do produto (obrigatório, maior que zero).
    /// </summary>
    [Required(ErrorMessage = "O preço do produto é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade em estoque (obrigatório, não negativo).
    /// </summary>
    [Required(ErrorMessage = "O estoque do produto é obrigatório.")]
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }
}
