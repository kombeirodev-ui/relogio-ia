# Correções solicitadas na primeira interface

Direção confirmada pelo usuário: o propósito principal é acompanhar uso das assinaturas de IA; hardware é opcional. A foto precisa aparecer sem filtro obrigatório, com controle de fade de 0 a 100%. A prévia deve permanecer visível durante os ajustes. O relógio desejado usa números digitais de sete segmentos, como rádio-relógios antigos, sem ponteiros.

## Escopo desta correção

1. Exibir Codex e Claude no modo compacto. Conectar limites reais do Codex por consulta de status ao app-server instalado, sem iniciar inferência. Conexão indisponível deve aparecer como indisponível, sem inventar consumo; Claude permanece explicitamente não conectado nesta etapa.
2. Retirar o filtro fixo; oferecer escurecimento de 0 a 100%, persistido, com 0% por padrão. Permitir cor do texto para contraste local, sem alterar a foto.
3. Fixar a prévia durante a edição em telas largas e estreitas.
4. Adicionar números de sete segmentos e importação de faixa PNG com onze células iguais em ordem 0123456789:. Validar dimensões e tamanho. Não usar IA para interpretar desenhos.
5. Testar preferências antigas, fade zero, valores inválidos, recorte da faixa e tratamento de janelas de limite ausentes. Compilar e conferir os fluxos na prévia.

## Arquitetura

Manter React e CSS, sem novas bibliotecas visuais. SVG para os segmentos. Conector de desenvolvimento no próprio Vite lê apenas status de cota do Codex via subprocesso temporário, encerra-o após a leitura e mantém cache de 60 segundos. O navegador recebe somente percentuais, períodos e renovações. Um executável final com host Rust continua sendo etapa futura; este servidor é da prévia local.
