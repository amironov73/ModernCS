### Класс Freezable

```csharp
namespace System.Windows
{
    public abstract class Freezable : DependencyObject
    {
        protected Freezable();

        public Freezable Clone();

        public Freezable CloneCurrentValue();

        public Freezable GetAsFrozen();

        public Freezable GetCurrentValueAsFrozen();

        public bool CanFreeze { get; }

        public void Freeze();

        public bool IsFrozen { get; }

        public event EventHandler Changed;
    }
}
```
