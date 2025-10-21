# Grupo DTO / Mapping

Objetivo: Introduzir DTOs para desacoplar o modelo de domínio/entidade EF da resposta exposta na API e da entrada de criação.

## Tarefas
1. Validar a modelagem dos DTOs atuais (`ProdutoCreateDto`, `ProdutoReadDto`). Ajustar se necessário.
2. Implementar mapping manual em `MappingExtensions.ToReadDto`.
3. (Opcional) Demonstrar uso de AutoMapper e justificar trade-offs.
4. Decidir se vai expor todos os campos ou omitir alguns (ex: `DataCriacao`). Justificar no PR.
5. Garantir que endpoints (quando integrados) não retornem mais a entidade `Produto` diretamente.

## Boas Práticas
- DTO não deve conter lógica de negócio.
- Evitar “inflar” DTO com campos que não têm uso no cliente.
- Mapeamento deve ser unidirecional (entidade -> read DTO). Criação usa `ProdutoCreateDto` separado.

## Extras (Bônus)
- Adicionar campo calculado (ex: ValorEstoque) populado via mapping.
- Adicionar versão do contrato (v1) caso queira demonstrar versionamento.

## Entrega
- Branch: `feature/dto-mapping`
- Atualizar README explicando decisões.
- Pelo menos um teste de mapping (opcional).
Decisões do Grupo DTO
Estrutura e Padrões

Os DTOs foram implementados como record para garantir imutabilidade e comparação por valor, evitando efeitos colaterais.

O padrão de nomenclatura adotado foi PascalCase, conforme convenção do C#.

A serialização JSON aplica camelCase automaticamente via JsonNamingPolicy.CamelCase no Program.cs.

Cada DTO é puro, sem lógica de negócio, servindo apenas como contrato de transporte de dados entre camadas.

ProdutoReadDto

Inclui campos principais da entidade Produto e um campo calculado ValorEstoque, mapeado a partir de Preco * Estoque no MappingExtensions.

Campo ValorEstoque é opcional (decimal?) e não armazenado no banco, servindo apenas para exibição.

ProdutoCreateDto

Usado para criação de produtos, contendo apenas os campos necessários à operação (Nome, Descricao, Preco, Estoque).

Mantém o princípio de segregação de DTOs: criação (CreateDto) e leitura (ReadDto) possuem contratos independentes.

Boas Práticas

Evitar inflar DTOs com campos desnecessários.

O mapeamento segue o fluxo entidade → DTO de leitura.

Comentários TODO foram mantidos para apoiar revisões futuras e padronização do grupo.