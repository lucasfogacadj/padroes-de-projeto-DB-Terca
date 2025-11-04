# 🎓 GUIA RÁPIDO DO PROFESSOR - Aula PUT/PATCH & Exception Handling

## ⏱️ Timing Sugerido (Aula de 2-3 horas)

### PARTE 1: Introdução (15 min)
- ✅ Apresentar agenda
- ✅ Revisar conceitos de REST
- ✅ Explicar objetivos da aula

### PARTE 2: PUT vs PATCH - Teoria (30 min)
- ✅ Slides 1-17 (docs/Aula_PUT_PATCH_ExceptionHandling.md)
- ✅ Enfatizar diferenças práticas
- ✅ Quando usar cada um

### PARTE 3: PUT vs PATCH - Prática (45 min)
- ✅ Mostrar código implementado
- ✅ Executar testes do arquivo .http
- ✅ Demonstração ao vivo

### PARTE 4: Exception Handling - Teoria (30 min)
- ✅ Slides 18-35
- ✅ Problem Details (RFC 7807)
- ✅ Middleware pipeline

### PARTE 5: Exception Handling - Prática (30 min)
- ✅ Mostrar Global Exception Handler
- ✅ Testar cenários de erro
- ✅ Ver logs estruturados

### PARTE 6: Q&A e Exercícios (30 min)
- ✅ Responder dúvidas
- ✅ Passar exercícios práticos
- ✅ Próximos passos

---

## 🎯 PONTOS-CHAVE A ENFATIZAR

### PUT vs PATCH
1. **PUT = Substituição Total**
   - "Se você atualiza perfil do Facebook, envia TUDO de novo"
   - Todos campos obrigatórios
   - Idempotente

2. **PATCH = Mudança Parcial**
   - "Curtir um post = só incrementa contador"
   - Apenas campos que mudam
   - Economia de banda

3. **Idempotência é CRÍTICA**
   - Retry automático seguro
   - Mesmo resultado N vezes

### Exception Handling
1. **Exceções de Negócio vs Técnicas**
   - 4xx = cliente errou (esperado)
   - 5xx = servidor errou (bug)

2. **DRY Principle**
   - try-catch em TODO endpoint = duplicação
   - Middleware centralizado = manutenível

3. **Problem Details é Padrão**
   - RFC 7807 da IETF
   - Usado por Google, Microsoft, etc.

4. **Logging Estruturado**
   - TraceId para correlação
   - Níveis apropriados (Error vs Warning)

---

## 💻 DEMONSTRAÇÕES AO VIVO

### Demo 1: PUT com Sucesso
```bash
# Abrir APIProdutos_Completo.http
# Executar teste #4 (PUT completo)
# Mostrar resposta 200 OK
# Verificar no GET que TUDO mudou
```

### Demo 2: PUT sem Campo (ERRO)
```bash
# Executar teste #5 (PUT sem descrição)
# Mostrar erro 400 Bad Request
# Explicar: "PUT exige TODOS os campos!"
```

### Demo 3: PATCH Apenas Preço (SUCESSO)
```bash
# Executar teste #8 (PATCH só preço)
# Mostrar resposta 200 OK
# Verificar no GET que SÓ preço mudou
# Enfatizar: "Outros campos inalterados!"
```

### Demo 4: Produto Não Encontrado (404)
```bash
# Executar teste #15 (GET produto 999)
# Mostrar Problem Details:
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Produto com ID '999' não foi encontrado.",
  "traceId": "..."
}
# Explicar cada campo do Problem Details
```

### Demo 5: Erro de Validação (400)
```bash
# Executar teste #18 (POST preço negativo)
# Mostrar erro com campo "errors":
{
  "status": 400,
  "errors": {
    "preco": ["O preço deve ser maior que zero."]
  }
}
# Enfatizar: "Cliente sabe EXATAMENTE o que corrigir"
```

### Demo 6: Logging no Console
```bash
# Forçar erro 500 (comentar linha no código temporariamente)
# Mostrar log estruturado no console:
# "warn: ... Erro ao processar requisição GET /produtos/1..."
# Enfatizar TraceId para correlação
```

---

## 🗣️ FRASES DE EFEITO PARA USAR

### PUT vs PATCH
- "PUT é como reformar casa: derruba TUDO e reconstrói"
- "PATCH é como pintar parede: só mexe no que precisa"
- "Idempotência salva vidas em redes instáveis!"
- "Mobile apps AMAM PATCH (economiza 3G/4G)"

### Exception Handling
- "try-catch em todo endpoint? Isso é copy-paste programming!"
- "Middleware é seu guarda-costas: protege TODOS os endpoints"
- "Stack trace em produção = presente de Natal para hackers"
- "TraceId é seu melhor amigo no debug de produção"
- "4xx = cliente burro, 5xx = eu burro 😅"

---

## 📝 EXERCÍCIOS PRÁTICOS (PARA PASSAR NO FINAL)

### Exercício 1: Implementar PUT/PATCH em Casa (Individual)
**Tarefa:** 
- Clonar repositório
- Testar todos os 38 cenários do arquivo .http
- Documentar 3 diferenças práticas entre PUT e PATCH

**Entrega:** Screenshot dos testes + texto explicativo

---

### Exercício 2: Criar Nova Exceção (Dupla)
**Tarefa:**
- Criar `InsufficientStockException` (422 Unprocessable Entity)
- Lançar no método `RemoverEstoque(int quantidade)`
- Testar cenário: tentar remover mais estoque do que disponível

**Entrega:** Código + teste HTTP

---

### Exercício 3: Melhorar Validação (Grupo)
**Tarefa:**
- Modificar `CriarAsync` para acumular TODOS erros de validação
- Retornar `ValidationException` com múltiplos erros
- Exemplo: nome vazio + preço negativo + estoque negativo → 1 resposta com 3 erros

**Entrega:** PR com código modificado

---

### Exercício 4: Logging Avançado (Individual - Desafio)
**Tarefa:**
- Instalar Serilog via NuGet
- Configurar logging em arquivo
- Adicionar log de auditoria: quem/quando atualizou produto

**Entrega:** Arquivo de log + código

---

## 🐛 POSSÍVEIS PROBLEMAS E SOLUÇÕES

### Problema 1: Middleware não captura exceção
**Causa:** Middleware registrado DEPOIS de outros  
**Solução:** `app.UseGlobalExceptionHandler()` deve ser PRIMEIRO  
**Demo:** Mover linha e mostrar que para de funcionar

### Problema 2: EF Core não salva mudanças do PATCH
**Causa:** Entidade não rastreada (AsNoTracking)  
**Solução:** `GetByIdAsync` deve usar tracking  
**Demo:** Remover `AsNoTracking` do método

### Problema 3: PUT aceita objeto vazio
**Causa:** DataAnnotations não validadas  
**Solução:** Adicionar `builder.Services.AddControllers().AddDataAnnotations()`  
**Demo:** Mostrar antes/depois

### Problema 4: 500 mostra stack trace em "produção"
**Causa:** `IsDevelopment()` retorna true  
**Solução:** Mudar `ASPNETCORE_ENVIRONMENT` para "Production"  
**Demo:** `$env:ASPNETCORE_ENVIRONMENT="Production"; dotnet run`

---

## 🎤 PERGUNTAS ESPERADAS DOS ALUNOS

### P1: "Por que não usar AutoMapper?"
**R:** 
- Para aprendizado, mapping manual é mais claro
- AutoMapper adiciona "mágica" que esconde conceitos
- Profissionalmente, sim, AutoMapper é válido
- **Adicionar na próxima aula se quiserem**

### P2: "E se eu quiser 'limpar' um campo no PATCH (tornar null)?"
**R:**
- Boa pergunta! Ambiguidade: `null` = ignorar ou limpar?
- Opção 1: Convenção (null = ignorar sempre)
- Opção 2: Valor especial ("__DELETE__")
- Opção 3: JSON Patch (RFC 6902) - avançado
- **Nossa implementação: null = ignorar**

### P3: "Por que não retornar 204 No Content no PUT/PATCH?"
**R:**
- 200 + corpo → Cliente tem produto atualizado (1 request)
- 204 sem corpo → Cliente precisa fazer GET depois (2 requests)
- **Trade-off: banda vs round-trips**
- Ambos corretos, depende do caso

### P4: "E se dois usuários editarem ao mesmo tempo?"
**R:**
- Ótima pergunta! Problema de concorrência
- Solução: Optimistic Locking (RowVersion/ETag)
- **Próxima aula ou tópico avançado**
- Por ora, "último vence" (last-write-wins)

### P5: "Middleware não é 'overengineering' para projeto pequeno?"
**R:**
- Projeto cresce rápido! Planejar para escala
- Remover 50 try-catch depois é trabalhoso
- **Fazer certo desde o início = menos refactor**
- Mas sim, em API de 2 endpoints, discutível

---

## 📊 MÉTRICAS DE SUCESSO DA AULA

Ao final, alunos devem conseguir:

- [ ] Explicar diferença entre PUT e PATCH
- [ ] Decidir qual usar em cenário real
- [ ] Criar exceção customizada
- [ ] Entender fluxo do middleware
- [ ] Ler/interpretar Problem Details
- [ ] Testar endpoints com arquivo .http
- [ ] Explicar idempotência
- [ ] Diferenciar erro 4xx de 5xx

---

## 🎁 MATERIAL EXTRA (SE DER TEMPO)

### Tópico Bonus 1: JSON Patch (RFC 6902)
```http
PATCH /produtos/1
Content-Type: application/json-patch+json

[
  { "op": "replace", "path": "/preco", "value": 100 },
  { "op": "add", "path": "/tags/-", "value": "promocao" }
]
```

### Tópico Bonus 2: ETag para Cache/Concorrência
```http
GET /produtos/1
→ ETag: "v1-abc123"

PUT /produtos/1
If-Match: "v1-abc123"  ← Valida versão
→ 200 OK (se match) ou 412 Precondition Failed
```

### Tópico Bonus 3: Rate Limiting
```csharp
builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("fixed", opt => {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
    });
});
```

---

## 📸 SCREENSHOTS ÚTEIS PARA SLIDES

1. **Swagger/OpenAPI** mostrando endpoints PUT/PATCH
2. **Postman/REST Client** com teste bem-sucedido
3. **Postman/REST Client** com erro 404 formatado
4. **VS Code** mostrando estrutura de pastas
5. **Console** com logs estruturados
6. **Comparação lado-a-lado**: PUT (todos campos) vs PATCH (1 campo)

---

## 🚀 PRÓXIMA AULA - PREVIEW

No final, mencionar:

> "Próxima aula: **FluentValidation + Testes Unitários**
> - Validações complexas e reutilizáveis
> - Testar Service sem banco de dados (mocks)
> - xUnit + Moq
> - Code coverage
> 
> **Pré-requisito**: Terminar exercícios de hoje!"

---

## ✅ CHECKLIST PRÉ-AULA

- [ ] API compilando sem erros (`dotnet build`)
- [ ] Banco de dados criado (`dotnet ef database update`)
- [ ] Arquivo .http testado (pelo menos testes 1-14)
- [ ] Slides revisados
- [ ] VS Code com extensão REST Client instalada
- [ ] Postman como backup (se REST Client falhar)
- [ ] Exemplos de código prontos para copiar/colar
- [ ] Terminal limpo e legível (fonte grande)
- [ ] Git status limpo (commitar antes da aula)

---

## 📞 CONTATOS ÚTEIS

- **Repositório:** github.com/lucasfogacadj/padroes-de-projeto-DB-Terca
- **Dúvidas:** criar Issue no GitHub ou email
- **Office Hours:** Terças 14h-16h

---

**Boa aula, professor! 🎉**

*Lembre-se: Se alunos saírem sabendo QUANDO usar PUT vs PATCH e POR QUÊ centralizar exceções, missão cumprida!*
