# AVALIAÇÃO SEMESTRAL (AS) - DESENVOLVIMENTO BACKEND

## 📋 Informações Gerais

**Disciplina**: Desenvolvimento Backend  
**Período**: 2025/2  
**Valor**: 10,0 pontos  
**Peso**: 40% da nota final  
**Modalidade**: Individual  
**Data de Lançamento**: 18/11/2025  
**Data de Entrega**: 16/12/2025 às 23:59  
**Plataforma de Entrega**: Blackboard/Moodle  

---

## 🎯 Objetivos da Avaliação

Esta avaliação tem como objetivo verificar a capacidade do aluno de:

1. **Implementar** uma API REST completa utilizando ASP.NET Core com Minimal APIs
2. **Aplicar** padrões de projeto (Design Patterns) em um contexto real
3. **Estruturar** código seguindo princípios de Clean Architecture
4. **Persistir** dados utilizando Entity Framework Core
5. **Validar** entrada de dados com FluentValidation
6. **Documentar** decisões técnicas de forma acadêmica
7. **Apresentar** soluções técnicas de forma clara e objetiva

---

## 📝 Descrição do Problema

Você foi contratado(a) como desenvolvedor(a) backend para criar uma **API de Gerenciamento de Usuários** para uma plataforma digital. A API deve permitir o cadastro, consulta, atualização e remoção de usuários do sistema, seguindo as melhores práticas de desenvolvimento e aplicando padrões de projeto adequados.

### Contexto de Negócio

A empresa precisa de uma solução que:
- Permita operações CRUD (Create, Read, Update, Delete) sobre usuários
- Garanta validação rigorosa dos dados de entrada
- Seja escalável e de fácil manutenção
- Separe responsabilidades em camadas bem definidas
- Utilize banco de dados relacional para persistência

---

## 🔧 Requisitos Técnicos Obrigatórios

### 1. Tecnologias e Frameworks

#### 1.1 Plataforma e Linguagem
- ✅ **.NET 8.0 ou superior**
- ✅ **C# 12.0**
- ✅ **ASP.NET Core** com **Minimal APIs**

#### 1.2 Banco de Dados
- ✅ **Entity Framework Core** (versão 8.0+)
- ✅ **SQLite** (para desenvolvimento e entrega)
- ✅ **Code First** com Migrations

#### 1.3 Bibliotecas Externas
- ✅ **FluentValidation.AspNetCore** (versão 11.3+)
- ✅ Outras bibliotecas necessárias para implementação

### 2. Estrutura do Projeto

O projeto deve seguir a estrutura de **Clean Architecture** com as seguintes camadas:

```
APIUsuarios/
├── Domain/
│   └── Entities/
│       └── Usuario.cs
│
├── Application/
│   ├── DTOs/
│   │   ├── UsuarioCreateDto.cs
│   │   ├── UsuarioReadDto.cs
│   │   └── UsuarioUpdateDto.cs
│   │
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs
│   │   └── IUsuarioService.cs
│   │
│   ├── Services/
│   │   └── UsuarioService.cs
│   │
│   └── Validators/
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
│
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs
│   │
│   └── Repositories/
│       └── UsuarioRepository.cs
│
├── Migrations/
│   └── (geradas automaticamente)
│
├── Program.cs
├── appsettings.json
└── APIUsuarios.csproj
```

### 3. Entidade Usuario

A entidade **Usuario** deve conter **obrigatoriamente** os seguintes atributos:

```csharp
public class Usuario
{
    public int Id { get; set; }                    // PK, Auto-increment
    public string Nome { get; set; }               // Obrigatório, 3-100 caracteres
    public string Email { get; set; }              // Obrigatório, formato válido, único
    public string Senha { get; set; }              // Obrigatório, min 6 caracteres
    public DateTime DataNascimento { get; set; }   // Obrigatório, idade >= 18 anos
    public string Telefone { get; set; }           // Opcional, formato (XX) XXXXX-XXXX
    public bool Ativo { get; set; }                // Obrigatório, default true
    public DateTime DataCriacao { get; set; }      // Obrigatório, preenchido automaticamente
    public DateTime? DataAtualizacao { get; set; } // Opcional, atualizado automaticamente
}
```

**Observações**:
- O campo `Senha` deve ser armazenado de forma segura (hash)
- O campo `Email` deve ser único no banco de dados
- Os campos de data devem seguir formato ISO 8601

### 4. Padrões de Projeto Obrigatórios

#### 4.1 Repository Pattern
- **Interface**: `IUsuarioRepository`
- **Implementação**: `UsuarioRepository`
- **Responsabilidade**: Abstração da camada de persistência de dados

**Métodos obrigatórios**:
```csharp
Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct);
Task<Usuario?> GetByIdAsync(int id, CancellationToken ct);
Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);
Task AddAsync(Usuario usuario, CancellationToken ct);
Task UpdateAsync(Usuario usuario, CancellationToken ct);
Task RemoveAsync(Usuario usuario, CancellationToken ct);
Task<bool> EmailExistsAsync(string email, CancellationToken ct);
Task<int> SaveChangesAsync(CancellationToken ct);
```

#### 4.2 Service Pattern
- **Interface**: `IUsuarioService`
- **Implementação**: `UsuarioService`
- **Responsabilidade**: Lógica de negócio e orquestração

**Métodos obrigatórios**:
```csharp
Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct);
Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct);
Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct);
Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct);
Task<bool> RemoverAsync(int id, CancellationToken ct);
Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct);
```

#### 4.3 DTO Pattern
Criar DTOs específicos para cada operação:

**UsuarioCreateDto**:
```csharp
public record UsuarioCreateDto(
    string Nome,
    string Email,
    string Senha,
    DateTime DataNascimento,
    string? Telefone
);
```

**UsuarioReadDto**:
```csharp
public record UsuarioReadDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao
);
```

**UsuarioUpdateDto**:
```csharp
public record UsuarioUpdateDto(
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo
);
```

**Importante**: DTOs **NÃO** devem expor a senha do usuário

#### 4.4 FluentValidation

Implementar validadores para:

**UsuarioCreateDtoValidator**:
- Nome: obrigatório, entre 3 e 100 caracteres
- Email: obrigatório, formato válido, único no banco
- Senha: obrigatória, mínimo 6 caracteres
- DataNascimento: obrigatória, idade mínima 18 anos
- Telefone: opcional, formato brasileiro válido

**UsuarioUpdateDtoValidator**:
- Mesmas regras do Create, exceto senha
- Email deve ser único (exceto o próprio usuário)

### 5. Endpoints da API

Implementar os seguintes endpoints:

| Método | Endpoint | Descrição | Status Code Sucesso |
|--------|----------|-----------|---------------------|
| GET | `/usuarios` | Lista todos os usuários | 200 OK |
| GET | `/usuarios/{id}` | Busca usuário por ID | 200 OK |
| POST | `/usuarios` | Cria novo usuário | 201 Created |
| PUT | `/usuarios/{id}` | Atualiza usuário completo | 200 OK |
| DELETE | `/usuarios/{id}` | Remove usuário (soft delete) | 204 No Content |

**Códigos de erro esperados**:
- `400 Bad Request` - Validação falhou
- `404 Not Found` - Usuário não encontrado
- `409 Conflict` - Email já cadastrado
- `500 Internal Server Error` - Erro no servidor

### 6. Validações de Negócio

Além das validações do FluentValidation, implementar:

1. **Email único**: Não permitir cadastro de emails duplicados
2. **Idade mínima**: Usuário deve ter pelo menos 18 anos
3. **Soft Delete**: Ao deletar, marcar `Ativo = false` ao invés de remover do banco
4. **Hash de senha**: Senha deve ser armazenada com hash (sugestão: BCrypt)
5. **Normalização**: Email deve ser armazenado em lowercase

### 7. Configurações Adicionais

#### 7.1 CORS
Configurar CORS para permitir requisições de qualquer origem (desenvolvimento)

#### 7.2 Swagger/OpenAPI
Incluir documentação automática da API com Swagger

#### 7.3 Tratamento de Erros
Implementar retorno padronizado de erros (Problem Details - RFC 7807)

---

## 📦 Entregáveis

A avaliação é composta por **3 (três) entregáveis obrigatórios**:

### Entregável 1: Repositório GitHub (4,0 pontos)

#### Requisitos:
1. **Repositório público** no GitHub
2. **Nome do repositório**: `api-usuarios-as-[seu-nome]`
   - Exemplo: `api-usuarios-as-joao-silva`
3. **Estrutura do repositório**:
   ```
   api-usuarios-as-[seu-nome]/
   ├── APIUsuarios/               # Código-fonte da API
   ├── README.md                  # Documentação técnica
   ├── .gitignore                 # Arquivo gitignore do .NET
   └── APIUsuarios.postman_collection.json  # Collection do Postman
   ```

#### README.md (Obrigatório):

O README deve conter:

```markdown
# API de Gerenciamento de Usuários

## Descrição
Breve descrição do projeto (2-3 parágrafos)

## Tecnologias Utilizadas
- .NET 8.0
- Entity Framework Core
- SQLite
- FluentValidation
- Outras...

## Padrões de Projeto Implementados
- Repository Pattern
- Service Pattern
- DTO Pattern
- Dependency Injection

## Como Executar o Projeto

### Pré-requisitos
- .NET SDK 8.0 ou superior

### Passos
1. Clone o repositório
2. Execute as migrations
3. Execute a aplicação
4. Acesse o Swagger

### Exemplos de Requisições
(Incluir exemplos curl ou JSON)

## Estrutura do Projeto
Explicação das pastas e arquivos

## Autor
Seu nome completo
RA: Seu RA
Curso: [Nome do curso]
```

#### Collection do Postman (Obrigatório):
- Incluir todos os endpoints implementados
- Exemplos de requisições válidas e inválidas
- Testes automatizados (opcional, mas recomendado)

#### Commits:
- **Mínimo de 10 commits** bem descritivos
- Exemplo de boas mensagens:
  - ✅ `feat: implementar entidade Usuario`
  - ✅ `feat: adicionar Repository Pattern`
  - ✅ `feat: implementar validações com FluentValidation`
  - ❌ `atualização` (muito genérico)

#### Critérios de Avaliação - Repositório:

| Critério | Pontos | Descrição |
|----------|--------|-----------|
| **Código Funcional** | 1,5 | API compila e executa sem erros |
| **Padrões Implementados** | 1,0 | Repository, Service, DTO, FluentValidation |
| **Estrutura do Projeto** | 0,5 | Organização de pastas e arquivos |
| **README.md Completo** | 0,5 | Documentação técnica adequada |
| **Collection Postman** | 0,3 | Todos os endpoints documentados |
| **Commits** | 0,2 | Histórico de commits coerente |
| **TOTAL** | **4,0** | |

---

### Entregável 2: Documento Acadêmico (4,0 pontos)

#### Formato:
- **Tipo**: Artigo/Relatório Técnico
- **Formato**: PDF
- **Páginas**: 10 a 15 páginas (incluindo capa e referências)
- **Normas**: ABNT (NBR 14724, NBR 6023, NBR 10520)
- **Fonte**: Times New Roman ou Arial, tamanho 12
- **Espaçamento**: 1,5 linhas
- **Margens**: 3cm (superior e esquerda), 2cm (inferior e direita)

#### Estrutura Obrigatória:

**1. CAPA** (conforme modelo da instituição)
- Nome da Instituição
- Curso
- Disciplina
- Título do Trabalho
- Nome do Aluno
- RA
- Cidade e Data

**2. RESUMO** (150-250 palavras)
- Contexto do trabalho
- Objetivo
- Metodologia aplicada
- Principais resultados
- Palavras-chave (3-5)

**3. SUMÁRIO**

**4. INTRODUÇÃO** (1-2 páginas)
- Contextualização do problema
- Objetivos gerais e específicos
- Justificativa
- Organização do documento

**5. FUNDAMENTAÇÃO TEÓRICA** (3-4 páginas)

Abordar os seguintes tópicos:

**5.1 API REST**
- Definição e características
- Métodos HTTP
- Status codes

**5.2 Clean Architecture**
- Conceito de separação em camadas
- Benefícios da abordagem

**5.3 Padrões de Projeto Utilizados**

**5.3.1 Repository Pattern**
- Definição
- Objetivo
- Como foi implementado no projeto
- Vantagens e desvantagens
- Trecho de código ilustrativo

**5.3.2 Service Pattern**
- Definição
- Objetivo
- Como foi implementado no projeto
- Vantagens e desvantagens
- Trecho de código ilustrativo

**5.3.3 DTO (Data Transfer Object)**
- Definição
- Objetivo
- Como foi implementado no projeto
- Vantagens e desvantagens
- Trecho de código ilustrativo

**5.3.4 Dependency Injection**
- Definição
- Objetivo
- Como foi implementado no projeto
- Vantagens e desvantagens

**5.4 FluentValidation**
- Definição
- Vantagens sobre DataAnnotations
- Implementação no projeto

**6. DESENVOLVIMENTO** (3-4 páginas)

**6.1 Arquitetura da Solução**
- Diagrama de camadas (desenho/figura)
- Descrição de cada camada

**6.2 Modelagem de Dados**
- Diagrama ER ou modelo de classe
- Justificativa dos atributos

**6.3 Fluxo de Requisições**
- Diagrama de sequência (opcional)
- Descrição do fluxo desde o endpoint até o banco

**6.4 Decisões Técnicas**
- Por que escolher SQLite?
- Por que usar Minimal APIs?
- Outras decisões relevantes

**6.5 Desafios Enfrentados**
- Dificuldades encontradas
- Soluções aplicadas
- Aprendizados

**7. RESULTADOS** (1-2 páginas)
- Endpoints implementados (tabela)
- Testes realizados
- Prints de tela (opcional)
- Análise crítica da solução

**8. CONCLUSÃO** (1 página)
- Síntese do trabalho
- Objetivos alcançados
- Trabalhos futuros
- Considerações finais

**9. REFERÊNCIAS**
- Mínimo de 8 referências
- Incluir: documentação oficial, artigos, livros
- Formato ABNT

**Exemplos de referências esperadas**:
```
MICROSOFT. ASP.NET Core documentation. Disponível em: 
<https://docs.microsoft.com/aspnet/core>. Acesso em: 18 nov. 2025.

MARTIN, Robert C. Clean Architecture: A Craftsman's Guide to Software 
Structure and Design. Boston: Prentice Hall, 2017.

FOWLER, Martin. Patterns of Enterprise Application Architecture. 
Boston: Addison-Wesley, 2002.

GAMMA, Erich et al. Design Patterns: Elements of Reusable 
Object-Oriented Software. Boston: Addison-Wesley, 1994.
```

#### Critérios de Avaliação - Documento:

| Critério | Pontos | Descrição |
|----------|--------|-----------|
| **Estrutura ABNT** | 0,5 | Formatação, capa, referências |
| **Qualidade do Texto** | 0,5 | Clareza, coesão, gramática |
| **Fundamentação Teórica** | 1,5 | Conceitos explicados corretamente |
| **Análise dos Padrões** | 1,0 | Compreensão dos Design Patterns |
| **Desenvolvimento** | 0,8 | Explicação da arquitetura e decisões |
| **Resultados e Conclusão** | 0,5 | Análise crítica da solução |
| **Referências** | 0,2 | Mínimo 8 referências, bem formatadas |
| **TOTAL** | **4,0** | |

---

### Entregável 3: Vídeo Demonstrativo (2,0 pontos)

#### Formato:
- **Duração**: 5 a 10 minutos
- **Formato**: MP4, AVI ou Link (YouTube, Google Drive)
- **Resolução**: Mínimo 720p
- **Áudio**: Claro e audível

#### Conteúdo Obrigatório:

**1. Introdução (30s - 1min)**
- Apresentação pessoal
- Breve descrição do projeto

**2. Estrutura do Projeto (1-2min)**
- Mostrar a estrutura de pastas no VS Code ou Visual Studio
- Explicar brevemente cada camada
- Mostrar arquivos principais

**3. Explicação de Código (2-3min)**

Mostrar e explicar:
- **Entidade Usuario**: atributos e anotações
- **Repository**: interface e implementação (método exemplo)
- **Service**: lógica de negócio (método exemplo)
- **DTO**: exemplo de um DTO
- **Validator**: regras de validação (exemplo)
- **Program.cs**: configuração de DI e endpoints

**4. Demonstração Prática (2-3min)**

Usando **Postman** ou **Swagger**:
- ✅ **POST /usuarios**: Criar usuário com dados válidos (201)
- ❌ **POST /usuarios**: Tentar criar usuário com email duplicado (409)
- ❌ **POST /usuarios**: Tentar criar usuário com dados inválidos (400)
- ✅ **GET /usuarios**: Listar todos os usuários (200)
- ✅ **GET /usuarios/{id}**: Buscar usuário específico (200)
- ✅ **PUT /usuarios/{id}**: Atualizar usuário (200)
- ✅ **DELETE /usuarios/{id}**: Remover usuário (204)
- **Mostrar no banco**: Verificar que o usuário deletado tem Ativo=false

**5. Considerações Finais (30s - 1min)**
- Resumo do que foi implementado
- Principais aprendizados
- Agradecimento

#### Requisitos Técnicos:
- ✅ Gravação de tela com áudio (narração)
- ✅ Mostrar código e testes funcionando
- ✅ Qualidade de áudio compreensível
- ✅ Boa dicção e ritmo de apresentação
- ❌ NÃO usar música de fundo alta
- ❌ NÃO incluir conteúdo não relacionado

#### Ferramentas Sugeridas:
- **Gravação**: OBS Studio, Loom, Zoom, Microsoft Teams
- **Edição**: DaVinci Resolve, Shotcut (gratuitos)
- **Hospedagem**: YouTube (não listado), Google Drive (link público)

#### Critérios de Avaliação - Vídeo:

| Critério | Pontos | Descrição |
|----------|--------|-----------|
| **Duração Adequada** | 0,2 | Entre 5 e 10 minutos |
| **Apresentação Pessoal** | 0,2 | Identificação e introdução clara |
| **Explicação da Estrutura** | 0,4 | Mostra e explica camadas do projeto |
| **Demonstração de Código** | 0,6 | Explica padrões implementados |
| **Testes Funcionais** | 0,4 | Demonstra endpoints funcionando |
| **Qualidade Técnica** | 0,2 | Áudio, vídeo e clareza |
| **TOTAL** | **2,0** | |

---

## 📤 Instruções de Entrega

### Formato de Entrega

Na plataforma Blackboard/Moodle, entregar **1 (um) arquivo ZIP** contendo:

```
AS_Backend_[SEUNOME]_[RA].zip
├── 1_Link_Repositorio.txt         # Link do GitHub
├── 2_Documento_Academico.pdf      # Documento em PDF
└── 3_Link_Video.txt               # Link do vídeo
```

### Conteúdo dos Arquivos TXT:

**1_Link_Repositorio.txt**:
```
Nome: João Silva
RA: 123456789
Link do Repositório: https://github.com/joaosilva/api-usuarios-as-joao-silva
```

**3_Link_Video.txt**:
```
Nome: João Silva
RA: 123456789
Link do Vídeo: https://www.youtube.com/watch?v=XXXXXXXXX
ou
Link do Vídeo: https://drive.google.com/file/d/XXXXXXXXX
```

### Checklist de Entrega

Antes de enviar, verifique:

- [ ] Repositório GitHub é público e acessível
- [ ] README.md está completo e bem formatado
- [ ] Collection do Postman está incluída no repositório
- [ ] Código compila sem erros (`dotnet build`)
- [ ] Todos os endpoints funcionam corretamente
- [ ] Documento PDF está no formato ABNT
- [ ] Documento tem entre 10-15 páginas
- [ ] Mínimo de 8 referências bibliográficas
- [ ] Vídeo tem entre 5-10 minutos
- [ ] Vídeo mostra todos os requisitos funcionando
- [ ] Áudio do vídeo está claro e audível
- [ ] Links funcionam e são públicos
- [ ] Nome do arquivo ZIP está correto
- [ ] Todos os 3 arquivos estão dentro do ZIP

---

## 📊 Critérios de Avaliação - Resumo

### Distribuição de Pontos

| Entregável | Pontuação | Peso |
|------------|-----------|------|
| **1. Repositório GitHub** | 4,0 | 40% |
| **2. Documento Acadêmico** | 4,0 | 40% |
| **3. Vídeo Demonstrativo** | 2,0 | 20% |
| **TOTAL** | **10,0** | 100% |

### Rubricas Detalhadas

#### Repositório GitHub (4,0 pontos)

**Excelente (3,5 - 4,0)**:
- Código limpo, organizado e bem comentado
- Todos os padrões implementados corretamente
- README completo e profissional
- Collection do Postman com testes
- Commits bem descritivos e frequentes
- Segue todas as convenções de código

**Bom (2,8 - 3,4)**:
- Código funcional com pequenas inconsistências
- Padrões implementados mas com melhorias possíveis
- README completo mas poderia ser mais detalhado
- Collection do Postman básica
- Commits razoáveis

**Regular (2,0 - 2,7)**:
- Código funciona mas com problemas de organização
- Alguns padrões implementados parcialmente
- README incompleto
- Collection básica ou incompleta
- Poucos commits ou mal descritos

**Insuficiente (0 - 1,9)**:
- Código não compila ou não executa
- Padrões não implementados ou incorretos
- README ausente ou muito básico
- Collection ausente
- Commits inadequados

#### Documento Acadêmico (4,0 pontos)

**Excelente (3,5 - 4,0)**:
- Formatação ABNT impecável
- Fundamentação teórica sólida e bem referenciada
- Análise crítica e profunda dos padrões
- Excelente qualidade de escrita
- Mínimo 10 referências de qualidade
- Diagramas e figuras bem elaborados

**Bom (2,8 - 3,4)**:
- Formatação ABNT com pequenos erros
- Fundamentação teórica adequada
- Análise dos padrões boa mas superficial
- Boa qualidade de escrita
- 8-9 referências adequadas

**Regular (2,0 - 2,7)**:
- Formatação ABNT com vários erros
- Fundamentação teórica básica
- Análise superficial dos padrões
- Qualidade de escrita razoável
- 6-7 referências

**Insuficiente (0 - 1,9)**:
- Não segue ABNT
- Fundamentação teórica inadequada ou incorreta
- Sem análise dos padrões
- Escrita confusa ou com muitos erros
- Menos de 6 referências

#### Vídeo Demonstrativo (2,0 pontos)

**Excelente (1,8 - 2,0)**:
- Duração adequada (5-10 min)
- Apresentação clara e profissional
- Explica bem estrutura e código
- Demonstra todos os endpoints com sucesso
- Excelente qualidade de áudio e vídeo
- Bem organizado e didático

**Bom (1,4 - 1,7)**:
- Duração adequada
- Apresentação clara
- Explica estrutura adequadamente
- Demonstra maioria dos endpoints
- Boa qualidade técnica

**Regular (1,0 - 1,3)**:
- Duração fora do ideal
- Apresentação superficial
- Explica parcialmente
- Demonstra alguns endpoints
- Qualidade técnica razoável

**Insuficiente (0 - 0,9)**:
- Duração muito curta ou muito longa
- Apresentação confusa
- Não explica adequadamente
- Poucos endpoints demonstrados
- Qualidade técnica ruim

---

## ⚠️ Penalidades

### Atrasos
- **Até 24 horas**: -1,0 ponto
- **Até 48 horas**: -2,0 pontos
- **Após 48 horas**: Não será aceito (nota 0)

### Plágio
- **Plágio total**: Nota 0 (zero) + procedimentos disciplinares
- **Plágio parcial**: Nota proporcional ao conteúdo original (máximo 50%)
- **Colaboração excessiva**: Redução de até 50% da nota

**Atenção**: Todo código será verificado em ferramentas de detecção de plágio. Consultar colegas é permitido, mas copiar código é plágio.

### Entrega Incompleta
- Falta de 1 entregável: -3,0 pontos
- Falta de 2 entregáveis: -6,0 pontos
- Falta de todos os entregáveis: Nota 0

### Repositório Privado ou Inacessível
- Se o repositório não puder ser acessado: Nota 0 no entregável 1

### Vídeo Inacessível
- Se o vídeo não puder ser reproduzido: Nota 0 no entregável 3

---

## 💡 Dicas e Recomendações

### Para o Desenvolvimento

1. **Comece cedo**: Não deixe para última hora
2. **Faça commits frequentes**: Mostre sua evolução
3. **Teste constantemente**: Não espere finalizar tudo para testar
4. **Use o projeto de exemplo**: Analise o projeto `padroes-de-projeto-DB-Terca`
5. **Consulte a documentação oficial**: Microsoft Docs é seu amigo
6. **Peça ajuda se necessário**: Mas não copie código

### Para o Documento

1. **Use template ABNT**: Baixe template pronto do Word
2. **Comece pelo sumário**: Organize suas ideias
3. **Cite fontes confiáveis**: Documentação oficial, livros, artigos acadêmicos
4. **Revise ortografia**: Use corretor ortográfico
5. **Peça para alguém ler**: Feedback é importante
6. **Não deixe para última hora**: Documento leva tempo

### Para o Vídeo

1. **Faça um roteiro**: Planeje o que vai falar
2. **Teste a gravação**: Verifique áudio e vídeo antes
3. **Grave em ambiente silencioso**: Evite ruídos
4. **Seja objetivo**: Não fale demais de cada coisa
5. **Mostre funcionando**: Evidências > palavras
6. **Pratique antes**: Grave um teste primeiro

### Ferramentas Úteis

**Desenvolvimento**:
- Visual Studio Code ou Visual Studio 2022
- Postman
- DB Browser for SQLite
- Git + GitHub Desktop

**Documento**:
- Microsoft Word (com template ABNT)
- Mendeley ou Zotero (gerenciador de referências)
- Grammarly (correção ortográfica)

**Vídeo**:
- OBS Studio (gravação)
- DaVinci Resolve (edição)
- Audacity (edição de áudio)

---

## 📚 Recursos de Apoio

### Documentação Oficial

1. **ASP.NET Core**
   - https://docs.microsoft.com/aspnet/core

2. **Entity Framework Core**
   - https://docs.microsoft.com/ef/core

3. **FluentValidation**
   - https://docs.fluentvalidation.net

4. **Minimal APIs**
   - https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis

### Livros Recomendados

1. **Clean Architecture** - Robert C. Martin
2. **Design Patterns** - Gang of Four
3. **Patterns of Enterprise Application Architecture** - Martin Fowler
4. **Domain-Driven Design** - Eric Evans

### Artigos e Tutoriais

1. **Repository Pattern in .NET**
   - https://www.c-sharpcorner.com/article/repository-pattern-in-asp-net-core/

2. **Service Layer Pattern**
   - https://martinfowler.com/eaaCatalog/serviceLayer.html

3. **DTO Pattern**
   - https://martinfowler.com/eaaCatalog/dataTransferObject.html

### Vídeos (YouTube)

1. **Clean Architecture** - CodeOpinion
2. **Repository Pattern** - Nick Chapsas
3. **FluentValidation** - IAmTimCorey

### Projeto de Referência

- **Repositório de exemplo**: `padroes-de-projeto-DB-Terca`
- Analise a estrutura, os padrões e a organização
- **NÃO COPIE O CÓDIGO**: Use como inspiração

---

## ❓ Perguntas Frequentes (FAQ)

### Sobre o Desenvolvimento

**P: Posso usar outro banco de dados além do SQLite?**
R: Para entrega, deve ser SQLite. Mas você pode configurar para aceitar outros.

**P: Preciso implementar autenticação JWT?**
R: Não é obrigatório para esta AS. Foque nos padrões solicitados.

**P: Posso adicionar mais funcionalidades?**
R: Sim! Funcionalidades extras são bem-vindas e podem agregar pontos bônus.

**P: E se eu encontrar um bug após a entrega?**
R: Entregue o que estiver funcionando. Bugs menores terão desconto proporcional.

**P: Posso usar AutoMapper para os DTOs?**
R: Sim, mas não é obrigatório. Mapping manual é aceitável.

### Sobre o Documento

**P: Posso usar fontes além de Times New Roman?**
R: Sim, Arial tamanho 12 também é aceito pela ABNT.

**P: Preciso incluir código no documento?**
R: Sim, trechos de código ilustrativos são importantes. Use fonte Courier New.

**P: Quantas figuras/diagramas devo incluir?**
R: Mínimo 2 (arquitetura e modelo de dados). Mais é melhor.

**P: Posso usar citações de blogs?**
R: Prefira fontes acadêmicas, mas blogs técnicos reconhecidos são aceitáveis.

### Sobre o Vídeo

**P: Preciso aparecer no vídeo?**
R: Não é obrigatório. Gravação de tela com narração é suficiente.

**P: Posso gravar em inglês?**
R: Prefira português, mas inglês é aceitável se bem articulado.

**P: E se o vídeo passar de 10 minutos?**
R: Tente manter entre 5-10min. Até 12min é tolerável, mas seja objetivo.

**P: Preciso editar o vídeo?**
R: Não precisa ser profissional, mas cortes básicos são recomendados.

### Sobre a Entrega

**P: Posso entregar antes do prazo?**
R: Sim! Quanto antes, melhor.

**P: O que acontece se o GitHub ficar fora do ar?**
R: Use o GitLab como backup. Informe no arquivo TXT.

**P: Posso refazer após feedback?**
R: Não há reentrega. A nota será a da primeira submissão.

**P: E se eu esquecer de incluir algo no ZIP?**
R: Só será avaliado o que estiver no ZIP. Confira antes de enviar!

---

## 📞 Suporte e Dúvidas

### Canais de Comunicação

**Dúvidas Técnicas**:
- Fórum da disciplina no Blackboard/Moodle
- Horário de atendimento: Segunda a sexta, 14h-18h

**Dúvidas sobre o Documento**:
- Email: professor@instituicao.edu.br
- Resposta em até 48h úteis

**Dúvidas sobre Entrega**:
- Suporte técnico da plataforma

### Prazos de Resposta

- Dúvidas até 10/12: Resposta em até 48h
- Dúvidas após 10/12: Resposta em até 24h (melhor esforço)
- **Após 15/12**: Sem garantia de resposta antes da entrega

### Plantões de Dúvidas (Presencial/Online)

- **05/12/2025** - 14h-16h (Online via Teams)
- **10/12/2025** - 14h-16h (Presencial - Lab 3)
- **12/12/2025** - 14h-16h (Online via Teams)

---

## 🎯 Considerações Finais

Esta avaliação semestral foi projetada para consolidar todo o aprendizado da disciplina de Desenvolvimento Backend. Ela simula um cenário real de desenvolvimento de software, onde você precisará:

- Aplicar conceitos teóricos em código funcional
- Documentar suas decisões técnicas de forma acadêmica
- Comunicar soluções técnicas de forma clara

**Sucesso não é sorte, é preparação + oportunidade!**

Organize seu tempo, planeje suas entregas e execute com qualidade. Boa sorte! 🚀

---

## 📋 Checklist Final de Entrega

Imprima e marque conforme completar:

### Desenvolvimento
- [ ] Projeto criado e estruturado
- [ ] Entidade Usuario implementada
- [ ] Repository Pattern implementado
- [ ] Service Pattern implementado
- [ ] DTOs criados (Create, Read, Update)
- [ ] FluentValidation configurado
- [ ] Validadores implementados
- [ ] Endpoints implementados (GET, POST, PUT, DELETE)
- [ ] Migrations criadas e aplicadas
- [ ] Swagger configurado
- [ ] Código testado e funcionando
- [ ] README.md completo
- [ ] Collection Postman criada
- [ ] Commits frequentes e descritivos
- [ ] Repositório público no GitHub

### Documento
- [ ] Capa conforme modelo
- [ ] Resumo (150-250 palavras)
- [ ] Sumário
- [ ] Introdução
- [ ] Fundamentação teórica completa
- [ ] Desenvolvimento detalhado
- [ ] Resultados apresentados
- [ ] Conclusão
- [ ] Mínimo 8 referências ABNT
- [ ] Formatação ABNT correta
- [ ] 10-15 páginas
- [ ] Ortografia revisada
- [ ] Salvo em PDF

### Vídeo
- [ ] Roteiro preparado
- [ ] Apresentação pessoal
- [ ] Estrutura do projeto mostrada
- [ ] Código explicado
- [ ] Endpoints testados
- [ ] Todos os requisitos demonstrados
- [ ] Duração entre 5-10 minutos
- [ ] Áudio claro e audível
- [ ] Vídeo em boa resolução
- [ ] Upload concluído
- [ ] Link público e funcional

### Entrega
- [ ] Arquivo ZIP criado
- [ ] Nome do ZIP correto
- [ ] Link do repositório no TXT
- [ ] Documento PDF incluído
- [ ] Link do vídeo no TXT
- [ ] Links testados e públicos
- [ ] Upload na plataforma
- [ ] Confirmação de entrega recebida

---

**Data de Publicação**: 18/11/2025  
**Versão**: 1.0  
**Professor**: [Nome do Professor]  
**Disciplina**: Desenvolvimento Backend  
**Instituição**: [Nome da Instituição]

---

## 📄 Anexos

### Anexo A - Template de Commit Messages

```
feat: adicionar [funcionalidade]
fix: corrigir [bug]
docs: atualizar [documentação]
refactor: refatorar [código]
test: adicionar [teste]
chore: atualizar [configuração]

Exemplos:
✅ feat: implementar Repository Pattern
✅ feat: adicionar validação de email único
✅ fix: corrigir erro de validação de idade
✅ docs: atualizar README com instruções
✅ refactor: melhorar organização do Service
```

### Anexo B - Exemplo de Estrutura de Testes no Postman

```json
{
  "info": {
    "name": "API Usuários - AS",
    "description": "Collection completa da API de Usuários"
  },
  "item": [
    {
      "name": "Criar Usuário - Válido",
      "request": {
        "method": "POST",
        "url": "{{base_url}}/usuarios",
        "body": {
          "nome": "João Silva",
          "email": "joao@email.com",
          "senha": "senha123",
          "dataNascimento": "1995-01-15",
          "telefone": "(11) 98765-4321"
        }
      }
    }
  ]
}
```

### Anexo C - Exemplo de Resposta de Erro Padronizada

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": [
      "O nome deve ter entre 3 e 100 caracteres."
    ],
    "Email": [
      "O email não está em um formato válido."
    ]
  }
}
```

---

**BOA SORTE E BOM TRABALHO! 🎓💻**
