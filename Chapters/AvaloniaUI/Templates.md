## Шаблоны

Чтобы не создавать каждый раз приложение вручную, можно установить соответствующие шаблоны

```sh
dotnet new -i Avalonia.Templates
```

После этого у нас появятся следующие шаблоны:

| Шаблон                                    | Обозначение               | Языки  |
|-------------------------------------------|---------------------------|--------|
| Avalonia .NET Core App                    | avalonia.app              | C#, F# |
| Avalonia .NET Core MVVM App               | avalonia.mvvm             | C#, F# |
| Avalonia Cross Platform Application       | avalonia.xplat            | C#     |
| Avalonia Resource Dictionary              | avalonia.resource         |        |
| Avalonia Styles                           | avalonia.styles           |        |
| Avalonia TemplatedControl                 | avalonia.templatedcontrol | C#     |
| Avalonia UserControl                      | avalonia.usercontrol      | C#, F# |
| Avalonia Window                           | avalonia.window           | C#, F# |

По умолчанию во всех шаблонах используется язык C# (где это применимо).

После установки шаблонов приложения и компоненты можно создавать из командной строки

```sh
dotnet new avalonia.app -o MyApp
cd MyApp
dotnet new avalonia.window -o SecondWindow
dotnet new avalonia.window -o ThirdWindow
```
