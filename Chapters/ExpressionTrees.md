### Expression Trees

Пространство имен [System.Linq.Expressions](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions?view=net-5.0) позволяет строить так называемые expression trees (деревья выражений) во время выполнения программы. Например, такое дерево можно построить по результатам разбора пользовательского ввода (вроде «`def func(x): return x * 2`»). Как будет видно из примера, делать это не в пример проще, чем через старый недобрый System.Reflection.Emit (хотя бы из-за того, что не нужно заморачиваться со стеком). Кроме того, динамический созданный код подвластен сборщику мусора, что очень радует.

Всего предоставляется 56 разных элементов, из которых мы и должны выстроить свое дерево. Затем дерево можно скомпилировать в код и выполнять с той же скоростью, что и обычный код, созданный компилятором C#.

Для начала простейший пример:
```csharp
using System;
using System.Linq.Expressions;
  
class Program
{
    static void Main()
    {
        Expression<Func<int, int>> expression = x => (x + 1) * 2;
        Func<int, int> func = expression.Compile();
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(func(i));
        }
    }
}
```

Краткое описание доступных выражений

Тип      | Краткое описание
---------|----------------
**Основные** |
Expression | Абстрактный тип, являющийся базовым для остальных типов выражений. Также содержит полезную статические методы, через которые создаются необходимые типы выражений
Expression<TDelegate> | Представляет дерево выражений для лямбды
**Простые действия** |
BinaryExpression | Тип выражений для бинарных операторов (+, — и т. д.)
UnaryExpression | Унарные операторы(+, -), а также выражение throw
ConstantExpression | Константа — просто некий экземпляр
ParameterExpression | Параметр функции
MethodCallExpression | Представляет вызов метода, принимает MethodInfo
IndexExpression | Индексатор
BlockExpression | Блок, содержащий последовательность выражений. В нем может быть объявлена переменная
**Управление потоком выполнения** | 
ConditionalExpression | Выражение условного оператора — if-else
LabelTarget | Лейбл для переходов goto
LabelExpression | Выражение лейбла, которое можно поместить в структуру дерева выражений для последующего скачка к нему. Создается при помощи LabelTarget. Если на него прыгнули, получит значение соответствующего GotoExpression, в противном случае — значение по умолчанию. Можно использовать с void, тогда присвоения не будет.
GotoExpression | Собственно безусловный переход. Может быть разных типов. (в т. ч. «break»)
LoopExpression | Бесконечный цикл, выходится с помощью «break»
SwitchCase | Используется для представления элементов SwitchExpression
SwitchExpression | Собственно обычный switch/case
TryExpression | Тип выражения для представления try/catch/finally/fault блока
CatchBlock | Тип, не являющийся выражением, но содержащий их внутри
**Инициализация** |
ElementInit | Инициализатор одного элемента в IEnumerable. Используется в ListInitExpression
ListInitExpression | Вызов конструктора + инициализация коллекции элементами
DefaultExpression | Значение по умолчанию для типа или пустое выражение
NewArrayExpression | Инициализация нового массива + инициализация элементов
NewExpression | Вызов конструктора
**Действия с полями/свойствами** | 
MemberAssignment | Присваивание полю или свойству объекта
MemberBinding | Предоставляет базовый класс, из которого наследуются классы, представляющие биндинги, которые используются для инициализации членов вновь созданного объекта
MemberExpression | Доступ к полю/свойству
MemberInitExpression | Вызов конструктора и инициализация полей созданного объекта
MemberListBinding | Инициализация свойства/поля типа коллекции
MemberMemberBinding | Инициализация поля/свойства обьекта, который сам является полем/свойством
**Прочее** |
LambdaExpression | Лямбда
InvocationExpression | Применяет делегатное или лямбда-выражение к списку выражений аргументов
DebugInfoExpression | Задает или очищает точку останова для отладочной информации. Это позволяет отладчику выделять правильный исходный код при отладке
SymbolDocumentInfo | Хранит информацию об символах отладки для исходного файла, в частности имя файла и уникальный идентификатор языка.
DynamicExpression | Представляет динамическую операцию (разобрано выше)
RuntimeVariablesExpression | Выражение представляющее права чтения/записи для переменных времени выполнения
TypeBinaryExpression | Проверяет, является ли объект таким типом (is)


Теперь сгенерируем простую лямбду `(a, b) => a + b` вручную:
```csharp
// сначала опишем параметры лямбды
var parameterA = Expression.Parameter(typeof(int), "a"); // первый параметр
var parameterB = Expression.Parameter(typeof(int), "b"); // второй параметр
var expr = Expression.Lambda<Func<int,int,int>>
    (
        Expression.Add(parameterA, parameterB), // тело лямбды
        parameterA, // её параметры: первый
        parameterB  // и второй
    );
Console.WriteLine(expression); // (a, b) => a + b
var func = expr.Compile(); // получаем скомпилированный метод
 
// вызываем делегат
Console.WriteLine(func(2, 3)); // 5
```

Аналогично делается доступ к полям/свойствам объектов:
```csharp
using System;
using System.Linq.Expressions;
 
// какой-то класс, к свойствам которого мы хотим получить доступ
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
 
class Program
{
    static void Main()
    {
        var parameterP = Expression.Parameter(typeof(Person), "p");
        var expression = Expression.Lambda<Func<Person, string>>
            (
                Expression.Property(parameterP,"Name"), // тело лямбды
                parameterP // её параметр
            );
        Console.WriteLine(expression); // p => p.Name
        var func = expression.Compile();
 
        // пробуем обратиться к свойству объекта
        Person person = new Person { Name = "Старик Хоттабыч", Age = 8942 };
        Console.WriteLine(func(person)); // Старик Хоттабыч
    }
}
```

Теперь вызовем какой-нибудь метод (обратите внимание, никаких препятствий для вызова приватных методов чужих классов нет!):
```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;
 
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
 
    // какой-то метод, вычисляющий нечто очень нужное нам
    static string SomeMethod(string text)
    {
        return text.Substring(7);
    }
}
 
class Program
{
    static void Main()
    {
        var parameterP = Expression.Parameter(typeof(Person), "p");
        var methodInfo = typeof(Person).GetMethod // получаем метод
            (
                "SomeMethod",
                BindingFlags.Static|BindingFlags.NonPublic
            );
        var expression = Expression.Lambda<Func<Person, string>>
            (
                Expression.Call
                    ( 
                        methodInfo, // вызываемый метод
                        Expression.Property(parameterP, "Name") // аргумент
                    ),
                parameterP
            );
        Console.WriteLine(expression); // p => SomeMethod(p.Name)
        var func = expression.Compile();
 
        // пробуем обратиться к свойству объекта
        Person person = new Person { Name = "Старик Хоттабыч", Age = 8942 };
        Console.WriteLine(func(person)); // Хоттабыч
    }
}
```

Естественно, нет никаких проблем со встраиванием вызова уже имеющейся лямбды в дерево:
```csharp
using System;
using System.Linq.Expressions;
 
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
 
class Program
{
    static void Main()
    {
        // пусть у нас уже есть какая-то лямбда, неважно откуда полученная
        Expression<Func<Person, int>> lambda = p => p.Name.Length / 2 + 1;
 
        // встроим её вызов (Invoke) в нашу лямбду
        var parameterP = Expression.Parameter(typeof(Person), "p");
        var expression = Expression.Lambda<Func<Person, int>>
            (
                Expression.Multiply
                    (
                        Expression.Invoke(lambda, parameterP), // вызов
                        Expression.Property(parameterP, "Age")
                    ),
                parameterP
            );
        Console.WriteLine(expression);
        // p => (((p.Name.Length / 2) + 1) * p.Age)
 
        var func = expression.Compile();
        Person person = new Person { Name = "Старик Хоттабыч", Age = 8942 };
        Console.WriteLine(func(person)); // 71536
    }
}
```

Как установить значение свойства прямым вызовом метода-сеттера:
```csharp
using System;
using System.Linq;
using System.Linq.Expressions;
 
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
 
class Program
{
    static void Main()
    {
        var personParameter = Expression.Parameter(typeof(Person), "person");
        var nameParameter = Expression.Parameter(typeof(string), "name");
        var propertyInfo = typeof(Person).GetProperty("Name");
 
        // получаем акцессоры (геттер и сеттер) данного свойства
        var accessors = propertyInfo.GetAccessors();
 
        // выбираем тот, который ничего не возвращает, или вылетаем с ошибкой
        var methodInfo = accessors.First(m => m.ReturnType == typeof(void));
        var body = Expression.Call
            (
                personParameter,
                methodInfo,
                nameParameter
            );
        var expression = Expression.Lambda<Action<Person, string>>
            (
                body,
                personParameter,
                nameParameter
            );
        Console.WriteLine(expression);
        // (person, name) => person.set_Name(name)
 
        var setter = expression.Compile();
 
        Person person = new Person();
        setter(person, "Старик Хоттабыч");
        Console.WriteLine(person.Name);
    }
}
```

Можно обойтись простым присвоением значения свойству/полю:
```csharp
var personParameter = Expression.Parameter(typeof(Person), "person");
var nameParameter = Expression.Parameter(typeof(string), "name");
var access = Expression.PropertyOrField(personParameter, "Name");
var expression = Expression.Lambda<Action<Person, string>>
    (
        Expression.Assign(access, nameParameter),
        personParameter,
        nameParameter
    );
Console.WriteLine(expression); // (person, name) => (person.Name = name)
var setter = expression.Compile();
```

Как проинициализировать объект со свойствами:
```csharp
using System;
using System.Linq.Expressions;
 
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
 
class Program
{
    static void Main()
    {
        var namePar = Expression.Parameter(typeof(string), "name");
        var agePar = Expression.Parameter(typeof(int), "age");
        var body = Expression.MemberInit // инициализация
            (
                Expression.New(typeof(Person)), // вызов конструктора
                Expression.Bind // первое свойство
                    (
                        typeof(Person).GetProperty("Name"),
                        namePar // чем инициализируем
                    ),
                Expression.Bind // второе свойство
                    (
                        typeof(Person).GetProperty("Age"),
                        agePar
                    )
            );
        var expression = Expression.Lambda<Func<string, int, Person>>
            (
                body,
                namePar,
                agePar
            );
 
        Console.WriteLine(expression);
        // (name, age) => new Person() {Name = name, Age = age}
 
        var initializer = expression.Compile();
        var person = initializer("Старик Хоттабыч", 8942);
        Console.WriteLine("{0}, {1}", person.Name, person.Age);
    }
}
```

Допустим теперь, что мы хотим сохранить полученную лямбду в DLL. Что делать? Надо объединить две технологии: новую — Expression Trees — и старую — Reflection.Emit. Примечание: к сожалению, в .NET Core и NET 5 этот фокус не работает. :( 

```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
 
class Program
{
    static void Main()
    {
        // делаем простейшую лямбду (a, b) => a + b
        var parA = Expression.Parameter(typeof(int), "a");
        var parB = Expression.Parameter(typeof(int), "b");
        var expr = Expression.Lambda<Func<int, int, int>>
            (
                Expression.Add(parA, parB),
                parA, parB
            );
 
        var asmName = new AssemblyName("compiled");
        var asmBuilder = AssemblyBuilder.DefineDynamicAssembly
            (
                asmName,
                AssemblyBuilderAccess.Save
            );
        var moduleBuilder = asmBuilder.DefineDynamicModule
            (
                "compiled",
                "compiled.dll"
            );
        var typeBuilder = moduleBuilder.DefineType
            (
                "CompiledLambda",
                TypeAttributes.Public|TypeAttributes.Sealed
            );
        var methodBuilder = typeBuilder.DefineMethod
            (
                "AddTwoNumbers",
                MethodAttributes.Public|MethodAttributes.Static
            );
        expr.CompileToMethod(methodBuilder);
        typeBuilder.CreateType();
        asmBuilder.Save("compiled.dll");
    }
}
```
Получаем файл compiled.dll размером 2048 байт. Декомпилятор показывает вот что:
![декомпилированный класс](img/compiledLambda.png)

### Организация цикла

Замахнёмся на Вильяма нашего Шекспира, т. е. цикл `for` вроде такого:

```c#
void SomeFunction (int repeats)
{
  for (var i = 0; i != repeats; i++)
   Console.WriteLine(i);
}
```

Поскольку среди IL-кодов нет специального кода для `for`, цикл изображается компилятором вручную примерно так:

```c#
void SomeFunction (int repeats)
{
    var i = 0;
    while (true)
    {
        if (i == repeats)
            goto DONE;
        Console.WriteLine(i);
        i++;
    }
    DONE: ;
}
```

Вот как это выглядит при применении Linq.Expressions:

```c#
using System;
using System.Linq.Expressions;
using System.Reflection;
 
var parameter = Expression.Parameter(typeof(int), "repeats");
var variable = Expression.Variable(typeof(int), "index");
var label = Expression.Label("DONE");
var writeLine = GetMethod((int i) => Console.WriteLine(i));
 
var body = Expression.Block
    (
        new[] { variable },
        Expression.Assign(variable, Expression.Constant(0)),
        Expression.Loop
            (
                Expression.Block
                    (
                        Expression.IfThen
                            (
                                Expression.Equal(variable, parameter),
                                Expression.Break(label)
                            ),
                        Expression.Call(writeLine!, variable),
                        Expression.Assign
                            (
                                variable,
                                Expression.Increment(variable)
                            )
                    ),
                label
            )
    );
 
var action = Expression.Lambda<Action<int>>
    (
        body,
        "SomeFunction",
        new[] { parameter }
    );
 
action.Compile()(10);
 
static MethodInfo GetMethod<T>(Action<T> action) => action.Method;
```
