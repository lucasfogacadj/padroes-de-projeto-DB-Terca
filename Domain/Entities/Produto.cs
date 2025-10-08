using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Produto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public decimal Preco { get; set; }

    [Required]
    public int Estoque { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
}
