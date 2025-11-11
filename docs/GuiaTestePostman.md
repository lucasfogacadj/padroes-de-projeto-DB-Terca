# 🧪 Guia Rápido - Testando FluentValidation com Postman

## ⚡ Setup em 3 Passos

### 1️⃣ Importar Collection
```
Postman → File → Import → Selecionar APIProdutos.postman_collection.json
```

### 2️⃣ Iniciar a API
```powershell
cd D:\Aula\BackEndTer\padroes-de-projeto-DB-Terca
dotnet run
```

### 3️⃣ Executar Testes
```
Postman → FluentValidation - Testes → Run folder
```

---

## 🎯 Testes Rápidos

### ✅ Teste 1: Produto Válido
**Request**: `✅ Produto Válido Completo`

**Resultado Esperado**: 
- Status: `201 Created`
- Body: Produto criado com ID

### ❌ Teste 2: Nome Vazio
**Request**: `❌ Nome: Vazio`

**Resultado Esperado**:
- Status: `400 Bad Request`
- Erro: "O nome do produto é obrigatório."

### ❌ Teste 3: Múltiplos Erros
**Request**: `❌ Múltiplos Erros Simultâneos`

**Resultado Esperado**:
- Status: `400 Bad Request`
- Erros em: Nome, Preço, Estoque
- Todos validados simultaneamente

---

## 📋 Checklist de Validações

Execute e marque ✅:

**Nome**:
- [ ] ❌ Vazio → 400
- [ ] ❌ Apenas espaços → 400
- [ ] ❌ > 200 caracteres → 400
- [ ] ✅ Válido → 201

**Descrição**:
- [ ] ✅ Vazia (opcional) → 201
- [ ] ❌ > 1000 caracteres → 400

**Preço**:
- [ ] ❌ Zero → 400
- [ ] ❌ Negativo → 400
- [ ] ❌ > 2 decimais → 400
- [ ] ✅ Válido → 201

**Estoque**:
- [ ] ❌ Negativo → 400
- [ ] ✅ Zero → 201
- [ ] ✅ Positivo → 201

---

## 🔍 Interpretando Respostas

### Sucesso (201 Created)
```json
{
  "id": 1,
  "nome": "Produto Teste",
  "descricao": "Descrição",
  "preco": 99.99,
  "estoque": 10,
  "dataCriacao": "2025-11-11T10:30:00"
}
```

### Erro de Validação (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": [
      "O nome do produto é obrigatório."
    ],
    "Preco": [
      "O preço deve ser maior que zero."
    ]
  }
}
```

---

## 💡 Dicas

### Ver Testes Automatizados
1. Envie um request
2. Clique na aba "Test Results"
3. Veja testes passando ✅ ou falhando ❌

### Executar Todos os Testes
1. Clique com botão direito em "FluentValidation - Testes"
2. Selecione "Run folder"
3. Veja relatório completo

### Criar Novo Teste
1. Duplique um request existente
2. Altere o JSON do body
3. Atualize a descrição
4. Execute!

---

## 🚀 Comandos Úteis

### Verificar API está rodando
```powershell
Invoke-WebRequest http://localhost:5000 -UseBasicParsing
```

### Ver logs em tempo real
```powershell
dotnet run --verbosity detailed
```

### Recompilar após mudanças
```powershell
dotnet build
```

---

## ❓ Troubleshooting

### Erro: "Could not send request"
- ✅ Verifique se a API está rodando (`dotnet run`)
- ✅ Confirme a porta (padrão: 5000)
- ✅ Verifique variável `base_url` no Postman

### Erro: Unexpected response
- ✅ Limpe o banco: delete `app.db`
- ✅ Execute migrations: `dotnet ef database update`
- ✅ Reinicie a API

### Testes falhando
- ✅ Verifique se implementou FluentValidation
- ✅ Confirme mensagens de erro em português
- ✅ Valide estrutura do Problem Details

---

## 📖 Próximos Passos

1. ✅ Execute todos os 15 testes da pasta FluentValidation
2. ✅ Observe as diferenças entre requests válidos e inválidos
3. ✅ Leia as descrições de cada request
4. ✅ Experimente modificar valores
5. ✅ Crie seus próprios testes

---

**Boa prática! 🎯**
