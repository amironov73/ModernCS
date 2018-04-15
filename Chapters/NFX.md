### NFX UNISTACK

NFX – интересный фреймворк, предоставляющий следующий функционал:

* Контейнер приложения + внедрение зависимостей;
* Конфигурирование;
* «Большая память»: распределённые кучи, хранение миллионов объектов в памяти (без отрицательных последствий для GC);
* «Большой кеш»: кеширование объектов из распределённой «большой памяти»;
* Логирование;
* Подсистема коммуникаций (альтернатива WCF);
* Подсистема безопасности с пользователями, мандатами, ролями, разрешениями;
* JSON;
* Сверхэффективная двоичная сериализация;
* Синтаксический разбор и обработка текста (формальные грамматики);
* Уровень доступа к базам данных;
* и многое другое.
 
Проект обитает на ГитХабе: https://github.com/aumcode/nfx.

Изо всего вышеперечисленного меня больше всего заинтересовала сверхбыстрая и сверхкомпактная двоичная сериализация (класс `NFX.Serialization.Slim.SlimSerialzier`). Авторы позиционируют свой движок для следующих случаев:

* нужна большая скорость (сотни тысяч объектов в секунду);
* требуется экономить объём передаваемых данных;
* разметка классов для сериализации невозможна (например, мы имеем дело с чужими классами или у нас слишком много классов, чтобы размечать их).
 
`SlimSerializer` поддерживает:

* много примитивных (но нужных!) структур: DateTime, TimeSpan, GUID и т. п.;
* очень нужные массивы вроде byte[], char[], string[] и т. п.;
* классы и структуры с read-only полями;
*custom-сериализация через ISerializable и т. п.;
* каскадно-вложенная сериализация (например, во время custom-сериализации можно вызвать SlimSerializer для какого-нибудь поля);
* нормализует графы любой сложности и вложенности;
* обнаружение повреждённых данных при десериализации.
 

Есть ограничения:

* Не поддерживается версионность объектов;
* Не сериализуются делегаты;
* Сериализованные данные никто, кроме SlimSerializer, не понимает.
 
Пользоваться сериализатором от NFX так же легко, как и стандартным:

```csharp
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
 
using NFX.Serialization.Slim;
 
namespace ConsoleApplication1
{
    [Serializable]
    class MyClass
    {
        public string Name { get; set; }
 
        public int Age { get; set; }
 
        public string Address { get; set; }
    }
 
    class Program
    {
        static void Main()
        {
            MyClass[] customers = 
            {
                new MyClass
                {
                    Name = "Alexey Mironov",
                    Age = 40,
                    Address = "Russia, Irkutsk"
                }, 
                new MyClass
                {
                    Name = "John Doe",
                    Age = 100,
                    Address = "Unknown"
                },
                // Ещё 1000 клиентов
            };
 
            // SlimSerializer
            TypeRegistry types = new TypeRegistry(new []{typeof(MyClass)});
            SlimSerializer slimSerializer = new SlimSerializer(types);
            using (FileStream stream = File.Create("slim.data"))
            {
                slimSerializer.Serialize(stream, customers);
            }
 
            // Стандартный
            BinaryFormatter standardSerializer = new BinaryFormatter();
            using (FileStream stream = File.Create("standard.data"))
            {
                standardSerializer.Serialize(stream, customers);
            }
        }
    }
}
```

Обратите внимание: для SlimSerializer даже необязательно помечать класс как [Serializable], он сохранит класс и так. А вот стандартный BinaryFormatter упадёт с исключением, если забыть [Serializable] у класса или у одного из его членов.

Статья на Хабре: http://habrahabr.ru/post/257247/