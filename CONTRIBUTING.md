# Guia de Contribuição

Este documento descreve o processo de contribuição para este projeto através de fork.

## 📋 Pré-requisitos

- Ter uma conta no GitHub
- Git instalado em sua máquina

## 🔄 Processo de Contribuição via Fork

### 1. Fazer o Fork do Repositório

Crie uma cópia pessoal do repositório em sua conta:

1. Acesse o repositório original: `https://github.com/lucasfogacadj/padroes-de-projeto-DB-Terca`
2. Clique no botão **Fork** no canto superior direito
3. Aguarde a criação do fork em sua conta

### 2. Clonar o Fork

Clone o repositório forkado para sua máquina local:

```bash
git clone https://github.com/SEU-USUARIO/padroes-de-projeto-DB-Terca.git
cd padroes-de-projeto-DB-Terca
```

### 3. Configurar o Repositório Upstream

Adicione o repositório original como upstream para manter seu fork atualizado:

```bash
git remote add upstream https://github.com/lucasfogacadj/padroes-de-projeto-DB-Terca.git
git remote -v
```

### 4. Criar uma Branch

Crie uma branch específica para sua contribuição:

```bash
git checkout -b feature/nome-da-sua-feature
```

**Convenções de nomenclatura:**
- `feature/` - para novas funcionalidades
- `fix/` - para correções de bugs
- `docs/` - para documentação
- `refactor/` - para refatoração de código

### 5. Fazer as Alterações

Faça suas modificações no código e commit:

```bash
git add .
git commit -m "Descrição clara das alterações"
```

**Boas práticas para commits:**
- Use mensagens descritivas e objetivas
- Commits pequenos e focados
- Escreva em português ou inglês (mantenha consistência)

### 6. Manter o Fork Atualizado

Antes de fazer push, sincronize com o repositório original:

```bash
git fetch upstream
git merge upstream/main
```

Ou, se preferir rebase:

```bash
git fetch upstream
git rebase upstream/main
```

### 7. Fazer Push para o Fork

Envie suas alterações para o fork no GitHub:

```bash
git push origin feature/nome-da-sua-feature
```

### 8. Criar um Pull Request (PR)

1. Acesse seu fork no GitHub
2. Clique em **Compare & pull request**
3. Preencha o título e descrição do PR com:
   - Descrição clara das mudanças
   - Motivação para a contribuição
   - Testes realizados (se aplicável)
4. Clique em **Create pull request**

## ✅ Checklist Antes de Enviar o PR

- [ ] Código está funcionando corretamente
- [ ] Testes passam (se houver)
- [ ] Documentação atualizada (se necessário)
- [ ] Commits com mensagens descritivas
- [ ] Branch atualizada com o repositório original
- [ ] Código segue os padrões do projeto

## 🔍 Revisão do Pull Request

Após criar o PR:

1. Os mantenedores do projeto revisarão seu código
2. Podem solicitar alterações ou melhorias
3. Faça as mudanças necessárias na mesma branch
4. Push das alterações atualizará automaticamente o PR
5. Após aprovação, o PR será mesclado ao repositório original

## 💡 Dicas

- **Mantenha seu fork sincronizado**: Atualize regularmente com o upstream
- **Comunicação**: Seja claro e educado nas discussões do PR
- **Paciência**: A revisão pode levar algum tempo
- **Issues**: Verifique issues abertas antes de começar uma nova feature

## 📞 Contato

Se tiver dúvidas sobre o processo de contribuição, abra uma issue no repositório ou entre em contato com os mantenedores.

---

**Obrigado por contribuir! 🚀**
