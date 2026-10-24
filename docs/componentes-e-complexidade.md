# Componentes, complexidade e leveza

Proposta preliminar de 23/10/2026. Não houve instalação, implementação ou medição de desempenho. Leveza é requisito central do programa, inclusive com fundo estático.

## O que aproveitar pronto

| Parte | Componente candidato | Trabalho próprio |
|---|---|---|
| Consumo das assinaturas de IA | Integrações do ai-usagebar | Selecionar métricas e redesenhar a apresentação. |
| Aplicativo Windows | Rust + WebView2, já presentes na base | Transformar painel temporário em widget persistente. |
| Interface | React e estilos já presentes | Desenhar versões compacta e expandida; carregar editor só quando aberto. |
| Recorte de fotos | react-easy-crop | Montar tela de importação, zoom, enquadramento, confirmação e salvamento. |
| Exportação do recorte | Canvas do próprio WebView2 | Produzir imagem final e miniatura; não requer Photoshop nem servidor de IA. |
| CPU, RAM e disco | Avaliar biblioteca Rust sysinfo ou APIs diretas do Windows | Coletar apenas dados selecionados, tratar indisponibilidade e atualizar indicadores. |
| GPU e VRAM | Avaliar contadores/APIs do Windows | Compatibilidade entre fabricantes e múltiplas GPUs. Não assumir que sysinfo resolve GPU. |
| Música | Sessões de mídia do Windows | Conectar botões, identificar sessão e mostrar título; verificar Spotify na prática. |
| Skins | CSS, SVG e configuração local | Definir formato de skin, controles de edição, fontes, cores, imagens, prévia e persistência. |
| Fundos animados | Reprodução suportada pelo WebView2 | Enquadramento, limites de resolução, pausa e modos econômicos. |

sysinfo é um candidato, não uma dependência escolhida. A versão mais recente consultada exige Rust 1.95, enquanto a base declara mínimo 1.90; escolher versão compatível ou revisar a ferramenta de compilação na implementação.

## Recorte: simples versus complexo

**Recorte de foto estática: complexidade baixa a média.** Biblioteca pronta fornece arraste, zoom, rotação e coordenadas. Nós programamos o fluxo e a gravação. A tela roda dentro do aplicativo.

**Enquadrar GIF/vídeo sem reexportar: complexidade média.** Guardar posição, escala e máscara visual e reproduzir o arquivo original. Isso preserva animação sem embutir um conversor de mídia, mas não reduz necessariamente o custo de decodificação de um arquivo grande.

**Recortar e reexportar GIF/vídeo animado: complexidade alta.** Processamento dos quadros, duração, transparência e codificação. Evitar embutir FFmpeg na primeira versão; avaliar se o usuário realmente precisa dessa exportação.

## Complexidade relativa do produto

| Funcionalidade | Complexidade preliminar |
|---|---|
| Trocar imagem, cores, fonte e selecionar indicadores | Baixa a média |
| Widget compacto/expandido e relógio circular | Média |
| Recorte de foto com prévia | Baixa a média |
| CPU e RAM totais | Baixa a média |
| GPU/VRAM em máquinas variadas | Média a alta |
| Barra de música com nome da faixa | Média; depende da sessão de mídia disponível |
| Salvar posição e arrastar entre monitores | Média; considerar escala e monitor desconectado |
| Editor de skins com layout totalmente livre | Alta; começar com posições e layouts predefinidos reduz o escopo |
| Fundos animados com pausa e modo econômico | Média |
| Sobreposição em jogos | Média a alta; não garantir tela cheia exclusiva |
| Wallpaper de desktop inteiro | Alta; fora do recorte inicial sugerido |

São avaliações de esforço e risco, não estimativas de prazo ou medições.

## Regras propostas para manter leve

- Reaproveitar o host Rust/WebView2; não acrescentar Electron ou um servidor Node/Python permanente.
- Node/Vite servem para desenvolver e empacotar, sem necessidade de um servidor desses rodando para o usuário.
- WebView2 também consome RAM e processos: comparar o custo real com uma tela nativa mínima caso a meta de leveza não seja atendida. Rust sozinho não garante um app leve.
- Carregar o editor de skins e o recorte somente quando forem abertos; liberar prévias e imagens temporárias ao fechar.
- Ler CPU/GPU em intervalo moderado configurável; ponto inicial sugerido de 1–2 segundos, a medir. Não consultar a cada quadro.
- Renovar limites de IA em intervalos próprios e respeitar limites dos serviços; o contador visual não exige nova chamada de rede a cada segundo.
- Coletar apenas indicadores ativos e reduzir trabalho quando o widget estiver oculto.
- Preferir eventos para música; evitar consultas constantes quando a sessão oferece notificações.
- Gerar versão de imagem estática adequada ao tamanho do widget; não manter uma foto enorme decodificada sem necessidade.
- Oferecer fundo estático e modo econômico; pausar animações quando ocultas. Não prometer economia específica antes de medir.
- Evitar efeitos permanentes caros, atualizações de toda a interface e um sistema genérico de plugins na primeira versão. Módulos opcionais internos podem atender ao começo.
- Medir CPU em repouso, RAM de todos os processos associados ao app/WebView2, GPU, início do programa e consumo com cada módulo e tipo de fundo.

## Recorte inicial sugerido

Widget compacto/expandido com métricas de IA da base, CPU/RAM, imagem estática com recorte e personalização de cores/fontes. Em seguida, validar GPU, barra de música e animações. O editor de layout totalmente livre e a integração de wallpaper completo ficam para uma evolução posterior.

## Fontes consultadas

- [react-easy-crop — recursos e exemplos](https://valentinh.github.io/react-easy-crop/).
- [Canvas toBlob — exportação de imagem](https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement/toBlob).
- [Sessões de mídia do Windows — informações e controles](https://learn.microsoft.com/en-us/uwp/api/windows.media.control.globalsystemmediatransportcontrolssession).
- [sysinfo — suporte a Windows e métricas de sistema](https://docs.rs/sysinfo/latest/sysinfo/).

Política desta etapa: preservar aproximadamente 4% restantes da janela de cinco horas do Codex; o saldo é compartilhado com outras sessões e pode mudar durante o trabalho.
