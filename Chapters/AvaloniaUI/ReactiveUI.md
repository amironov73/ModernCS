### Правильные свойства с помощью ReactiveUI

Как испокон веков реализуется интерфейс `INotifyPropertyChanged`? Примерно так:

```csharp
using System.ComponentModel;
 
public sealed class OldPerson
    : INotifyPropertyChanged
{
    private string? _firstName;
 
    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (value != _firstName)
            {
                _firstName = value;
                OnPropertyChanged (nameof (FirstName));
                FullNameChanged();
            }
        }
    }
 
    private string? _lastName;
 
    public string? LastName
    {
        get => _lastName;
        set
        {
            if (value != _lastName)
            {
                _lastName = value;
                OnPropertyChanged (nameof (LastName));
                FullNameChanged();
            }
        }
    }
 
    private string _fullName = "";
 
    public string FullName => _fullName;
 
    public event PropertyChangedEventHandler? PropertyChanged;
 
    private void OnPropertyChanged
        (
            string propertyName
        )
    {
        PropertyChanged?.Invoke 
            (
                this, 
                new PropertyChangedEventArgs (propertyName)
            );
    }
 
    private void FullNameChanged()
    {
        _fullName = $"{_firstName} {_lastName}";
        OnPropertyChanged (nameof (FullName));
    }
 
    public override string ToString() => FullName;
}
```

Слишком много однообразной писанины, мы обязательно где-нибудь ошибемся и замучаемся потом искать ошибку. Поэтому делать надо так: подключаем к своему проекту два NuGet-пакета: `ReactiveUI` и `ReactiveUI.Fody` и переписываем класс следующим образом:

```csharp
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
 
public sealed class NewPerson
    : ReactiveObject
{
    public NewPerson()
    {
        this.WhenAnyValue (x => x.FirstName, x => x.LastName)
            .Select (t => $"{t.Item1} {t.Item2}")
            .ToPropertyEx (this, vm => vm.FullName);
    }
 
    [Reactive]
    public string? FirstName { get; set; }
 
    [Reactive]
    public string? LastName { get; set; }
 
    [ObservableAsProperty]
    public string FullName => "";
 
    public override string ToString() => FullName;
}
```

Теперь зависимость свойства `FullName` от свойств `FirstName` и `LastName` задана ровно в одном месте – в конструкторе, а остальную работу за нас сделал Fody. Красота!
