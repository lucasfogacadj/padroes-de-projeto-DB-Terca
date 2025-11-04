# 🎯 COMPARAÇÃO VISUAL: PUT vs PATCH

## Cenário: Atualizar Produto

### 📦 Produto Original no Banco de Dados
```json
{
  "id": 1,
  "nome": "Notebook Dell",
  "descricao": "Intel i5, 8GB RAM, SSD 256GB",
  "preco": 3500.00,
  "estoque": 10,
  "dataCriacao": "2025-01-15T10:30:00Z"
}
```

---

## 🔄 OPÇÃO 1: PUT - Substituição Total

### Request:
```http
PUT /produtos/1
Content-Type: application/json

{
  "nome": "Notebook Dell XPS 15",
  "descricao": "Intel i7 11ª geração, 16GB RAM, SSD 512GB, Tela 4K",
  "preco": 5500.00,
  "estoque": 8
}
```

### ✅ Response: 200 OK
```json
{
  "id": 1,
  "nome": "Notebook Dell XPS 15",          ← MUDOU
  "descricao": "Intel i7 11ª...",           ← MUDOU
  "preco": 5500.00,                         ← MUDOU
  "estoque": 8,                             ← MUDOU
  "dataCriacao": "2025-01-15T10:30:00Z"     ← MANTEVE (imutável)
}
```

### 📝 Características:
✅ Todos os campos foram substituídos  
✅ Precisa enviar **TODOS** os campos obrigatórios  
✅ Se omitir campo → Erro 400 Bad Request  
✅ Idempotente (executar N vezes = mesmo resultado)

---

## 🔧 OPÇÃO 2: PATCH - Atualização Parcial

### Request:
```http
PATCH /produtos/1
Content-Type: application/json

{
  "preco": 5200.00
}
```

### ✅ Response: 200 OK
```json
{
  "id": 1,
  "nome": "Notebook Dell XPS 15",          ← MANTEVE
  "descricao": "Intel i7 11ª...",          ← MANTEVE
  "preco": 5200.00,                        ← MUDOU (único campo enviado!)
  "estoque": 8,                            ← MANTEVE
  "dataCriacao": "2025-01-15T10:30:00Z"    ← MANTEVE
}
```

### 📝 Características:
✅ Apenas `preco` foi atualizado  
✅ Outros campos **permaneceram inalterados**  
✅ Campos não enviados são **ignorados**  
✅ Economia de banda (envia só o necessário)

---

## ⚖️ COMPARAÇÃO LADO A LADO

| Aspecto | PUT | PATCH |
|---------|-----|-------|
| **O que atualiza?** | TUDO (substituição total) | APENAS campos enviados |
| **Campos obrigatórios?** | ✅ Sim, todos | ❌ Não, todos opcionais |
| **Omitir campo?** | ❌ Erro 400 | ✅ Campo mantém valor atual |
| **Tamanho do payload** | Grande (todos campos) | Pequeno (só o que muda) |
| **Idempotência** | ✅ Sim | ✅ Sim (valores absolutos) |
| **Uso típico** | Reformular recurso | Ajuste pontual |
| **Exemplo prático** | Editar perfil completo | Curtir post, alterar preço |

---

## 🎯 QUANDO USAR CADA UM?

### Use PUT quando:
```
✅ Cliente tem TODOS os dados do recurso
✅ Interface de "edição completa" (formulário com todos campos)
✅ Substituição total faz sentido no negócio
✅ Modelo de dados é simples e estável

Exemplos reais:
- Editar perfil de usuário (formulário completo)
- Substituir configuração inteira
- Upload de arquivo (substitui completamente)
```

### Use PATCH quando:
```
✅ Atualização frequente de campos específicos
✅ Recurso tem muitos campos
✅ Economia de banda é importante (mobile)
✅ Evitar conflitos de atualização concorrente

Exemplos reais:
- Alterar preço de produto (e-commerce)
- Marcar notificação como lida (só 1 flag)
- Incrementar contador de views
- Atualizar status de tarefa
```

---

## 🚫 ERRO COMUM: PUT SEM TODOS OS CAMPOS

### ❌ Request Incorreto:
```http
PUT /produtos/1
Content-Type: application/json

{
  "preco": 5500.00
}
```

### ❌ Response: 400 Bad Request
```json
{
  "status": 400,
  "title": "Erro de validação",
  "detail": "Um ou mais erros de validação ocorreram.",
  "errors": {
    "nome": ["O nome do produto é obrigatório."],
    "descricao": ["A descrição do produto é obrigatória."],
    "estoque": ["O estoque do produto é obrigatório."]
  },
  "traceId": "0HN1HKP8ASQQ4:00000001"
}
```

### 💡 Solução:
**Use PATCH** se quer atualizar só o preço!

---

## 🔁 IDEMPOTÊNCIA - CONCEITO CRÍTICO

### ✅ PUT é Idempotente:
```http
# Executar 3 vezes:
PUT /produtos/1 { "nome": "X", "preco": 100, "descricao": "Y", "estoque": 5 }

Execução 1: preco = 100
Execução 2: preco = 100  ← Mesmo resultado!
Execução 3: preco = 100  ← Seguro para retry!
```

### ✅ PATCH também (com valores absolutos):
```http
# Executar 3 vezes:
PATCH /produtos/1 { "preco": 100 }

Execução 1: preco = 100
Execução 2: preco = 100  ← Idempotente!
Execução 3: preco = 100  ← Seguro!
```

### ❌ PATCH com operações relativas (NÃO IDEMPOTENTE):
```http
# ❌ MAL IMPLEMENTADO:
PATCH /produtos/1 { "estoque": "+5" }

Execução 1: estoque = 10 + 5 = 15
Execução 2: estoque = 15 + 5 = 20  ← Diferente!
Execução 3: estoque = 20 + 5 = 25  ← Problema!
```

**Conclusão:** Sempre use valores absolutos, não operações!

---

## 📊 CENÁRIO PRÁTICO: E-COMMERCE

### Caso 1: Administrador Editando Produto
**Situação:** Admin abre tela de edição, vê todos campos preenchidos, muda tudo.  
**Solução:** **PUT** - faz sentido enviar todos dados de volta.

```http
PUT /produtos/1
{
  "nome": "Novo nome completo",
  "descricao": "Nova descrição completa",
  "preco": 1999.99,
  "estoque": 50
}
```

---

### Caso 2: Sistema de Precificação Automática
**Situação:** Bot atualiza preços de milhares de produtos a cada hora.  
**Solução:** **PATCH** - só precisa enviar o preço, economiza banda.

```http
PATCH /produtos/1   { "preco": 1899.99 }
PATCH /produtos/2   { "preco": 2499.99 }
PATCH /produtos/3   { "preco": 899.99 }
# ... 10.000 produtos
```

**Economia de banda:** ~80% menos dados!

---

### Caso 3: App Mobile - Ajustar Estoque
**Situação:** App mobile do estoquista, conexão 4G instável.  
**Solução:** **PATCH** - payload pequeno, retry seguro (idempotente).

```http
PATCH /produtos/1
{
  "estoque": 25
}
```

Se conexão cair e reenviar → mesmo resultado (idempotente)!

---

## 🛡️ TRATAMENTO DE ERROS - PROBLEM DETAILS

### PUT - Produto Não Encontrado (404):
```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Produto com ID '999' não foi encontrado.",
  "instance": "/produtos/999",
  "type": "https://httpstatuses.com/404",
  "traceId": "0HN1HKP8ASQQ4:00000001",
  "errorCode": "NOT_FOUND"
}
```

### PUT - Validação Falhou (400):
```json
{
  "status": 400,
  "title": "Erro de validação",
  "detail": "Erro de validação no campo 'preco': O preço deve ser maior que zero.",
  "instance": "/produtos/1",
  "type": "https://httpstatuses.com/400",
  "traceId": "0HN1HKP8ASQQ4:00000002",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "preco": ["O preço deve ser maior que zero."]
  }
}
```

### 💡 Vantagens do Problem Details:
✅ **Padronizado** (RFC 7807 - IETF)  
✅ **Rastreável** (traceId para logs)  
✅ **Acionável** (cliente sabe exatamente o que corrigir)  
✅ **Estruturado** (máquinas conseguem processar)  

---

## 🎓 EXERCÍCIO PARA FIXAR

### Cenário: API de Tarefas (TODO List)

Você tem uma tarefa:
```json
{
  "id": 1,
  "titulo": "Estudar PUT vs PATCH",
  "descricao": "Ler documentação",
  "concluida": false,
  "prioridade": "alta",
  "dataCriacao": "2025-11-04T10:00:00Z"
}
```

### Questão 1: Marcar como Concluída
**Qual método usar? Por quê?**

<details>
<summary>👉 Clique para ver resposta</summary>

**PATCH** - Só precisa mudar 1 campo (`concluida`)

```http
PATCH /tarefas/1
{ "concluida": true }
```

Motivo: Eficiente, idempotente, semântica correta.
</details>

---

### Questão 2: Editar Tarefa Completa
**Qual método usar? Por quê?**

<details>
<summary>👉 Clique para ver resposta</summary>

**PUT** - Usuário editou tudo no formulário

```http
PUT /tarefas/1
{
  "titulo": "Estudar Middleware",
  "descricao": "Implementar global exception handler",
  "concluida": false,
  "prioridade": "media"
}
```

Motivo: Substituição total, todos dados disponíveis.
</details>

---

### Questão 3: Sistema Arquivar Tarefas Antigas
**Qual método usar? Por quê?**

<details>
<summary>👉 Clique para ver resposta</summary>

**PATCH** - Bot arquivando em massa

```http
PATCH /tarefas/1   { "arquivada": true }
PATCH /tarefas/2   { "arquivada": true }
# ... milhares
```

Motivo: Payload pequeno, performático, idempotente.
</details>

---

## 📚 REFERÊNCIAS

- [RFC 7231 - HTTP PUT](https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.4)
- [RFC 5789 - HTTP PATCH](https://datatracker.ietf.org/doc/html/rfc5789)
- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)

---

## 🎯 RESUMO - LEVE PARA CASA

```
┌─────────────────────────────────────────────────────────┐
│                    PUT vs PATCH                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  PUT = Reformar Casa 🏗️                                │
│  - Derruba TUDO e reconstrói                           │
│  - Precisa de TODOS os materiais                       │
│  - Resultado: casa nova completa                        │
│                                                         │
│  PATCH = Pintar Parede 🎨                              │
│  - Só mexe no que precisa                              │
│  - Economiza material                                   │
│  - Resultado: só parede mudou                           │
│                                                         │
│  Ambos são IDEMPOTENTES ♻️                             │
│  (executar N vezes = mesmo resultado)                   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

**FIM** 🎉

*Agora você domina PUT e PATCH como um profissional!*
