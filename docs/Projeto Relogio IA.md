---
tipo: projeto
status: planejamento
criado_em: 2026-10-23
atualizado_em: 2026-10-23
tags:
  - projeto
  - widget
  - ia
  - skins
---

# Relógio IA — widget e criador de skins

Nome provisório. Projeto independente do Escriba: relógio e criador de skins pertencem a este mesmo projeto. Integrações com outros aplicativos não incorporam seus códigos ou seus projetos aqui.

## Objetivo

Criar um widget desktop pequeno para acompanhar o consumo de IA, com aparência personalizável para combinar com o wallpaper do usuário. A direção visual é livre, permitindo ao usuário criar sua própria skin e combinar o widget com qualquer wallpaper. Anime foi apenas um exemplo de estilo possível.

Status atual: levantamento de ideias e estudo da base. Não há implementação das novas funcionalidades nem validação prática das integrações.

## Base e localização

- Base: [akitaonrails/ai-usagebar](https://github.com/akitaonrails/ai-usagebar).
- Snapshot estudado: `d1af986fe4bf8d05316bc8d6e1506881b5f4d265`, versão declarada 1.43.0.
- Pasta própria: `C:\Users\paulo\Documents\Codex\Projetos\Relogio IA`.
- Código original preservado em `base/ai-usagebar` nessa pasta.
- Documentação e imagens de referência em `docs`.
- Licença principal MIT; preservar autoria e licença. Conferir dependências e recursos visuais antes de distribuição comercial.

## Ideias solicitadas — para desenvolvimento futuro

### Widget compacto e expansível

- Formato pequeno, retangular e mais horizontal que o painel original.
- Possibilidade de expandir para detalhes e recolher para o modo compacto.
- Arraste entre monitores, independentemente da tela onde a IA está sendo utilizada.
- Salvar posição e tamanho; recuperar o widget se um monitor for desconectado.
- Opção de manter visível e travar a posição.
- Mostrar porcentagem consumida ou restante e tempo até renovação.
- Períodos reais de cada serviço; resumo mensal separado quando não existir cota mensal informada.

### Criador de skins integrado

- Personalizar a aparência do relógio e combinar com o wallpaper do PC.
- Importar imagens próprias de qualquer estilo; anime é apenas um exemplo.
- Tela para recortar, reposicionar e enquadrar a imagem antes de aplicar.
- Fundos animados, incluindo GIFs, e possibilidade de alternar imagens.
- Personalizar relógio/mostradores, cores, tipografia e aparência dos botões.
- Prévia da composição antes de aplicar; salvar e reutilizar skins é uma proposta a validar.
- Manter legibilidade dos indicadores sobre fundos variados.

### Baixo consumo de recursos

Requisito expresso do usuário: animações não devem pesar no PC ou consumir energia desnecessariamente. Ainda não há medições ou metas numéricas.

Propostas para avaliação: limitar quadros por segundo, pausar animações quando o widget estiver oculto ou minimizado, modo econômico e alternativa de fundo estático. Atualizar a contagem visual sem consultar as APIs a cada quadro.

### Módulo opcional de música / Spotify

- Play, pause, próxima e anterior.
- Apresentação coerente com a skin, sem ocupar espaço obrigatório no widget.
- Avaliar controles de mídia do Windows versus integração específica com Spotify; compatibilidade, autenticação e restrições ainda não foram pesquisadas.
- O termo plugin descreve a ideia de um módulo opcional. Não há arquitetura de plugins definida.

### Atalhos para IA e projetos

- Botão para abrir o Codex; avaliar também Claude e outros aplicativos.
- Possibilidade de acessar um projeto para conferir seu andamento.
- Abrir automaticamente junto da ferramenta de IA, se desejado.
- Abrir um aplicativo é diferente de consultar seu estado: progresso de projeto depende de uma integração ou fonte de dados apropriada.

### Monitoramento de hardware durante o uso de IA

Correção de escopo pelo usuário: não é necessário detectar quais aplicativos estão usando IA nem atribuir consumo a cada aplicativo. A intenção é acompanhar os indicadores de CPU e GPU enquanto utiliza IA, especialmente modelos locais.

Os indicadores mostram a utilização do hardware do PC, sem afirmar que esse uso pertence exclusivamente à IA. RAM e disco continuam como opções selecionáveis já solicitadas. A forma de ativar ou exibir esses marcadores ainda será definida; não foi solicitado um mecanismo de detecção automática de IA.

O Escriba foi um exemplo de contexto de uso, não uma exigência de integração ou identificação de processos. Continua sendo um projeto separado.

### Sons e wallpaper

- Sons opcionais para eventos a definir, com opção de silenciar.
- Wallpaper de tela inteira continua como possibilidade futura; o primeiro conceito é uma imagem ou animação dentro do widget.

## Caminho técnico preliminar

A recomendação inicial é aproveitar a coleta de uso e a interface React do ai-usagebar, adicionando um modo persistente ao host Windows em Rust/WebView2. O painel original se fecha ao clicar fora e não oferece o comportamento livre desejado.

Alternativa: criar uma interface separada consumindo o relatório JSON da base. Essa decisão continua aberta até o desenho da experiência e a verificação das necessidades da janela.

## Próxima retomada

1. Cadastrar a pasta própria na aba Projetos do Codex. As ferramentas disponíveis nesta sessão não oferecem cadastro de novos projetos; a preparação dos arquivos não equivale ao cadastro na interface.
2. Definir o desenho compacto e expandido e a primeira experiência do criador de skins.
3. Escolher o recorte inicial; Spotify, indicadores de hardware e atalhos são ideias registradas, sem promessa de disponibilidade pronta.
4. Validar fontes de dados e limitações das integrações antes de implementar.

## Referências visuais do projeto original

Os prints a seguir vieram do repositório, não de uma execução nossa. Mostram a base atual, sem o relógio e as skins propostos.

![Painel original em português](imagens/windows-tray-native-pt-br.png)

![Painel original clássico](imagens/windows-tray-dashboard.png)

![Configurações originais](imagens/windows-tray-settings.png)

[Código React da base estudada](https://github.com/akitaonrails/ai-usagebar/tree/d1af986fe4bf8d05316bc8d6e1506881b5f4d265/windows/popover/src).

## Correção do objetivo de monitoramento

A proposta anterior de detectar toda IA do PC e atribuir consumo por aplicativo foi retirada do escopo pelo usuário. O objetivo é mostrar CPU e GPU durante o uso de IA, mantendo os indicadores de hardware selecionáveis. As métricas de assinatura dos provedores já previstas são uma função separada.

## Monitor de recursos selecionáveis — IA local

Ideia solicitada em 23/10/2026: incluir indicadores de hardware para acompanhar o impacto de modelos locais, com liberdade para escolher quais ficam visíveis. Não é necessário mostrar todos simultaneamente.

- CPU: utilização do processador.
- GPU: utilização da placa de vídeo; avaliar também memória de vídeo (VRAM) como indicador opcional, especialmente para modelos locais.
- RAM: memória utilizada e disponível.
- Disco (HD/SSD): avaliar atividade de leitura/escrita e espaço utilizado/disponível como indicadores distintos. A seleção final depende do que o usuário quiser acompanhar.
- Selecionar indicadores pela interface e preservar a escolha; acomodar o conjunto escolhido no modo compacto e nos detalhes expandidos.

A apresentação mostra utilização do hardware do PC. Identificação de IA e atribuição por aplicativo não fazem parte do escopo corrigido; uso total de GPU ou CPU não deve ser rotulado como exclusivamente da IA.

Tratar suporte a diferentes placas e GPUs múltiplas na avaliação técnica. Coletar métricas em intervalo adequado, para que o próprio monitor e os fundos animados tenham baixo impacto. Trata-se de um requisito registrado, ainda sem implementação ou medição.

## Complemento — barra de música e sobreposição durante jogos

Pedido de 23/10/2026, para desenvolvimento futuro: disponibilizar uma pequena barra opcional de mídia dentro do widget, combinando com a skin. Spotify é o caso principal, mas não há exigência de integração direta com sua API.

- Botões para música anterior, play/pause e próxima.
- Avaliar controles de mídia do Windows e atalhos configuráveis como alternativas a um plugin específico do Spotify.
- Permitir ativar ou ocultar a barra sem obrigar o usuário a deixar um aplicativo de música aberto na tela.
- Objetivo de uso: jogar enquanto a IA trabalha em segundo plano, acompanhando o widget e controlando a música.
- Oferecer opção de manter o widget acima das janelas e permitir ao usuário escolher onde e quando ele aparece. Avaliar alternância rápida entre interagir com os botões e deixar os cliques passarem para a janela abaixo.
- Compatibilidade com jogos precisa ser validada: manter uma janela acima das demais não garante sobreposição em tela cheia exclusiva. Avaliar primeiro modo janela e janela sem bordas; não prometer funcionamento sobre qualquer jogo.
- Preservar foco do jogo quando possível e manter baixo impacto de renderização e coleta de métricas.

Requisito registrado, sem implementação. O nome plugin continua sendo uma ideia de módulo opcional, não uma escolha técnica já feita.

## Complemento — identificação da música

O usuário confirmou que a barra de mídia deve mostrar o nome da música em reprodução, além dos botões de controle. Avaliar também artista e, opcionalmente, capa, sem torná-los requisitos confirmados.

Na avaliação técnica, verificar se os metadados fornecidos pela sessão de mídia do Windows atendem ao Spotify e aos modos de uso desejados. Se não atenderem, estudar a integração com a API do Spotify, incluindo autenticação e condições aplicáveis. A necessidade de API ainda não está decidida. Quando não houver metadados, indicar essa ausência sem inventar o título.

## Referência de experiência — Winamp dos anos 2000

Referência trazida pelo usuário: a usabilidade do Winamp do começo dos anos 2000, sem copiar a skin ou a identidade visual do Winamp.

Interpretar como direção para um pequeno painel desktop pessoal: controles acessíveis, formato compacto, possibilidade de expandir/recolher e aparência profundamente personalizável. A identidade é definida por cada usuário para combinar com seu wallpaper; anime foi apenas um exemplo, sem limitar os estilos disponíveis.

Relógio/consumo de IA, indicadores de hardware selecionáveis e barra de música devem compor uma experiência coesa, ajustável pelo usuário, em vez de ocupar obrigatoriamente uma tela grande. A organização exata dos módulos e a possibilidade de painéis separados ainda serão desenhadas e validadas.

Esta é uma referência de interação para o futuro desenho de UX, não uma solicitação de reproduzir a interface original ou uma mudança para implementar agora.


## Esclarecimento visual — personalização livre

O usuário esclareceu que anime era apenas um exemplo. O produto deve permitir skins e imagens de estilos variados, conforme a preferência de cada pessoa. A referência Winamp diz respeito à usabilidade compacta e personalizável, sem impor uma estética específica.


