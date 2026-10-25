# Roadmap — Linux e macOS

Registrado em 25/10/2026. Planejamento futuro solicitado pelo mantenedor; nenhuma versão Linux ou macOS está implementada ou prometida para esta beta.

## Objetivo

Levar o Relógio IA para Linux e macOS, mantendo uma interface compacta, personalizável e leve, cotas de IA e módulos opcionais. O estilo dos templates pode ser reaproveitado como referência de desenho; o executável WinForms atual não é uma versão multiplataforma.

## O que existe no projeto original do Akita

Documentação consultada no repositório oficial ai-usagebar, commit 2a54310892d0e3ca65dee22f32cfbcba4a0db018:

- Linux: integrações com Waybar, Omarchy, GNOME Shell e KDE Plasma 6; frontend GTK experimental para Linux Mint. O widget Waybar é específico de Wayland.
- macOS: leitor e interface de barra de menus ai-usagebar-tray, com NSStatusItem e popover WKWebView; documentação de instalação e compilação própria.
- O leitor e a interface original não equivalem ao nosso widget com skins. Estudar a reutilização do leitor/relatório de cotas como componente separado, preservando MIT e créditos do Akita.

Fontes: [README original](https://github.com/akitaonrails/ai-usagebar/blob/2a54310892d0e3ca65dee22f32cfbcba4a0db018/README.md), [macOS](https://github.com/akitaonrails/ai-usagebar/blob/2a54310892d0e3ca65dee22f32cfbcba4a0db018/macos/README.md), [instalação macOS](https://github.com/akitaonrails/ai-usagebar/blob/2a54310892d0e3ca65dee22f32cfbcba4a0db018/macos/INSTALL.md).

Esta consulta usa uma revisão mais recente que o snapshot originalmente estudado pelo Relógio IA. Não houve atualização automática do leitor nem importação de código dessa revisão.

## Etapas comuns, antes de escolher tecnologia

- [ ] Avaliar reaproveitamento do leitor multiplataforma original e seu formato de dados, login, cache e licenças.
- [ ] Separar leitura de cotas, desenho, preferências e integrações específicas do Windows.
- [ ] Comparar alternativas de interface e medir memória, CPU, tamanho do pacote e custo de renderização. Não escolher framework apenas por facilidade de portabilidade.
- [ ] Definir uma versão mínima: horário, cotas, posição, tamanho e preferências. Música e sensores podem entrar em etapas separadas.
- [ ] Preservar fontes licenciadas, gráficos próprios e créditos; não adicionar assets antigos removidos da edição pública.

## Linux

- [ ] Definir distribuições, ambientes de desktop e arquiteturas de início.
- [ ] Validar leitura real de cotas e credenciais no Linux.
- [ ] Portar janela, temas e preferências; testar X11/Wayland nos ambientes escolhidos, inclusive posição, monitores e sobreposição. Registrar as restrições encontradas.
- [ ] Avaliar controle de música local por MPRIS, sem assumir que todos os players expõem os mesmos recursos.
- [ ] Implementar sensores e identificar quais GPUs têm leitura disponível, mantendo os módulos opcionais.
- [ ] Definir empacotamento, inicialização com a sessão e processo de atualização.
- [ ] Validar instalação e consumo de recursos em Linux real ou ambiente de teste adequado antes de declarar suporte.

## macOS

- [ ] Avaliar reaproveitamento do leitor original, caminhos de login e acesso ao Keychain sem copiar credenciais do usuário.
- [ ] Portar a interface com skins, preferências e controles de janela; definir diferenças entre widget flutuante e barra de menus.
- [ ] Estudar integração de mídia e sensores próprios do sistema; a integração Windows atual não é reutilizável diretamente.
- [ ] Definir suporte a Apple Silicon e, se viável, Intel, além da versão mínima do macOS.
- [ ] Definir empacotamento e avaliar assinatura/notarização para distribuição.
- [ ] Preparar compilação e testes em ambiente macOS quando disponível; um runner pode ajudar, mas não substitui a conferência visual e funcional.
- [ ] Obter validação em Mac real por colaborador/testador. Enquanto isso, identificar como experimental e não validado, sem prometer compatibilidade.

Não ter um Mac agora não impede documentar, projetar ou preparar partes compartilhadas. Impede afirmar que a versão final foi testada no sistema.

## Critério de entrega

Versões independentes, com requisitos claros por sistema, testes de cotas e interface, limites documentados, pacote instalável e medições de desempenho. O roadmap não altera a build pessoal nem a Release Windows v0.1.0-beta.