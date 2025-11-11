# 📮 Postman Collection - Atualização FluentValidation

## 🎯 Resumo das Atualizações

A collection do Postman foi **completamente atualizada** para incluir testes específicos do **FluentValidation**.

### 📦 O que foi adicionado

#### 1. Nova Pasta: "FluentValidation - Testes" 
Uma pasta dedicada com **15 requests** organizados:

**✅ Testes Positivos (3 requests)**:
- Produto completamente válido
- Produto sem descrição (válido - campo opcional)
- Produto com estoque zero (válido - aceita zero)

**❌ Testes Negativos (12 requests)**:
- Nome: vazio, apenas espaços, excede 200 caracteres
- Descrição: excede 1000 caracteres
- Preço: zero, negativo, mais de 2 casas decimais
- Estoque: negativo
- Múltiplos erros simultâneos

#### 2. Testes Automatizados
Cada request tem **scripts de teste** que validam:
- Status code correto (201 ou 400)
- Estrutura do Problem Details (RFC 7807)
- Presença de erros nos campos corretos
- Mensagens de erro em português
- Múltiplos erros executados simultaneamente

#### 3. Requests Existentes Atualizados
Os requests da pasta "Produtos" foram melhorados com:
- Descrições mais detalhadas sobre FluentValidation
- Scripts de teste automatizados
- Exemplos de validação específicos
- Documentação de regras violadas

### 📊 Organização da Collection

```
API Produtos - Padrões de Projeto/
├── Produtos/
│   ├── Listar Todos os Produtos
│   ├── Buscar Produto por ID
│   ├── Criar Produto - Válido ✅
│   ├── Criar Produto - Nome Vazio (Erro) ❌
│   ├── Criar Produto - Preço Inválido (Erro) ❌
│   ├── Criar Produto - Estoque Negativo (Erro) ❌
│   ├── Criar Produto - Nome Muito Longo (Erro) ❌
│   ├── Criar Produto - Descrição Muito Longa (Erro) ❌
│   ├── Criar Produto - Nome Apenas Espaços (Erro) ❌
│   ├── Criar Produto - Múltiplos Erros (Erro) ❌
│   ├── Criar Produto - Preço com Muitas Casas Decimais (Erro) ❌
│   ├── Criar Múltiplos Produtos
│   ├── Atualizar Produto Completo (PUT) - Válido
│   ├── Atualizar Produto Completo (PUT) - Erros (3 requests)
│   ├── Atualizar Produto Parcial (PATCH) - Válido
│   ├── Atualizar Produto Parcial (PATCH) - Erros (4 requests)
│   ├── Remover Produto
│   └── Mais...
│
├── FluentValidation - Testes/ ⭐ NOVO
│   ├── ✅ Produto Válido Completo
│   ├── ✅ Produto Sem Descrição (Válido)
│   ├── ✅ Produto com Estoque Zero (Válido)
│   ├── ❌ Nome: Vazio
│   ├── ❌ Nome: Apenas Espaços
│   ├── ❌ Nome: Excede 200 caracteres
│   ├── ❌ Descrição: Excede 1000 caracteres
│   ├── ❌ Preço: Zero
│   ├── ❌ Preço: Negativo
│   ├── ❌ Preço: Mais de 2 casas decimais
│   ├── ❌ Estoque: Negativo
│   └── ❌ Múltiplos Erros Simultâneos
│
└── Health Check/
    └── Verificar API Online
```

### 🧪 Como Usar a Collection

#### 1. Importar no Postman
```
File → Import → Selecionar APIProdutos.postman_collection.json
```

#### 2. Configurar Variável de Ambiente
A collection já está configurada com:
- `base_url`: http://localhost:5000
- `produto_id`: 1 (atualizado automaticamente)

#### 3. Executar Testes

**Opção 1: Testar Apenas Validações**
1. Clique com botão direito em "FluentValidation - Testes"
2. Selecione "Run folder"
3. Veja 15 testes executarem automaticamente

**Opção 2: Testar Tudo**
1. Clique no nome da collection
2. Clique em "Run"
3. Selecione todos os requests
4. Execute

**Opção 3: Testar Individualmente**
- Clique em qualquer request
- Clique em "Send"
- Veja o resultado e os testes na aba "Test Results"

### 📝 Exemplos de Uso

#### Teste de Nome Vazio
```http
POST /produtos
{
  "nome": "",
  "descricao": "Teste",
  "preco": 100,
  "estoque": 10
}
```

**Resposta Esperada**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": [
      "O nome do produto é obrigatório."
    ]
  }
}
```

**Testes Automáticos**:
- ✅ Status code is 400
- ✅ Error on Nome field
- ✅ Error message in Portuguese

#### Teste de Múltiplos Erros
```http
POST /produtos
{
  "nome": "",
  "preco": -100,
  "estoque": -50
}
```

**Resposta Esperada**:
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
    ],
    "Estoque": [
      "O estoque não pode ser negativo."
    ]
  }
}
```

**Testes Automáticos**:
- ✅ Status code is 400
- ✅ Has at least 3 validation errors
- ✅ All validations executed (não para no primeiro erro)

### 🎯 Validações Testadas

| Campo | Regra | Request de Teste |
|-------|-------|------------------|
| **Nome** | Obrigatório | ❌ Nome: Vazio |
| **Nome** | Não apenas espaços | ❌ Nome: Apenas Espaços |
| **Nome** | Máx 200 chars | ❌ Nome: Excede 200 caracteres |
| **Descrição** | Opcional | ✅ Produto Sem Descrição |
| **Descrição** | Máx 1000 chars | ❌ Descrição: Excede 1000 caracteres |
| **Preço** | > 0 | ❌ Preço: Zero, ❌ Preço: Negativo |
| **Preço** | Máx 2 decimais | ❌ Preço: Mais de 2 casas decimais |
| **Estoque** | >= 0 | ❌ Estoque: Negativo |
| **Estoque** | Aceita zero | ✅ Produto com Estoque Zero |

### 🔍 Recursos Adicionados

#### Scripts de Teste Automáticos
Todos os requests têm testes que verificam:
```javascript
pm.test("Status code is 400 Bad Request", function () {
    pm.response.to.have.status(400);
});

pm.test("Response is Problem Details format", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('errors');
    pm.expect(jsonData.errors).to.have.property('Nome');
});

pm.test("Error message in Portuguese", function () {
    var jsonData = pm.response.json();
    var nomeErrors = jsonData.errors.Nome;
    pm.expect(nomeErrors[0]).to.include('obrigatório');
});
```

#### Descrições Detalhadas
Cada request tem documentação explicando:
- Qual validador está sendo testado
- Qual regra está sendo violada
- Resultado esperado
- Pattern usado (FluentValidation)
- Mensagem de erro esperada

### 📚 Documentação na Collection

A collection inclui documentação sobre:
- ✅ Padrões implementados (Repository, Service, Factory, DTO, FluentValidation)
- ✅ Regras de validação de cada campo
- ✅ Formato de respostas de erro (Problem Details RFC 7807)
- ✅ Diferença entre PUT e PATCH
- ✅ Como interpretar mensagens de erro

### 🎓 Para Alunos

A collection serve como:
1. **Referência** - Exemplos de todos os cenários de validação
2. **Prática** - Execute e veja as validações funcionando
3. **Aprendizado** - Compare requests válidos e inválidos
4. **Testes** - Valide suas implementações
5. **Documentação** - Todas as regras explicadas

### 🚀 Próximos Passos

1. **Importar** a collection no Postman
2. **Executar** a pasta "FluentValidation - Testes"
3. **Observar** as respostas e mensagens de erro
4. **Experimentar** alterando valores
5. **Criar** novos requests baseados nos exemplos

### 💡 Dicas

**Para testar rapidamente**:
- Use "Run folder" na pasta "FluentValidation - Testes"
- Todos os 15 testes executarão em sequência
- Veja relatório consolidado ao final

**Para entender as validações**:
- Leia a aba "Description" de cada request
- Veja os scripts na aba "Tests"
- Compare requests ✅ válidos com ❌ inválidos

**Para criar novos testes**:
- Duplique um request existente
- Altere o body JSON
- Atualize os scripts de teste
- Atualize a descrição

### ✅ Checklist de Validação

Use esta collection para verificar se sua API:
- [ ] Valida nome vazio
- [ ] Valida nome apenas com espaços
- [ ] Valida tamanho máximo do nome (200)
- [ ] Valida tamanho máximo da descrição (1000)
- [ ] Valida preço zero
- [ ] Valida preço negativo
- [ ] Valida casas decimais do preço (máx 2)
- [ ] Valida estoque negativo
- [ ] Aceita estoque zero
- [ ] Aceita descrição vazia
- [ ] Retorna Problem Details (RFC 7807)
- [ ] Retorna mensagens em português
- [ ] Executa todas as validações (não para no primeiro erro)
- [ ] Retorna status 400 para validações
- [ ] Retorna status 201 para criação bem-sucedida

### 📖 Referências

- **FluentValidation**: https://docs.fluentvalidation.net/
- **Problem Details RFC 7807**: https://tools.ietf.org/html/rfc7807
- **Postman Tests**: https://learning.postman.com/docs/writing-scripts/test-scripts/

---

**Versão da Collection**: 2.0  
**Data da Atualização**: 11/11/2025  
**Requests Totais**: 35+  
**Testes Automatizados**: Sim (scripts JavaScript)  
**Compatível com**: Postman, Newman (CLI)

**Pronto para uso! 🎉**
