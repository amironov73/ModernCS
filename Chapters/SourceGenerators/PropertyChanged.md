### PropertyChanged

Как известно, с незапамятных времен в .NET существует интерфейс INotifyPropertyChanged. Он несложный, однако, его реализация заставляет нас писать довольно много однообразных лишних букв:

```c#
public class Person
    : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string? _firstName;
 
    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (!EqualityComparer<string>.Default.Equals (_firstName, value))
            {
                _firstName = value;
                OnPropertyChanged (new PropertyChangedEventArgs (nameof (FirstName)));
            }
        }
    }
 
    private string? _lastName;
 
    public string? LasttName
    {
        get => _lasttName;
        set
        {
            if (!EqualityComparer<string>.Default.Equals (_lastName, value))
            {
                _lastName = value;
                OnPropertyChanged (new PropertyChangedEventArgs (nameof (LastName)));
            }
        }
    }
 
    protected virtual void OnPropertyChanged (PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke (this, args);
    }
}
```

В этом примере бо́льшая часть кода — служебная, и его написание повергает C#-программиста в уныние.

Теперь, с появлением в C# генераторов кода, нет нужды раз за разом писать служебный код. «Вкалывают роботы — счастлив человек!» :)

Подключаем к своему проекту пакет PropertyChanged.SourceGenerator и пишем в разы меньше:

```c#
using PropertyChanged.SourceGenerator;

public partial class Person
    : INotifyPropertyChanged
{
    [Notify] private string? _firstName;

    [Notify] private string? _lastName;
 
    public override string ToString() => $"{_firstName} {_lastName}";
}
```

Остальное за нас напишет генератор. Нам же остается лишь радостно применять классы в коде

```c#
var person = new Person();
person.PropertyChanged += (sender, eventArgs) =>
{
    Console.WriteLine ($"Property changed: {eventArgs.PropertyName}");
    Console.WriteLine ($"Full name now: {sender}");
};

person.FirstName = "Alexey";
person.LastName = "Mironov";
```
