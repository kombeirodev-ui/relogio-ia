# Widget compacto — primeira etapa

**Goal:** entregar uma interface React local com modo compacto/expandido, seleção de indicadores e personalização de cor/foto, sem inferência de IA.

**Architecture:** aplicação web local estática, preparada para futura inclusão no host Rust/WebView2. Os serviços e sensores nativos ainda não estão conectados: valores devem aparecer como indisponíveis, nunca simulados. O repositório base permanece intacto.

**Tech Stack:** React, Vite, CSS e Canvas; testes de regras com node:test.

## Desenho adotado a partir da direção autorizada

Painel retangular compacto com linguagem de equipamento de mesa, hora real, indicadores selecionáveis e ação Expandir. Uma área de ajustes permite cor, fonte e foto estática com enquadramento e zoom. Preferências ficam neste navegador. Editor carregado sob demanda; sem fontes externas, reprodução de mídia ou chamadas a modelos.

## Etapas

1. Criar testes de comportamento em `app/model.test.mjs`: restaurar configurações corrompidas sem quebrar a interface, aceitar apenas indicadores válidos, validar arquivos e calcular recorte dentro da imagem. Executar e confirmar falha antes da implementação.
2. Implementar `app/model.mjs`, sem dependências, e executar os testes.
3. Criar `package.json`, `index.html`, `app/main.jsx`, `app/styles.css` e editor sob demanda `app/SkinEditor.jsx`. Tratar falhas de armazenamento/importação sem descartar a última configuração válida.
4. Compilar, verificar os fluxos em navegador, tamanho dos arquivos gerados, mensagens de erro e persistência após recarregar. Salvar print e instruções na pasta de entregas desta conversa.
5. Atualizar README e continuidade. Registrar em commit e enviar a branch do projeto. Não publicar site nem modificar o Escriba.

## Critérios de aceite

- Expandir/recolher funciona e possui rótulo acessível.
- Usuário escolhe CPU/GPU/RAM/disco; nenhum valor de hardware ou assinatura é inventado.
- Cor, fonte e indicadores persistem ao recarregar.
- Foto passa por validação de tamanho/formato, tem prévia de enquadramento e pode ser removida.
- Recorte de foto é local e não usa API. Arquivo grande ou inválido produz erro compreensível.
- Sem chamadas a serviços de IA. Não afirmar métricas de desempenho do aplicativo desktop antes da integração e medição.

## Limites desta entrega

É a primeira interface funcional para testar no navegador. Ainda não é um executável de desktop, não flutua sobre jogos e não contém sensores ou Spotify. O host nativo vem numa etapa posterior.
