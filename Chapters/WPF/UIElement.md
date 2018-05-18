### Класс UIElement

```csharp
namespace System.Windows
{
    public partial class UIElement : Visual
    {
        public bool AllowDrop { get; set; }

        public Size DesiredSize { get; }

        public bool IsMeasureValid { get; }

        public bool IsArrangeValid { get; }

        public Size RenderSize { get; set; }

        public Transform RenderTransform { get; set; }

        public Point RenderTransformOrigin { get; set; }

        public bool IsMouseDirectlyOver { get; }

        public bool IsMouseOver { get; }

        public bool IsStylusOver { get; }

        public bool IsKeyboardFocusWithin { get; }

        public bool IsMouseCaptured { get; }

        public bool IsMouseCaptureWithin { get; }

        public bool IsStylusDirectlyOver { get; }

        public bool IsStylusCaptured { get; }

        public bool IsStylusCaptureWithin { get; }

        public bool IsKeyboardFocused { get; }

        public bool IsInputMethodEnabled { get; }

        public double Opacity { get; set; }

        public Brush OpacityMask { get; set; }

        public BitmapEffect BitmapEffect { get; set; }

        public Effect Effect { get; set; }

        public string Uid { get; set; }

        public Visibility Visibility { get; set; }

        public bool ClipToBounds { get; set; }

        public Geometry Clip { get; set; }

        public bool SnapsToDevicePixels { get; set; }

        public bool IsFocused { get; }

        public bool IsEnabled { get; set; }

        public bool IsVisible { get; }

        public bool Focusable { get; set; }

        public bool AreAnyTouchesOver { get; }

        public bool AreAnyTouchesDirectlyOver { get; }

        public bool AreAnyTouchesCapturedWithin { get; }

        public bool AreAnyTouchesCaptured { get;}

        public IEnumerable<TouchDevice> TouchesCaptured { get; }

        public IEnumerable<TouchDevice> TouchesCapturedWithin { get; }

        public IEnumerable<TouchDevice> TouchesOver { get; }

        public IEnumerable<TouchDevice> TouchesDirectlyOver { get; }

        public void InvalidateMeasure();

        public void InvalidateArrange();

        public void InvalidateVisual();

        public void Measure(Size availableSize);

        public void Arrange(Rect finalRect);

        public void UpdateLayout();

        public Point TranslatePoint(Point point, UIElement relativeTo);

        public IInputElement InputHitTest(Point point);

        public bool CaptureMouse();

        public void ReleaseMouseCapture();

        public bool CaptureStylus();

        public void ReleaseStylusCapture();

        public bool Focus();

        public virtual bool MoveFocus(TraversalRequest request);

        public virtual DependencyObject PredictFocus(FocusNavigationDirection direction);

        public bool CaptureTouch(TouchDevice touchDevice);

        public bool ReleaseTouchCapture(TouchDevice touchDevice);

        public void ReleaseAllTouchCaptures();

        public event EventHandler LayoutUpdated;

        public event RoutedEventHandler GotFocus;

        public event RoutedEventHandler LostFocus;
    }
}
```
