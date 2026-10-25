# Teste do pacote final — 25/10/2026

O ZIP Windows foi extraído em uma pasta nova e o SHA256 do executável foi conferido. O teste foi feito no Windows atual, não em máquina virtual ou instalação limpa.

- O executável abriu, permaneceu respondendo e fechou normalmente.
- As duas cotas Codex e CPU, GPU, RAM e disco retornaram dados reais.
- Spotify foi identificado com faixa, artista e controles disponíveis.
- Play/pause, próxima e anterior, shuffle, três modos de repetição, volume e mudo foram executados e seus efeitos conferidos por leitura do estado.
- Faixa, reprodução, shuffle, repetição, volume e mudo retornaram aos estados iniciais. Spotify aceitou a solicitação de retorno à posição original.
- Não houve controle do mouse ou teclado nem alteração do volume geral do Windows.
- A build pessoal continua com o hash original.

Os testes de música chamaram os mesmos métodos usados pelos botões, sem clicar fisicamente neles. Isso verifica os comandos e efeitos reais; não equivale a percorrer manualmente todas as telas.

Na edição pública, permanecem fora desta validação: iniciar após reiniciar Windows, jogos em tela cheia, todos os cenários de múltiplos monitores, Claude com conta real e compatibilidade em outro computador. Os testes posteriores do mantenedor na build pessoal estão descritos abaixo. Os 40 testes automatizados anteriores e os testes feitos durante o desenvolvimento também fazem parte da evidência.

## Resultado dos comandos reais de música

OK faixa e artista legiveis
OK play/pause altera estado
OK play/pause retorna ao estado inicial
OK barra e diminuir volume Spotify
OK aumentar volume Spotify
OK botao mudo
OK volume e mudo restaurados
OK shuffle muda estado
OK shuffle restaurado
OK repeticao modo 2
OK repeticao modo 1
OK repeticao modo 0
OK repeticao restaurada
OK proxima altera a faixa
OK anterior retorna a faixa original
OK posicao original aceita para restauracao
OK estado final de reproducao restaurado
Falhas: 0

## Testes relatados pelo mantenedor após reiniciar o Windows

Na build pessoal que estava em uso, o relógio abriu automaticamente com o Windows. No jogo testado, permaneceu sobreposto; foi movimentado, recebeu cliques e play/pause de música, e o retorno ao jogo ocorreu normalmente.

O modo foi descrito pelo mantenedor como janela cheia. Não foi confirmado como tela cheia exclusiva. A validação se limita ao jogo e ao ambiente usados. Para interagir com o relógio durante jogos, recomenda-se janela sem bordas; tela cheia exclusiva pode perder foco ou minimizar ao clicar em outra janela.

Esses testes do mantenedor foram feitos na build pessoal, não repetidos na edição separada de publicação. O teste automatizado de abertura e comandos Spotify do pacote público permanece registrado nas seções anteriores.