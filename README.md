# Relógio IA

Projeto em planejamento de um widget desktop compacto, expansível e personalizável para acompanhar limites de uso de IA, indicadores de hardware e controles de música.

## Experiência desejada

- Usabilidade compacta e personalizável inspirada no Winamp dos anos 2000.
- Criador de skins com imagens, recorte, fontes, botões e fundos animados de estilos livres.
- Indicadores selecionáveis de CPU, GPU, RAM e disco, úteis durante o uso de IA local.
- Indicadores de assinatura dos serviços de IA conforme os dados disponíveis.
- Barra opcional de música com nome da faixa, anterior, play/pause e próxima.
- Posição livre entre monitores, modo compacto/expandido e opção de sobreposição.
- Baixo consumo de recursos como requisito a validar.

Não há implementação dessas melhorias. Não faz parte do escopo detectar automaticamente todos os aplicativos que usam IA nem atribuir consumo a cada aplicativo. Os indicadores de hardware mostram utilização do PC.

## Documentação

- [Briefing e ideias](docs/Projeto%20Relogio%20IA.md)
- [Estudo da base](docs/estudo-relogio-ia.md)

## Referência e autoria

A base técnica estudada é [ai-usagebar, de AkitaOnRails](https://github.com/akitaonrails/ai-usagebar), versão declarada 1.43.0, commit d1af986fe4bf8d05316bc8d6e1506881b5f4d265. Este é um projeto independente, sem vínculo ou endosso declarado do autor original.

O código da base não está incluído neste repositório de planejamento. A cópia local em `base/` é ignorada pelo Git. As imagens de referência em `docs/imagens` vieram do repositório original; veja [os avisos e a licença MIT original](THIRD_PARTY_NOTICES.md).

Ao incorporar código da base, preservar sua licença e autoria e verificar dependências e demais recursos. A licença da base não define automaticamente uma licença para todos os futuros componentes próprios.
