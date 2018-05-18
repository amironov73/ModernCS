### Класс FrameworkElement

```csharp
namespace System.Windows
{
    public partial class FrameworkElement : UIElement
    {
        public Style Style { get; set; }

        public bool OverridesDefaultStyle { get; set; }

        public bool UseLayoutRounding { get; set; }

        public bool ApplyTemplate();

        public void BeginStoryboard(Storyboard storyboard);

        public void BeginStoryboard(Storyboard storyboard, HandoffBehavior handoffBehavior);

        public void BeginStoryboard(Storyboard storyboard, HandoffBehavior handoffBehavior, bool isControllable);

        public TriggerCollection Triggers { get; }

        public DependencyObject TemplatedParent { get; }

        public ResourceDictionary Resources { get; }

        public object FindResource(object resourceKey);

        public object TryFindResource(object resourceKey);

        public void SetResourceReference(
            DependencyProperty dp,
            object             name);

        public object DataContext { get; set; }

        public BindingExpression GetBindingExpression(DependencyProperty dp);

        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding);

        public BindingExpression SetBinding(DependencyProperty dp, string path);

        public BindingGroup BindingGroup { get; set; }

        public XmlLanguage Language { get; set; }

        public string Name { get; set; }

        public object Tag { get; set; }

        public InputScope InputScope { get; set; }

        public void BringIntoView();

        public void BringIntoView(Rect targetRectangle);

        public event SizeChangedEventHandler SizeChanged;

        public double ActualWidth { get; }

        public double ActualHeight { get; set; }

        public Transform LayoutTransform { get; set; }

        public double Width { get; set; }

        public double MinWidth { get; set; }

        public double MaxWidth { get; set; }

        public double Height { get; set; }

        public double MinHeight { get; set; }

        public double MaxHeight { get; set; }

        public FlowDirection FlowDirection { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public System.Windows.Input.Cursor Cursor { get; set; }

        public bool ForceCursor { get; set; }

        public event EventHandler Initialized;

        public event RoutedEventHandler Loaded;

        public event RoutedEventHandler Unloaded;
    }
}
```

