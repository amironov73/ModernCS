### Минимальное Blazor-приложение

Приложение, создаваемое Visual Studio или dotnet CLI, к сожалению, содержит довольно много ненужных вещей вроде таблички с предсказанной погодой. Эти вещи интересны на этапе изучения Blazor, но при создании нового приложения каждый раз приходится удалять их вручную. К счастью, добрые люди создали шаблоны проектов, в которых нет ничего лишнего.

Установка шаблона:

```sh
dotnet new --install FriscoVInc.DotNet.Templates.CleanBlazor
```

Использование шаблона:

```sh
dotnet new cleanblazorserver
```

или

```sh
dotnet new cleanblazorwasm
```

Удаление шаблона:

```sh
dotnet new --uninstall FriscoVInc.DotNet.Templates.CleanBlazor
```
