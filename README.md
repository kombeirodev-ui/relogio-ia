# Relógio IA

Primeira interface funcional de um widget desktop personalizável. A prévia local mostra limites reais do Codex e permite configurar mostrador, letras, fundo e indicadores extras.

## Experiência desejada

- Usabilidade compacta e personalizável inspirada no Winamp dos anos 2000.
- Criador de skins com imagens, recorte, fontes, botões e fundos animados de estilos livres.
- Indicadores selecionáveis de CPU, GPU, RAM e disco, úteis durante o uso de IA local.
- Indicadores de assinatura dos serviços de IA conforme os dados disponíveis.
- Barra opcional de música com nome da faixa, anterior, play/pause e próxima.
- Posição livre entre monitores, modo compacto/expandido e opção de sobreposição.
- Baixo consumo de recursos como requisito a validar.

Implementado na prévia: consumo do Codex no modo compacto e expandido, mostrador de sete segmentos, estilos tipográfico/contorno, importação de números em PNG, cinco famílias locais para letras, foto com recorte e fade de 0 a 100%, prévia fixa durante a edição e preferências locais persistentes.

Ainda não implementado: integração Claude, leituras de hardware, Spotify, fundos animados e janela/executável de desktop. Não faz parte do escopo detectar automaticamente todos os aplicativos que usam IA nem atribuir consumo por aplicativo.

## Rodar a prévia

Requer Node.js 22.12+ e pnpm. Execute `pnpm install` e `pnpm dev`, depois abra http://127.0.0.1:4387. Para verificar: `pnpm test` e `pnpm build`.

A consulta de consumo usa o Codex instalado e autenticado, disponível no PATH. O conector do Vite chama somente `initialize` e `account/rateLimits/read`, sem iniciar inferência. O processo temporário é encerrado após a consulta, com cache de 60 segundos. Nenhuma credencial é enviada ao navegador. A compilação estática sozinha não inclui esse conector: executar `pnpm preview` ou o servidor de desenvolvimento para consultar limites.

As preferências, fotos e números importados ficam no armazenamento deste navegador. Os arquivos de foto são processados localmente. A página mostra conexão indisponível quando não consegue obter limites; não fabrica valores.

## Números personalizados

Importe um PNG de até 1 MB contendo 11 células iguais na ordem `0123456789:`. Exemplo: 1100 × 150 pixels, cada símbolo em sua célula de 100 × 150. Recomenda-se fundo transparente. A largura precisa ser múltipla de 11; altura entre 16 e 512 pixels. Não há reconhecimento por IA: cada célula corresponde diretamente a um símbolo. A importação normaliza o tamanho preservando proporções e cores.

## Documentação

- [Briefing e ideias](docs/Projeto%20Relogio%20IA.md)
- [Estudo da base](docs/estudo-relogio-ia.md)

## Referência e autoria

A base técnica estudada é [ai-usagebar, de AkitaOnRails](https://github.com/akitaonrails/ai-usagebar), versão declarada 1.43.0, commit d1af986fe4bf8d05316bc8d6e1506881b5f4d265. Este é um projeto independente, sem vínculo ou endosso declarado do autor original.

O código da base ainda não foi incorporado à nova interface. A cópia local em `base/` é ignorada pelo Git. Os prints de referência `windows-tray-*.png` em `docs/imagens` vieram do repositório original; veja [os avisos e a licença MIT original](THIRD_PARTY_NOTICES.md). A interface em `app/` e o conector em `server/` foram criados para este projeto.

Ao incorporar código da base, preservar sua licença e autoria e verificar dependências e demais recursos. A licença da base não define automaticamente uma licença para todos os futuros componentes próprios.
