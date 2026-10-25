# Relógio IA v0.1.0-beta

Primeira beta do relógio nativo para Windows. Reúne cotas de IA, sensores selecionáveis e controles locais do Spotify em uma janela personalizável.

## Incluído

- Nove templates inspirados nos players, programas de bate-papo e interfaces clássicas dos anos 2000 e atuais, com desenhos próprios e identidade OTIMIZEPC autorizada.
- Cotas Codex, horário da leitura, renovação e tratamento de indisponibilidade.
- CPU, GPU, RAM e ocupação de disco opcionais.
- Faixa e artista Spotify, transporte, shuffle, repetição e volume.
- Editor integrado, foto, fade, tamanho, números personalizados e alertas WAV opcionais.

## Download

`RelogioIA-Publicacao-Windows.zip` contém o executável portátil, instruções, créditos, licenças das fontes e SHA256 do executável. Extraia o ZIP antes de abrir o aplicativo. Não é um instalador nem um executável com assinatura digital de editor.

O código correspondente fica em `RelogioIA-Codigo-Publicacao.zip`, sem histórico Git, imagens de referência antigas ou preferências pessoais. No repositório futuro, os arquivos desse pacote devem ficar na raiz, sem os binários de `desktop/release/`.

## O que já foi testado

40 testes automatizados aprovados na exportação limpa. Os nove temas foram renderizados em arquivos e conferidos. O ZIP final foi extraído e executado neste PC: abriu, respondeu e fechou normalmente. Cotas, sensores e comandos reais do Spotify foram conferidos.

## Limitações da beta

- Sem nome da playlist Spotify; controles dependem da sessão local disponibilizada.
- Claude experimental, ainda sem conta real validada; usa endpoint não documentado publicamente.
- Abertura com Windows e sobreposição no jogo foram confirmadas pelo mantenedor na build pessoal. Não repetidas na edição pública; outros jogos, tela cheia exclusiva e cenários completos de múltiplos monitores permanecem sem validação.
- Sem fundo animado. Sensores e cotas não representam custo monetário nem atribuição por aplicativo.
- Código próprio sob MIT; materiais de terceiros têm avisos e licenças específicos.

Esta versão é um pré-lançamento. Consulte README.md e docs/TESTE-DO-PACOTE.md para instruções e escopo dos testes.
