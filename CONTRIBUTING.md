# Como contribuir

O mantenedor revisa as propostas antes de incorporá-las. Abrir um pedido de alteração não muda automaticamente a versão oficial.

## Sugerir uma melhoria ou informar um problema

Abra uma Issue no repositório,  com o resultado esperado, o que aconteceu, passos para reproduzir, versão do Windows e versão do relógio. Para problemas visuais, inclua tema, largura da janela e captura sem dados pessoais. Não envie arquivos de login, tokens ou a pasta inteira de preferências.

## Ambiente para desenvolvimento

- Windows com .NET Framework 4.8.
- Windows SDK 10 instalado, incluindo `Windows.winmd` e referências WinRT.
- PowerShell e compilador C# do .NET Framework, detectados pelo script.

Na raiz do projeto:

```powershell
powershell -NoProfile -File .\desktop\build.ps1 -Test
```

O resultado fica em `desktop/release/RelogioIA.exe`; os testes ficam em `desktop/release/Tests.exe`. Não execute o aplicativo para rodar os testes: o script usa o executável de testes separado. SDK é requisito de compilação, não de uso do programa pronto.

## Enviar alterações

1. Crie uma cópia do repositório na sua conta e uma branch para sua mudança.
2. Faça uma mudança de escopo claro. Preserve uso leve, módulos opcionais e acesso por um único executável.
3. Rode a compilação com testes. Ao alterar desenhos, confira as variações de tamanho e módulos visíveis ou ocultos.
4. Envie uma Pull Request explicando problema, mudança, validação e limitações. Inclua imagens se alterar a aparência.
5. Aguarde revisão. A proposta pode receber pedidos de ajuste antes de entrar na versão oficial.

Não adicione logos, sons, fontes ou sprites de terceiros sem origem e permissão de uso registradas. Preserve os créditos e licenças existentes. Não inclua preferências pessoais, arquivos compilados, credenciais ou históricos de conversas.

## Licenciamento

O código próprio adota MIT. As contribuições devem ser compatíveis com essa licença. O mantenedor revisa as Pull Requests antes de incorporá-las. A proteção da branch depende das configurações e do plano do repositório.
