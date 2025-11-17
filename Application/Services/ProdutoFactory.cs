namespace Application.Services;

// TODO (Grupo Factory): Centralizar criação de Produto garantindo invariantes.
// Sugestões de validação: nome não vazio, descricao não vazia, preco > 0, estoque >= 0.
// Discutir se deve lançar ArgumentException, DomainException custom ou retornar Result.
// Explicar no PR por que uma Factory faz sentido (ou se seria overkill neste tamanho) — reflexão.
public static class ProdutoFactory
{
    public static Produto Criar(string nome, string descricao, decimal preco, int estoque)
    {
          if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome é inválido...", nameof(nome));
        }
        
        if (preco <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(preco), "O preço tem que ser maior que 0.");
        }

        if (estoque < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(estoque), "O estoque tem que ser postivo");
        }

        Console.WriteLine("Produto validado com sucesso! Criando...");
        Produto produtoValido = new Produto();
        produtoValido.Nome = nome;
        produtoValido.Descricao = descricao;
        produtoValido.Preco = preco;
        produtoValido.Estoque = estoque;
        produtoValido.DataCriacao = DateTime.Now;
        return produtoValido;
    }
}
