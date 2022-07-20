### Как виртуализируются в WPF

В WinForms всё просто — включаем, например, в `ListBox` свойство `VirtualMode`, подписываемся на соответствующие события и вперёд. А как того же эффекта добиться в WPF? Рассказываю.

Например, мы хотим отобразить в ListBox имена 100 тысяч файлов. Если просто добавить их, как это делается обычно, контрол радостно создаст 100 тыс. контейнеров для элементов, поизмеряет их, рассчитает их положение в пространстве, короче, сделает много бессмысленной работы и сожрёт ресурсы системы. Поэтому в WPF была придумана специальная панель [`VirtualizingStackPanel`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.virtualizingstackpanel?view=netframework-4.5). Она создает контейнеры только для тех элементов, которые сейчас видит пользователь (плюс ещё немного для кеширования). Плюс она умеет переиспользовать контейнеры, чтобы не создавать-убивать их понапрасну. Короче, полезная панель. 🙂

Пример: `MainWindow.xaml`

```xaml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow" Height="250" Width="500">
     
    <StackPanel
        HorizontalAlignment="Stretch"
        VerticalAlignment="Center">
                 
        <ListBox
            Name="MyVirtualList"
            Height="200"
            VirtualizingStackPanel.IsVirtualizing="True"
            VirtualizingStackPanel.CacheLength="1"
            VirtualizingStackPanel.CacheLengthUnit="Page"
            VirtualizingStackPanel.VirtualizationMode="Recycling"
        />
 
    </StackPanel>
</Window>
```

`MainWindow.xaml.cs`

```csharp
using System.Collections.Generic;
using System.Windows;
 
namespace WpfApp1
{
    public partial class MainWindow: Window
    {
        public MainWindow()
        {
            InitializeComponent();
 
            var list = new List&l;string> (100_000);
            for (var i = 0; i < list.Capacity; i++)
            {
                list.Add ($"Файл номер {i:000000}");
            }
 
            MyVirtualList.ItemsSource = list;
        }
    }
}
```
