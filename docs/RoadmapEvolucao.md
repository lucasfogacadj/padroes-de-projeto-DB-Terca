# 🗺️ Roadmap de Evolução - API de Produtos

## 📚 Visão Geral
Este roadmap está organizado em **fases progressivas**, cada uma introduzindo novos conceitos de desenvolvimento backend. As fases foram pensadas considerando:
- Complexidade crescente
- Dependências entre conceitos
- Aplicabilidade no mercado
- Valor didático para alunos

---

## 🎯 **FASE 1: Fundação Sólida (Atual → +2 semanas)**

### Status Atual ✅
- ✅ Clean Architecture básica (Domain, Application, Infrastructure)
- ✅ Repository Pattern
- ✅ Service Pattern
- ✅ Factory Pattern (básico)
- ✅ DTOs (parcial)
- ✅ Entity Framework Core + SQLite
- ✅ Minimal APIs

### A Completar 🔧

#### 1.1 Finalizar FluentValidation
**Objetivo**: Separar validações de negócio do serviço

**Implementação**:
```bash
dotnet add package FluentValidation.AspNetCore
```

**Tarefas**:
- [ ] Criar `Application/Validators/ProdutoCreateDtoValidator.cs`
- [ ] Criar `Application/Validators/ProdutoUpdateDtoValidator.cs`
- [ ] Integrar validação automática nos endpoints
- [ ] Documentar diferença entre validação de entrada vs invariantes de domínio

**Aprendizados**: Separação de responsabilidades, validação declarativa

---

#### 1.2 Melhorar Tratamento de Erros
**Objetivo**: Padronizar respostas de erro (RFC 7807 - Problem Details)

**Tarefas**:
- [ ] Criar `Application/Exceptions/` com exceções customizadas:
  - `ProdutoNaoEncontradoException`
  - `DomainException` (base)
  - `ValidationException`
- [ ] Implementar middleware de erro global
- [ ] Retornar Problem Details consistentes
- [ ] Adicionar correlation ID para rastreamento

**Aprendizados**: Exception handling, middlewares, API contracts

---

#### 1.3 Completar Camada de DTOs
**Objetivo**: Desacoplar totalmente entidades do contrato HTTP

**Tarefas**:
- [ ] Criar `ProdutoUpdateDto.cs`
- [ ] Criar `ProdutoPatchDto.cs` (para PATCH)
- [ ] Implementar AutoMapper (opcional) ou melhorar mapping manual
- [ ] Remover retorno de entidades `Produto` direto nos endpoints
- [ ] Usar apenas DTOs em todas as respostas

**Aprendizados**: API contract design, versionamento futuro

---

#### 1.4 Testes Unitários Básicos
**Objetivo**: Introduzir cultura de testes

**Implementação**:
```bash
dotnet new xunit -n APIProdutos.Tests
dotnet add APIProdutos.Tests package Moq
dotnet add APIProdutos.Tests package FluentAssertions
```

**Tarefas**:
- [ ] Testar `ProdutoService` (mocking repository)
- [ ] Testar `ProdutoFactory`
- [ ] Testar validadores FluentValidation
- [ ] Cobertura mínima: 70%

**Aprendizados**: TDD, mocking, arrange-act-assert

---

## 🚀 **FASE 2: Profissionalização (Semanas 3-6)**

### 2.1 Logging Estruturado
**Objetivo**: Rastreabilidade e observabilidade

**Implementação**:
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

**Tarefas**:
- [ ] Configurar Serilog com enrichers (timestamp, correlation ID)
- [ ] Adicionar logs estruturados em pontos críticos:
  - Entrada/saída de serviços
  - Exceções
  - Operações de DB
- [ ] Criar política de log levels (Debug, Info, Warning, Error)
- [ ] Exportar logs para arquivo JSON

**Aprendizados**: Observabilidade, debugging em produção

---

### 2.2 Paginação e Filtros
**Objetivo**: Preparar API para grandes volumes de dados

**Tarefas**:
- [ ] Criar `Application/DTOs/PaginatedResult<T>.cs`
- [ ] Implementar query parameters: `?page=1&pageSize=10`
- [ ] Adicionar filtros: `?nome=xxx&precoMin=10&precoMax=100`
- [ ] Implementar ordenação: `?orderBy=preco&orderDirection=desc`
- [ ] Usar LINQ com `Skip()` e `Take()`
- [ ] Retornar metadados de paginação (total de itens, páginas)

**Aprendizados**: Query optimization, API design, performance

---

### 2.3 Cache com Decorator Pattern
**Objetivo**: Melhorar performance sem mudar serviços existentes

**Implementação**:
```bash
dotnet add package Microsoft.Extensions.Caching.Memory
```

**Tarefas**:
- [ ] Criar `Application/Services/ProdutoServiceCacheDecorator.cs`
- [ ] Implementar cache em memória para listagens
- [ ] Definir TTL (Time To Live) apropriado
- [ ] Invalidar cache em operações de escrita
- [ ] Adicionar métricas de hit/miss ratio

**Aprendizados**: Decorator Pattern, caching strategies, performance

---

### 2.4 Documentação OpenAPI/Swagger Avançada
**Objetivo**: Documentação interativa e completa

**Tarefas**:
- [ ] Adicionar XML comments nos endpoints
- [ ] Configurar Swagger com exemplos de requisição/resposta
- [ ] Documentar todos os status codes possíveis
- [ ] Adicionar autenticação ao Swagger UI (preparação para Fase 3)
- [ ] Versionar API (v1, v2)

**Aprendizados**: API documentation, contract-first design

---

### 2.5 Testes de Integração
**Objetivo**: Testar fluxo completo da API

**Implementação**:
```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Testcontainers
```

**Tarefas**:
- [ ] Criar `APIProdutos.IntegrationTests`
- [ ] Usar WebApplicationFactory para testar endpoints
- [ ] Testar com banco de dados real (Testcontainers)
- [ ] Validar status codes, headers, body
- [ ] Testar cenários de erro

**Aprendizados**: Integration testing, test containers, E2E testing

---

## 🔒 **FASE 3: Segurança e Autenticação (Semanas 7-9)**

### 3.1 Autenticação JWT
**Objetivo**: Proteger a API

**Implementação**:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Tarefas**:
- [ ] Criar endpoint `/auth/login`
- [ ] Implementar geração de JWT tokens
- [ ] Adicionar claims (userId, roles)
- [ ] Proteger endpoints com `[Authorize]`
- [ ] Implementar refresh tokens
- [ ] Configurar expiration e validação

**Aprendizados**: JWT, authentication, authorization

---

### 3.2 Autorização Baseada em Roles
**Objetivo**: Controle de acesso granular

**Tarefas**:
- [ ] Criar entidade `Usuario` e `Role`
- [ ] Implementar roles: Admin, Gerente, Vendedor
- [ ] Proteger endpoints por role (ex: DELETE só Admin)
- [ ] Implementar políticas de autorização customizadas
- [ ] Adicionar audit trail (quem criou/modificou)

**Aprendizados**: RBAC, authorization policies, audit

---

### 3.3 Rate Limiting
**Objetivo**: Proteger contra abuso

**Implementação**:
```bash
dotnet add package AspNetCoreRateLimit
```

**Tarefas**:
- [ ] Configurar limite por IP: 100 req/min
- [ ] Configurar limite por usuário autenticado
- [ ] Retornar 429 Too Many Requests
- [ ] Adicionar headers: X-RateLimit-Remaining

**Aprendizados**: API security, throttling, DoS protection

---

## 📊 **FASE 4: Arquitetura Avançada (Semanas 10-13)**

### 4.1 CQRS (Command Query Responsibility Segregation)
**Objetivo**: Separar leituras de escritas

**Implementação**:
```bash
dotnet add package MediatR
```

**Tarefas**:
- [ ] Criar `Application/Commands/` e `Application/Queries/`
- [ ] Implementar handlers com MediatR
- [ ] Separar modelos de leitura (QueryModels) de escrita (Commands)
- [ ] Otimizar queries com projeções (Select apenas campos necessários)
- [ ] Comparar performance antes/depois

**Aprendizados**: CQRS, MediatR, query optimization

---

### 4.2 Domain Events
**Objetivo**: Desacoplar ações relacionadas

**Tarefas**:
- [ ] Criar `Domain/Events/ProdutoCriadoEvent.cs`
- [ ] Implementar event dispatcher
- [ ] Criar handlers:
  - Enviar email quando produto criado
  - Atualizar cache
  - Registrar auditoria
- [ ] Usar padrão Observer

**Aprendizados**: Event-driven architecture, domain events, loose coupling

---

### 4.3 Specification Pattern
**Objetivo**: Filtros complexos e reutilizáveis

**Tarefas**:
- [ ] Criar `Domain/Specifications/ProdutoSpec.cs`
- [ ] Implementar specs combináveis:
  - `ProdutoComPrecoEntreSpec`
  - `ProdutoEmEstoqueSpec`
  - `ProdutoAtivoSpec`
- [ ] Usar com Repository: `GetAsync(ISpecification<Produto>)`
- [ ] Combinar specs com operadores AND/OR

**Aprendizados**: Specification pattern, query composition, DDD

---

### 4.4 Background Jobs
**Objetivo**: Processar tarefas assíncronas

**Implementação**:
```bash
dotnet add package Hangfire
```

**Tarefas**:
- [ ] Configurar Hangfire Dashboard
- [ ] Criar job: Atualizar estoque de produtos
- [ ] Criar job recorrente: Relatório diário
- [ ] Implementar retry policy
- [ ] Monitorar jobs no dashboard

**Aprendizados**: Background processing, job scheduling, async tasks

---

## 🌐 **FASE 5: Microsserviços e Distribuição (Semanas 14-17)**

### 5.1 API Gateway
**Objetivo**: Ponto único de entrada

**Implementação**:
```bash
dotnet new webapi -n API.Gateway
dotnet add package Ocelot
```

**Tarefas**:
- [ ] Configurar Ocelot para rotear requisições
- [ ] Separar API Produtos em microsserviço
- [ ] Criar microsserviço de Pedidos (novo)
- [ ] Implementar service discovery
- [ ] Configurar load balancing

**Aprendizados**: Microservices, API Gateway, service discovery

---

### 5.2 Mensageria com RabbitMQ
**Objetivo**: Comunicação assíncrona entre serviços

**Implementação**:
```bash
dotnet add package RabbitMQ.Client
dotnet add package MassTransit
```

**Tarefas**:
- [ ] Configurar RabbitMQ (Docker)
- [ ] Publicar evento: `ProdutoEstoqueAtualizadoEvent`
- [ ] Consumir em serviço de Notificações
- [ ] Implementar retry e dead letter queue
- [ ] Garantir idempotência de consumidores

**Aprendizados**: Message queues, eventual consistency, async messaging

---

### 5.3 Distributed Tracing
**Objetivo**: Rastrear requisições entre serviços

**Implementação**:
```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
```

**Tarefas**:
- [ ] Configurar OpenTelemetry
- [ ] Integrar com Jaeger (Docker)
- [ ] Propagar trace context entre serviços
- [ ] Visualizar latência e dependências

**Aprendizados**: Distributed tracing, observability, debugging microservices

---

## 📦 **FASE 6: DevOps e Deploy (Semanas 18-20)**

### 6.1 Containerização com Docker
**Tarefas**:
- [ ] Criar `Dockerfile` multi-stage
- [ ] Criar `docker-compose.yml` (API + DB + RabbitMQ)
- [ ] Otimizar imagem (Alpine, layer caching)
- [ ] Configurar health checks
- [ ] Publicar no Docker Hub

**Aprendizados**: Docker, containerization, infrastructure as code

---

### 6.2 CI/CD com GitHub Actions
**Tarefas**:
- [ ] Criar workflow de build e testes
- [ ] Configurar análise de código (SonarCloud)
- [ ] Deploy automático para Azure/AWS
- [ ] Configurar ambientes (dev, staging, prod)
- [ ] Implementar blue-green deployment

**Aprendizados**: CI/CD, automation, deployment strategies

---

### 6.3 Kubernetes Básico
**Tarefas**:
- [ ] Criar deployment.yaml
- [ ] Configurar service (ClusterIP, LoadBalancer)
- [ ] Implementar ConfigMaps e Secrets
- [ ] Configurar horizontal pod autoscaling
- [ ] Deploy em cluster local (minikube/kind)

**Aprendizados**: Kubernetes, orchestration, scalability

---

## 🎓 **FASE 7: Tópicos Avançados (Opcional)**

### 7.1 GraphQL
- Alternativa ao REST
- Queries flexíveis
- HotChocolate

### 7.2 gRPC
- Comunicação performática
- Protocol Buffers
- Streaming bidirecional

### 7.3 Event Sourcing
- Armazenar eventos ao invés de estado
- Replay de eventos
- Auditoria completa

### 7.4 Elasticsearch
- Busca full-text
- Agregações complexas
- Log analytics

---

## 📋 Checklist de Avaliação por Fase

### Critérios Gerais
- [ ] Código compila sem warnings
- [ ] Testes passam (cobertura > 70%)
- [ ] Documentação atualizada (README + XML comments)
- [ ] PR com justificativa técnica
- [ ] Code review aprovado
- [ ] Demonstração funcional

### Bônus
- [ ] Benchmarks comparativos
- [ ] Discussão crítica: quando NÃO usar?
- [ ] Apresentação para a turma
- [ ] Artigo técnico no Medium/Dev.to

---

## 🎯 Objetivos de Aprendizado por Fase

| Fase | Foco Principal | Habilidades |
|------|----------------|-------------|
| 1 | Fundação | Clean Code, SOLID, testes |
| 2 | Profissionalização | Performance, observabilidade, API design |
| 3 | Segurança | Autenticação, autorização, proteção |
| 4 | Arquitetura | CQRS, eventos, patterns avançados |
| 5 | Distribuição | Microservices, mensageria, tracing |
| 6 | DevOps | Docker, CI/CD, Kubernetes |
| 7 | Especialização | Tecnologias de nicho |

---

## 🔄 Ciclo de Desenvolvimento por Funcionalidade

1. **Planejamento** (1 dia)
   - Definir escopo
   - Criar branch `feature/nome`
   - Abrir PR em draft

2. **Implementação** (3-5 dias)
   - TDD: Red → Green → Refactor
   - Commits atômicos
   - Code review contínuo

3. **Documentação** (1 dia)
   - README atualizado
   - XML comments
   - Exemplos de uso

4. **Review e Merge** (1 dia)
   - Aprovação de 2 revisores
   - CI verde
   - Merge para main

---

## 📚 Recursos Recomendados

### Livros
- **Clean Architecture** - Robert C. Martin
- **Domain-Driven Design** - Eric Evans
- **Microservices Patterns** - Chris Richardson
- **Building Microservices** - Sam Newman

### Cursos
- Microsoft Learn: ASP.NET Core
- Pluralsight: Microservices Architecture
- Udemy: Docker & Kubernetes

### Comunidades
- Stack Overflow
- Dev.to
- Reddit: r/dotnet, r/csharp
- Discord: .NET Community

---

## 💡 Dicas Finais

1. **Não pule fases** - Cada uma constrói sobre a anterior
2. **Foque na qualidade, não quantidade** - Melhor dominar bem uma fase
3. **Documente decisões** - Futuramente você agradecerá
4. **Peça code review** - Aprenda com feedbacks
5. **Refatore constantemente** - O código evolui, suas habilidades também
6. **Apresente para colegas** - Ensinar é a melhor forma de aprender
7. **Celebre conquistas** - Cada fase completada é uma vitória!

---

## 🎓 Próximos Passos Imediatos

1. Revisar este roadmap com a turma
2. Definir cronograma (sprints de 2 semanas)
3. Reorganizar grupos por fase
4. Iniciar Fase 1.1 (FluentValidation)
5. Agendar sessões de pair programming
6. Criar board no GitHub Projects para tracking

**Boa evolução! 🚀**
