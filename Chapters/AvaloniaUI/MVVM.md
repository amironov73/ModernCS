### MVVM

Avalonia поддерживает шаблон проектирования MVVM — "Model-View-ViewModel". Для этого она предоставляет `binding` - связывание элементов пользовательского интерфейса со свойствами контекста данных - некоторого объекта, содержащего все данные, необходимые для отображения пользовательского интерфейса.

Пример. Во вновь созданном приложении создадим класс `MainWindowViewModel`, который будет содержать данные для отображения, а именно - текст кнопки.

```csharp
using System.ComponentModel;

namespace AvaloniaApp1;

public class MainWindowViewModel
    : INotifyPropertyChanged
{
    private int _counter = 0;
    private string _buttonText = "Нажми меня!";

    public string ButtonText
    {
        get => _buttonText;

        set
        {
            _buttonText = value;
            PropertyChanged?.Invoke 
                (
                    this,
                    new PropertyChangedEventArgs (nameof (ButtonText))
                );
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void ButtonClicked()
    {
        ButtonText = $"Кнопка нажата {++_counter} раз";
    }
}
```

Теперь отредактируем разметку главного окна `MainWindow.axaml`

```csharp
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Width="300"
        Height="150"
        x:Class="AvaloniaApp1.MainWindow"
        Title="AvaloniaApp1"
        >
    
    <Button 
        Name="_button"
        HorizontalAlignment="Center"
        Content="{Binding ButtonText}"
        Command="{Binding ButtonClicked}"
        />
</Window>
```

Наконец, в code-behind `MainWindow.axaml.cs` создадим контекст данных для экземпляра класса `MainWindow`:

```csharp
using Avalonia.Controls;

namespace AvaloniaApp1;

public partial class MainWindow 
    : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
```

Теперь при нажатии на кнопку текст на ней будет изменяться в соответствии с количеством нажатий.
