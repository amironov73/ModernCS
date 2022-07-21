### TreeDataGrid

[TreeDataGrid](https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid) – контрол для Avalonia UI, совмещающий дерево и таблицу. Выглядит так:

![TreeDataGrid](img/tree-data-grid-1.png)

Кроме установки NuGet-пакета `Avalonia.Controls.TreeDataGrid`, необходимо добавить соответствующие стили в `App.axaml`:

```xaml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="AvaloniaApplication.App">
  <Application.Styles>
    <FluentTheme/>
    <StyleInclude Source="avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml"/>
  </Application.Styles>
</Application>
```

Пример использования: `MainWindow.axaml`

```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:Avalonia.NETCoreMVVMApp1.ViewModels"
        x:Class="Avalonia.NETCoreMVVMApp1.Views.MainWindow"
        Icon="/Assets/avalonia-logo.ico"
        Title="Avalonia.NETCoreMVVMApp1">
 
    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>
 
    <TreeDataGrid Source="{Binding Source}"/>
 
</Window>
```

`MainWindowViewModel.cs`

```csharp
using System.Collections.ObjectModel;
 
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
 
namespace Avalonia.NETCoreMVVMApp1.ViewModels
{
    public class MainWindowViewModel
        : ViewModelBase
    {
        private ObservableCollection<Person> _people = new ()
        {
            new Person
            {
                FirstName = "Eleanor",
                LastName = "Pope",
                Age = 32,
                Children =
                {
                    new Person { FirstName = "Marcel", LastName = "Gutierrez", Age = 4 },
                }
            },
            new Person
            {
                FirstName = "Jeremy",
                LastName = "Navarro",
                Age = 74,
                Children =
                {
                    new Person
                    {
                        FirstName = "Jane",
                        LastName = "Navarro",
                        Age = 42,
                        Children =
                        {
                            new Person { FirstName = "Lailah ", LastName = "Velazquez", Age = 16 }
                        }
                    },
                }
            },
            new Person { FirstName = "Jazmine", LastName = "Schroeder", Age = 52 },
        };
 
        public MainWindowViewModel()
        {
            Source = new HierarchicalTreeDataGridSource<Person> (_people)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<Person> 
                        (
                            new TextColumn<Person, string> ("First Name", x => x.FirstName),
                            x => x.Children
                        ),
                    new TextColumn<Person, string> ("Last Name", x => x.LastName),
                    new TextColumn<Person, int> ("Age", x => x.Age),
                },
            };
        }
 
        public HierarchicalTreeDataGridSource<Person> Source { get; }
    }
}
```
