### Класс Keyboard

```csharp
namespace System.Windows.Input
{
    public static class Keyboard
    {
        public static readonly RoutedEvent PreviewKeyDownEvent;

        public static void AddPreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler);

        public static void RemovePreviewKeyDownHandler(DependencyObject element, KeyEventHandler handler);

        public static readonly RoutedEvent KeyDownEvent;

        public static void AddKeyDownHandler(DependencyObject element, KeyEventHandler handler);

        public static void RemoveKeyDownHandler(DependencyObject element, KeyEventHandler handler);

        public static readonly RoutedEvent PreviewKeyUpEvent;

        public static void AddPreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler);

        public static void RemovePreviewKeyUpHandler(DependencyObject element, KeyEventHandler handler);

        public static readonly RoutedEvent KeyUpEvent;

        public static void AddKeyUpHandler(DependencyObject element, KeyEventHandler handler);

        public static void RemoveKeyUpHandler(DependencyObject element, KeyEventHandler handler);

        public static readonly RoutedEvent PreviewGotKeyboardFocusEvent;

        public static void AddPreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);

        public static void RemovePreviewGotKeyboardFocusHandler(DependencyObject element, KeyboardFocusChangedEventHandler handler);

        public static KeyStates GetKeyStates(Key key);

        public static bool IsKeyToggled(Key key);

        public static bool IsKeyUp(Key key);

        public static bool IsKeyDown(Key key);

        public static ModifierKeys Modifiers;

        public static RestoreFocusMode DefaultRestoreFocusMode { get; set; }

        public static IInputElement Focus(IInputElement element);

        public static void ClearFocus();

        public static IInputElement FocusedElement { get; }
    }
}
```
