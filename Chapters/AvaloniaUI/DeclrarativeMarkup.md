### Декларативная разметка

Проект [Avalonia.Markup.Declarative](https://github.com/AvaloniaUI/Avalonia.Markup.Declarative) позволяет отказаться от XAML и создавать пользовательский интерфейс в коде следующим образом:

```csharp
public sealed class MainView
  : ViewBase<MainViewModel>
{
    public static IValueConverter InverseBooleanConverter { get; }
        = new FuncValueConverter<bool, bool>(b => !b);
 
    //This method is executed before View building
    protected override void OnCreated()
    {
        ViewModel = new MainViewModel();
    }
 
    //Define markup in Build method
    protected override object Build(MainViewModel vm) =>
        new Grid()
            .Cols("Auto, 100, *") // equivalent of Grid.ColumnDefintions property
            .Background(Brushes.Green) // the same as grid.Background = Brushes.Green
            .Children(
                 
                new TextBlock()
                    .Text( @vm.TextVal ), // use @ character before to Bind control's property to ViewModel's property
 
                new TextBlock()
                    .Col(1) //equivalent of Grid.SetColumn(textBlock, 1)
                    .IsVisible( @vm.HideGreeting, //bind TextBlock.IsVisible to MainViewModel.HideGreeting property
                                bindingMode: BindingMode.OneWay, // we can set Binding mode if necessery
                                converter: InverseBooleanConverter ), //Set value converter to invert value
                    .Text( "Hello Avalonia" ),
 
                new Button()
                    .Col(2) //equivalent of Grid.SetColumn(textBlock, 1)
                    // we don't actually need binding here,
                    // so just direct set to Command on view model
                    .Command(vm.ClickButtonCommand)
                    .Content("Click me") // Content = "Click me"
                    .Padding(8, 0, 0, 0) //Set left padding to 8
                    .With(ButtonStyle) //Execute LabelStyle method over TextBlock control 
            );
 
    private void ButtonStyle(Button b) => b
        .VerticalAlignment(VerticalAlignment.Center)
        .FontSize(12);
}
```

Выглядит довольно симпатично, вот только вместо установки NuGet-пакета автор предлагает нам копировать код из его проекта в свой. 🙁
