### Класс DependencyObject

```csharp
namespace System.Windows
{
    public class DependencyObject : DispatcherObject
    {
        public DependencyObjectType DependencyObjectType { get; }

        public bool IsSealed { get; }

        public object GetValue(DependencyProperty dp);

        public void SetValue(DependencyProperty dp, object value);

        public void SetCurrentValue(DependencyProperty dp, object value);

        public void SetValue(DependencyPropertyKey key, object value);

        public void ClearValue(DependencyProperty dp);

        public void ClearValue(DependencyPropertyKey key);

        public void CoerceValue(DependencyProperty dp);

        public void InvalidateProperty(DependencyProperty dp);

        public object ReadLocalValue(DependencyProperty dp);

        public LocalValueEnumerator GetLocalValueEnumerator();
    }

    public class DependencyObjectType
    {
        public int Id { get; }

        public Type SystemType { get; }

        public DependencyObjectType BaseType { get; }

        public string Name { get; }

        public bool IsInstanceOfType(DependencyObject dependencyObject);

        public bool IsSubclassOf(DependencyObjectType dependencyObjectType);

        public static DependencyObjectType FromSystemType(Type systemType);
    }

    public sealed class DependencyProperty
    {
        public string Name { get; }

        public Type PropertyType { get; }

        public Type OwnerType { get; }

        public PropertyMetadata DefaultMetadata { get; }

        public ValidateValueCallback ValidateValueCallback { get; }

        public int GlobalIndex { get; }

        public bool ReadOnly { get; }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType);

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata);

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback);

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata);

        public static DependencyPropertyKey RegisterReadOnly(
            string name,
            Type propertyType,
            Type ownerType,
            PropertyMetadata typeMetadata,
            ValidateValueCallback validateValueCallback);

        public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata);

        public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback);

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType);

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata);

        public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback);

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata);

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata, DependencyPropertyKey key);

        public PropertyMetadata GetMetadata(Type forType);

        public PropertyMetadata GetMetadata(DependencyObject dependencyObject);

        public PropertyMetadata GetMetadata(DependencyObjectType dependencyObjectType);

        public DependencyProperty AddOwner(Type ownerType);

        public DependencyProperty AddOwner(Type ownerType, PropertyMetadata typeMetadata);

        public bool IsValidType(object value);

        public bool IsValidValue(object value);
    }
}
```
