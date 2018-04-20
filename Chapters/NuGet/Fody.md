### Fody – инструмент для инструментации сборок

Fody – воистину великий инструмент, ибо он прост в использовании и позволяет добиться впечатляющих результатов буквально в пару щелчков мышью. Чтобы не быть голословным, покажу пример. Пусть у нас есть простое консольное приложение:

```csharp
namespace ConsoleApplication1
{
    public class SampleClass
    {
        public int Add(int left, int right)
        {
            return left + right;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SampleClass sample = new SampleClass();
            Console.WriteLine(sample.Add(2,3));
        }
    }
}
```

В менеджере NuGet подключаем пакет Virtuosity.Fody. В проекте появляется файл FodyWeavers.xml следующего содержания:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
    <Virtuosity/>
</Weavers>
```

Предпринятые шаги позволили нам добиться маленького чуда при компиляции приложения (точнее, сразу после неё) методы класса SampleClass становятся виртуальными! Вот что нам показывает ildasm:

```
.method public hidebysig newslot virtual
        instance int32  Add(int32 left,
                            int32 right) cil managed
{
  // Code size       24 (0x18)
  .maxstack  2
  .locals init ([0] int32 CS$1$0000)
  IL_0000:  nop
  IL_0001:  ldarg      left
  IL_0005:  ldarg      right
  IL_0009:  add
  IL_000a:  stloc      CS$1$0000
  IL_000e:  br         IL_0013
  IL_0013:  ldloc      CS$1$0000
  IL_0017:  ret
} // end of method SampleClass::Add
```

Fody расширяется с помощью аддинов. Вот некоторые полезные (для меня) аддины:

##### AssertMessage

Делает Assert чуть более информативным. Например, код

```csharp
Assert.AreEqual(expectedCustomer.Money, actualCustomer.Money);
```

превращается в

```csharp
Assert.AreEqual(expectedCustomer.Money, actualCustomer.Money, 
 "Assert.AreEqual(expectedCustomer.Money, actualCustomer.Money);");
```

##### AutoLazy

Переписывает

```csharp
public class MyClass
{
    [Lazy]
    public static Settings Settings
    {
        get
        {
            using (var fs = File.Open("settings.xml", FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                return (Settings)serializer.Deserialize(fs);
            }
        }
    }
}
```

в

```csharp
public class MyClass
{
    // begin - fields added by the post-compile step
    private static readonly object _syncRoot = new object();
    private static volatile Settings _settings;
    // end
 
    [Lazy]
    public static Settings Settings
    {
        get
        {
            // thread-safe double-checked locking pattern generated here
            var result = _settings;
            if (result == null)
            {
                lock(_syncRoot)
                {
                    if (_settings == null)
                    {
                        result = _settings = GetSettingsImpl();
                    }
                }
            }
            return result;
        }
    }
 
    // actual implementation copied here
    private static Settings GetSettingsImpl()
    {
        using (var fs = File.Open("settings.xml", FileMode.Open))
        {
            var serializer = new XmlSerializer(typeof(Settings));
            return (Settings)serializer.Deserialize(fs);
        }
    }
}
```

##### CryptStr

Шифрует строковые литералы в сборках, так что их становится невозможно обнаружить невооруженным глазом. Пример:

```csharp
namespace ConsoleApplication
{
    public class SampleClass
    {
        public string GetSomeString()
        {
            return "Hello";
        }
    }
 
    class Program
    {
        static void Main(string[] args)
        {
            SampleClass sample = new SampleClass();
            Console.WriteLine(sample.GetSomeString());
        }
    }
}
```

переписывается в

```csharp
.method public hidebysig instance string 
        GetSomeString() cil managed
{
  // Code size       13 (0xd)
  .maxstack  2
  .locals init ([0] string CS$1$0000)
  IL_0000:  nop
  IL_0001:  ldc.i4.0
  IL_0002:  ldc.i4.5
  IL_0003:  call       string CryptGet$PST06000002(int32,
                                                   int32)
  IL_0008:  stloc.0
  IL_0009:  br.s       IL_000b
  IL_000b:  ldloc.0
  IL_000c:  ret
} // end of method SampleClass::GetSomeString
```

Строковый литерал превращён в бессмысленный набор байт, для доступа к которому используется специально сгенерированный метод. Конечно же, опытный хакер рано или поздно добудет содержимое литерала, однако от простого просмотра сборки в шестнадцатеричном редакторе мы защитились.

##### Equals

Генерирует методы Equals, GetHashCode и оператор == для классов, снабжённых атрибутом [Equals]. Пример:

```csharp
[Equals]
public class Point
{
    public int X { get; set; }
 
    public int Y { get; set; }
 
    [IgnoreDuringEquals]
    public int Z { get; set; }
 
    [CustomEqualsInternal]
    private bool CustomLogic(Point other)
    {
        return Z == other.Z || Z == 0 || other.Z == 0;
    }
}
```

переписывается в

```csharp
public class Point : IEquatable<Point>
{
    public int X { get; set; }
 
    public int Y { get; set; }
 
    public int Z { get; set; }
 
    private bool CustomLogic(Point other)
    {
        return Z == other.Z || Z == 0 || other.Z == 0;
    }
 
    public static bool operator ==(Point left, Point right)
    {
        return object.Equals((object)left, (object)right);
    }
 
    public static bool operator !=(Point left, Point right)
    {
        return !object.Equals((object)left, (object)right);
    }
 
    private static bool EqualsInternal(Point left, Point right)
    {
        return left.X == right.X && left.Y == right.Y amp;& left.CustomLogic(right);
    }
 
    public virtual bool Equals(Point right)
    {
        return !object.ReferenceEquals((object)null, (object)right) && (object.ReferenceEquals((object)this, (object)right) || Point.EqualsInternal(this, right));
    }
 
    public override bool Equals(object right)
    {
        return !object.ReferenceEquals((object)null, right) && (object.ReferenceEquals((object)this, right) || this.GetType() == right.GetType() && Point.EqualsInternal(this, (Point)right));
    }
 
    public override int GetHashCode()
    {
        return unchecked(this.X.GetHashCode() * 397 ^ this.Y.GetHashCode());
    }
}
```

##### SwallowExceptions

Проглатывает исключения в промаркированных атрибутом [SwallowExceptions] методах. Пример:

```csharp
public class MyClass
{
    [SwallowExceptions]
    void MyMethod()
    {
      DoSomethingDangerous();
    }
}
```

превращается в

```csharp
public class MyClass
{
    void MyMethod()
    {
      try
      {
        DoSomethingDangerous();
      }
      catch (Exception exception)
      {
 
      }
    }
}
```

##### Ionad

Подменяет статические типы на указанные. Пример:

```csharp
[StaticReplacement(typeof(DateTime))]
public static class DateTimeSubstitute
{
    public static IDateTime Current { get; set; }
 
    public static DateTime Now { get { return Current.Now; } }
}
 
public void SomeMethod()
{
    var time = DateTime.Now;
    // ...
}
```

превращается в

```csharp
public void SomeMethod()
{
    var time = DateTimeSubstitute.Now;
    // ...
}
```

##### Janitor

Упрощает реализацию интерфейса IDisposable:

* Находит классы с методом Dispose
* Находит поля, реализующие интерфейс IDisposable
* Добавляет поле disposeSignaled и установку этого поля внутри Dispose
* Проверяет поле disposeSignaled в прологе метода Dispose
* Добавляет проверку disposeSignaled в неприватные методы, генерирует исключение ObjectDisposedException
* Добавляет финализатор, обрабатывающий освобождение объекта

Пример:

```csharp
public class Sample : IDisposable
{
    MemoryStream stream;
 
    public Sample()
    {
        stream = new MemoryStream();
    }
 
    public void Method()
    {
        //Some code
    }
 
    public void Dispose()
    {
        //must be empty
    }
}
```

превращается в

```csharp
public class Sample : IDisposable
{
    MemoryStream stream;
    volatile int disposeSignaled;
    bool disposed;
 
    public Sample()
    {
        stream = new MemoryStream();
    }
 
    public void Method()
    {
        ThrowIfDisposed();
        //Some code
    }
 
    void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException("TemplateClass");
        }
    }
 
    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposeSignaled, 1) != 0)
        {
            return;
        }
        if (stream != null)
        {
            stream.Dispose();
            stream = null;
        }
        disposed = true;
    }
 
}
```

##### PropertyChanged и PropertyChanging

```csharp
[ImplementPropertyChanged]
public class Person 
{        
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }
 
    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }
}
```

превращается в

```csharp
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
 
    string givenNames;
    public string GivenNames
    {
        get { return givenNames; }
        set
        {
            if (value != givenNames)
            {
                givenNames = value;
                OnPropertyChanged("GivenNames");
                OnPropertyChanged("FullName");
            }
        }
    }
 
    string familyName;
    public string FamilyName
    {
        get { return familyName; }
        set
        {
            if (value != familyName)
            {
                familyName = value;
                OnPropertyChanged("FamilyName");
                OnPropertyChanged("FullName");
            }
        }
    }
 
    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }
 
    public virtual void OnPropertyChanged(string propertyName)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

##### Scalpel
Удаляет код тестов из сборки:

* Типы, имя которых заканчивается на Tests или Mock;
* Типы, помеченные атрибутом Scalpel.RemoveAttribute
* Ссылки на сборки, перечисленные в `<Scalpel RemoveReferences=’XXX’>`

##### ToString

Генерирует метод ToString, ориентируясь на публичные свойства класса. Пример:

```csharp
[ToString]
class TestClass
{
    public int Foo { get; set; }
 
    public double Bar { get; set; }
 
    [IgnoreDuringToString]
    public string Baz { get; set; }
}
```

превращается в

```csharp
class TestClass
{
    public int Foo { get; set; }
 
    public double Bar { get; set; }
 
    public string Baz { get; set; }
 
    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture, 
            "{{T: TestClass, Foo: {0}, Bar: {1}}}",
            this.Foo,
            this.Bar);
    }
}
```

##### Undisposed

Позволяет отслеживать объекты, реализующие интерфейс IDisposable, у которых не был вызван метод Dispose. Пусть у нас есть следующий класс:

```csharp
public class Sample : IDisposable
{
    public void Dispose()
    {
    }
}
```

Fody превратит его в

```csharp
public class Sample : IDisposable
{
    public Sample()
    {
        Undisposed.DisposeTracker.Register(this);
    }
 
    public void Dispose()
    {
        Undisposed.DisposeTracker.Unregister(this);
    }
}
```

Если нам не надо отслеживать некоторый класс, достаточно добавить к нему атрибут [Undisposed.DoNotTrack].

##### Usable

Автоматически добавляет using к локальным переменным, реализующим интерфейс IDisposable. Пример:

```csharp
public void Method()
{
    var w = File.CreateText("log.txt");
    w.WriteLine("I'm a lumberjack an' I'm ok.");
}
```

переписывается в

```csharp
public void Method()
{
    using (var w = File.CreateText("log.txt"))
    {
        w.WriteLine("I'm a lumberjack an' I'm ok.");
    }
}
```

##### Visualize

Добавляет атрибуты для удобного просмотра класса в отладчике. Пример:

```csharp
public class Example1
{
    public string Name { get; set; }
    public int Number { get; set; }
}
 
public class Example2 : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        return Enumerable.Range(0, 10).GetEnumerator();
    }
 
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

переписывается в

```csharp
[DebuggerDisplay("Name = {Name} | Number = {Number}")]
public class Example1
{
    public string Name { get; set; }
    public int Number { get; set; }
}
 
[DebuggerTypeProxy(typeof(<Example2>Proxy))]
public class Example2 : IEnumerable<int>
{
    private sealed class <Example2>Proxy
    {
        private readonly Example2 original;
 
        public <Example2>Proxy(Example2 original)
        {
            this.original = original;
        }
 
        public int[] Items
        {
            get { return new List<int>(original).ToArray(); }
        }
    }
 
    public IEnumerator<int> GetEnumerator()
    {
        return Enumerable.Range(0, 10).GetEnumerator();
    }
 
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

* * *

Fody живёт в GitHub: https://github.com/Fody/Fody и в NuGet: https://www.nuget.org/packages/Fody/.

