# Trabalho em Grupo (API de Produtos)

Este repositório está preparado para o trabalho em grupo. A base é uma API minimalista em ASP.NET Core + EF Core. Os grupos devem evoluir a arquitetura aplicando padrões selecionados.

## Objetivos
- Separar responsabilidades (persistência, regras de negócio, exposição).
- Aplicar padrões básicos de forma crítica (quando usar / quando não usar).
- Preparar terreno para fases futuras (logging, CQRS, eventos, etc.).
- Exercitar Git Flow (branch, PR, revisão, justificativa técnica).

## Grupos e Padrões Principais
| Grupo | Padrão | Pasta Principal | Entrega | Observações |
|-------|--------|-----------------|---------|-------------|
| 1 | Repository Pattern | `Infrastructure/Repositories` | `ProdutoRepository` | Focar em persistência, sem regra de negócio |
| 2 | Service (Application Service) | `Application/Services` | `ProdutoService` | Orquestra e centraliza regras |
| 3 | DTO + Mapping | `Application/DTOs` / `Application/Services` | DTOs + mapping | Desacoplar entidade do contrato externo |
| 4 | Validation (FluentValidation) | (a criar) | Validators | Padronizar erros e mensagens |
| 5 | Factory | `Application/Services` | `ProdutoFactory` | Garantir invariantes de criação |

(Se não houver grupo para Validation ainda, ficará como extra.)

## Status dos Grupos
| Grupo | Status | Branch | Observações |
|-------|--------|--------|-------------|
| 1 - Repository | ⏳ Pendente | - | Aguardando implementação |
| 2 - Service | ✅ Concluído | `feature/service` | Implementado com validações |
| 3 - DTO + Mapping | ⏳ Pendente | - | Aguardando implementação |
| 4 - Validation | ✅ Concluído | `main` | FluentValidation implementado |
| 5 - Factory | ⏳ Pendente | - | Aguardando implementação |

## Implementação do Service Pattern (Grupo 2)

### ✅ O que foi implementado
- **ProdutoService**: Classe que centraliza regras de negócio
- **Métodos implementados**:
  - `ListarAsync()` - Lista todos os produtos
  - `ObterAsync(int id)` - Busca produto por ID com validação
  - `CriarAsync(...)` - Cria produto com validações de negócio
  - `RemoverAsync(int id)` - Remove produto com validação de existência

### 🎯 Decisões técnicas tomadas

#### **Estratégia de Tratamento de Erros**
- **ArgumentException** para parâmetros inválidos (ID ≤ 0, preço ≤ 0, estoque < 0)
- **Return false** para operações que podem falhar (produto não encontrado)
- **Justificativa**: Permite tratamento flexível pelo chamador sem forçar try/catch

#### **Validações de Negócio Implementadas**
- ✅ Nome não pode ser nulo ou vazio
- ✅ Preço deve ser maior que zero
- ✅ Estoque não pode ser negativo
- ✅ ID deve ser maior que zero para operações de busca/remoção

#### **Integração com Outros Grupos**
- ✅ Usa `IProdutoRepository` (aguardando implementação do grupo Repository)
- ✅ Usa `ProdutoFactory.Criar()` (aguardando implementação do grupo Factory)
- ✅ Retorna entidade `Produto` (compatível com grupo DTO/Mapping)

## Implementação do Validation Pattern (Grupo 4)

### ✅ O que foi implementado
- **FluentValidation**: Biblioteca para validações declarativas e reutilizáveis
- **Validadores criados**:
  - `ProdutoCreateDtoValidator` - Validação para criação
  - `ProdutoUpdateValidator` - Validação para atualização completa (PUT)
  - `ProdutoPatchValidator` - Validação para atualização parcial (PATCH)

### 🎯 Decisões técnicas tomadas

#### **Separação de Responsabilidades**
- **Validação de entrada** (FluentValidation) vs **Invariantes de domínio** (entidades)
- Validadores focam em formato/sintaxe dos dados
- Regras de negócio complexas permanecem no Service/Domain

#### **Regras de Validação**
- ✅ Nome: obrigatório, máx 200 caracteres, não apenas espaços
- ✅ Descrição: opcional, máx 1000 caracteres
- ✅ Preço: obrigatório, > 0, máx 2 casas decimais
- ✅ Estoque: obrigatório, >= 0

#### **Integração com Endpoints**
- Validação automática antes de processar requisições
- Retorna `ValidationProblem` (RFC 7807) em caso de erros
- Mensagens de erro em português

#### **Benefícios**
- ✅ Código mais limpo e testável
- ✅ Validações centralizadas e reutilizáveis
- ✅ Mensagens de erro consistentes
- ✅ Fácil manutenção e extensão

### 📚 Documentação
- `Application/Validators/README.md` - Explicação completa dos validadores
- `Application/Validators/ExemplosTestes.md` - Exemplos de testes unitários

## 🧪 Testando a API

### Postman Collection
A collection do Postman foi atualizada com **testes completos de FluentValidation**:

📦 **APIProdutos.postman_collection.json** (v2.0)
- ✅ 35+ requests organizados
- ✅ 15 testes específicos de FluentValidation
- ✅ Scripts de teste automatizados
- ✅ Documentação detalhada de cada endpoint

**Pastas na Collection**:
1. **Produtos** - CRUD completo com validações
2. **FluentValidation - Testes** ⭐ - Testes organizados por campo
3. **Health Check** - Verificação de disponibilidade

**Como usar**:
1. Importar `APIProdutos.postman_collection.json` no Postman
2. Executar "Run folder" em "FluentValidation - Testes"
3. Ver todos os testes passarem/falharem automaticamente

📖 Documentação completa: `docs/PostmanCollectionUpdate.md`

## Ordem Sugerida de Integração
1. Repository
2. Service ✅
3. Factory + DTO/Mapping (podem andar em paralelo se coordenados)
4. Validation (após DTO pronto)

## Branch Naming
`feature/<nome-padrao>` – exemplos:
- `feature/repository`
- `feature/service` ✅
- `feature/dto-mapping`
- `feature/factory`
- `feature/validation`

## Checklist de PR
- [ ] Branch criada corretamente
- [ ] Escopo único (apenas o padrão do grupo)
- [ ] README da pasta atualizado/expandido
- [ ] Código compila (`dotnet build`)
- [ ] Explicação: Quando NÃO usar este padrão
- [ ] (Opcional) Teste simples
- [ ] Endpoints ainda funcionam (quando integrados)

## Critérios de Avaliação (Rubrica Simplificada)
| Critério | Peso |
|----------|------|
| Implementação correta do padrão | 4 |
| Separação de responsabilidades | 3 |
| Clareza (nomes / organização) | 2 |
| Documentação / justificativa | 2 |
| Integração sem quebrar API | 2 |
| Extras (testes, reflexão crítica) | +2 bônus |

## Próximos Passos dos Grupos
- Abrir PR cedo em modo rascunho (draft) para receber feedback.
- Escrever no corpo do PR: Antes/Depois (trecho de código breve) e justificativa.
- Não misturar mudanças não relacionadas (ex: não configurar logging agora).

## Dúvidas Frequentes
**Por que não deixar tudo nos endpoints?** Para escalar manutenção e testar regras isoladamente.
**Repository + EF Core é redundante?** Em projetos simples, sim — justificar no PR.
**Preciso usar AutoMapper?** Não; mapping manual é pedagógico.
**Onde ficam validações?** Invariantes críticas podem estar na Factory; validações de entrada no Validator.


Boa implementação! Lembrem-se de justificar escolhas, não só “fazer”.

---
### Documentação da Estrutura do Projeto
Para uma explicação detalhada das camadas, responsabilidades e decisões didáticas, consulte o documento:
`docs/EstruturaProjeto.md`

Esse material ajuda a entender onde cada novo arquivo deve ser colocado conforme o projeto evoluir.