### Что такое команда?

Команда - некоторая привязанная (bound) сущность, позволяющая разделить логику и пользовательский интерфейс.

Если рассматривать команды более подробно, то они представляют из себя следующее:

* Команды представляют собой объекты, реализующие интерфейс `ICommand`.
* Обычно команды связанны с какой либо функцией `Action`.
* Элементы пользовательского интерфейса привязываются к командам — когда интерфейс активируется пользователем, то выполняется команда — вызывается соответствующая `Action`.
* Команды знают, включены ли они или нет.
* Функции могут отключать команды, это автоматическое отключение всех пользовательских элементов ассоциированных с ней.

Существует множество различных применений команд. Например, для создания асинхронных функций, обеспечивающих логику, которая может быть проверена с/без помощи использования пользовательского интерфейса.

```csharp
using ReactiveUI;

var simpleCommand = ReactiveCommand.Create 
    (
        () => Console.WriteLine ("Simple command")
    );

simpleCommand.Execute().Subscribe();

var numberCommand = ReactiveCommand.Create<int> 
    (
        number => Console.WriteLine (number) 
    );
    
numberCommand.Execute (42).Subscribe();
```

### Реактивные команды

Подключаем к приложению пакеты `ReactiveUI`, `ReactiveUI.Fody` и `Avalonia.Reactive` и организуем работу с командами правильно. Главное - не забыть дописать строчку `UseReactiveUI()` в создание приложения!

![Окно с кнопками](img/reactive-command.png)

```csharp
using Avalonia;

using System;
using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Avalonia.Themes.Fluent;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaloniaApplication7;

internal static class Program
{
    [STAThread]
    public static void Main (string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime (args);

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<MyApp>()
            .UseReactiveUI()
            .UsePlatformDetect()
            .LogToTrace();
}

internal sealed class MyApp
    : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        var baseUri = new Uri ("avares://Avalonia.ThemeManager/Styles");
        Styles.Add (new FluentTheme (baseUri) { Mode = FluentThemeMode.Light });

        if (ApplicationLifetime is ClassicDesktopStyleApplicationLifetime classic)
        {
            classic.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

internal sealed class MyModel
    : ReactiveObject
{
    [Reactive] public string? SomeName { get; set; }

    [Reactive] public string? SomePassword { get; set; }
}

internal sealed class MainWindow
    : ReactiveWindow<MyModel>
{
    public MainWindow()
    { 
        Title = "Это просто окно";
        Width = 400;
        Height = 250;

        DataContext = new MyModel();

        Content = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 5,
            Margin = new Thickness (10),
            Children =
            {
                new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Content = "Вводите логин и пароль"
                },
                
                new TextBox
                {
                    Width = 200,
                    [!TextBox.TextProperty] = new Binding (nameof (MyModel.SomeName))
                },
                
                new TextBox
                {
                    Width = 200,
                    [!TextBox.TextProperty] = new Binding (nameof (MyModel.SomePassword))
                },
                
                new Button
                {
                    IsDefault = true,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = "OK - нажми меня!",
                    Command = ReactiveCommand.Create 
                        (
                            OkClicked,
                            OkEnabled()
                        )
                },
                
                new Button
                {
                    IsCancel = true,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = "Отмена - меня тоже!",
                    Command = ReactiveCommand.Create (CancelClicked)
                }
            }
        };
    }

    private IObservable<bool> OkEnabled()
    {
        return ViewModel!.WhenAnyValue 
            (
                x => x.SomeName,
                y => y.SomePassword,
                (x, y) => !string.IsNullOrEmpty (x) 
                          && !string.IsNullOrEmpty (y)
            );
    }

    private void OkClicked()
    {
        Debug.WriteLine ("OK");
        Close (true);
    }
    
    private void CancelClicked()
    {
        Debug.WriteLine ("Cancel");
        Close (false);
    }
}
```

Бывает так, что в обработчике команды необходимо выполнить нечто асинхронное, например, показать окно диалога. Вот как это делается:

```csharp
using System;
using System.Threading.Tasks;

using AvaLib;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.ReactiveUI;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaApp;

public sealed class FirstModel
    : ReactiveObject
{
    [Reactive] 
    public string Message { get; set; } = "Некоторое сообщение";

    [Reactive] 
    public string Log { get; set; } = null!;

    public void FirstClick()
    {
        Log = nameof (FirstClick) + Environment.NewLine;
    }

    public async Task SecondClick
        (
            Window window
        )
    {
        var secondWindow = new SecondWindow();
        var model = secondWindow.ViewModel!;
        var result = await secondWindow.ShowDialog&lt;object&gt; (window);
        Log = $"""
        Result: {result}
        Name: {model.SomeName}
        Password: {model.SomePassword}
        """;
    }
}

public sealed class FirstWindow
    : ReactiveWindow&lt;FirstModel&gt;
{
    public FirstWindow()
    {
        Title = "Это первое окно";
        Width = MinWidth = 400;
        Height = MinHeight = 250;

        DataContext = new FirstModel();

        Content = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 5,
            Margin = new Thickness (10),
            
            Children =
            {
                new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    [!ContentProperty] = new Binding(nameof (FirstModel.Message))
                },
                
                new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = "Нажми меня",
                    Command = ReactiveCommand.Create (ViewModel!.FirstClick)
                },
                
                new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = "Открыть второе окно",
                    Command = ReactiveCommand.CreateFromTask
                        (
                            () =&gt; ViewModel!.SecondClick (this)
                        )
                },
                
                new TextBox
                {
                    Width = 200,
                    Height = 120,
                    [!TextBox.TextProperty] = new Binding (nameof (FirstModel.Log))
                }
            }
        };
    }
}
```
