### Managed Extensibility Framework

Managed Extensibility Framework (MEF) – небольшая библиотека, входящая в .NET Framework, начиная с версии 4.0 (т. е. Visual Studio 2010), предназначенная для упрощения создания расширяемых приложений. Она берёт на себя рутинную работу по двум направлениям: 1) обнаружение точек расширения и классов-реализаций (экземпляры которых должны быть подключены к точкам расширения), 2) сопоставление одного с другим с последующей настройкой точек расширения (грубо говоря, создание экземпляров и присвоение их свойствам-точкам расширения) по запросу приложения.

Физически MEF располагается в сборке System.ComponentModel.Composition.dll, входящей в состав фреймворка 4.0 (и, надо полагать, последующих).

MEF очень прост в использовании. Попробуем сделать традиционный пример – калькулятор, и убедимся что это просто. Для начала создаём Class Library, которую назовём CommonStuff – в ней будут общие интерфейсы, используемые в других проектах:

```csharp
namespace CommonStuff
{
    public interface ICalculator
    {
        int Add ( int x, int y );
    }
}
```

Теперь напишем расширение, которое будет реализовывать интерфейс ICalculator. Лучше всего на эту роль подходит опять же Class Library (хотя при желании мы можем разместить свои расширения где угодно, в том числе в главном исполняемом файле). Назовём её CalcExtensions и добавим в неё две ссылки: на сборку System.ComponentModel.Composition и на проект CommonStuff. Реализация расширения будет тривиальной:

```csharp
using System.ComponentModel.Composition;
using CommonStuff;
 
namespace CalcExtensions
{
    // Помечаем класс как реализующий
    // конкретный интерфейс-расширение.
    [Export(typeof(ICalculator))]
    public class SimpleCalculator
        : ICalculator
    {
        public int Add(int x, int y)
        {
            return (x + y);
        }
    }
}
```

Расширения мы будем складывать в папку Addins рядом с исполняемым файлом приложения.

Приступим теперь к созданию собственно расширяемого приложения. Создадим проект типа Console Application и назовём его MainApplication. Добавим две ссылки: на сборку System.ComponentModel.Composition и на проект CommonStuff.

```csharp
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using CommonStuff;
 
namespace MainApplication
{
    class Program
    {
        // Помечаем точку расширения.
        // Указываем, реализация какого
        // интерфейса требуется.
        [Import(typeof(ICalculator))]
        public ICalculator Calculator;
 
        static void Main()
        {
            Program program = new Program();
            program.DoSomeCalculations();
        }
 
        public void DoSomeCalculations()
        {
            // Указываем, где искать точки расширения
            // и сборки с реализациями
            AggregateCatalog catalog = new AggregateCatalog();
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog
                (
                    Assembly.GetExecutingAssembly()
                );
            catalog.Catalogs.Add (assemblyCatalog);
            string addinPath = "Addins";
            DirectoryCatalog directoryCatalog = new DirectoryCatalog
                    (
                        addinPath
                    );
            catalog.Catalogs.Add (directoryCatalog);
 
            // Создаём контейнер
            CompositionContainer container = new CompositionContainer(catalog);
 
            // Производим привязку реализаций к точке расширения
            // для своего экземпляра.
            container.ComposeParts(this);
 
            // Вызываем метод из SimpleCalculator
            int result = Calculator.Add ( 10, 5 );
            Console.WriteLine ("Result is: {0}", result);
        }
    }
}
```

Существуют, конечно же, гораздо более мощные фреймворки для создания расширяемых приложений. Но у MEF есть два главных достоинства: он входит в состав .NET Framework и его очень легко освоить.
