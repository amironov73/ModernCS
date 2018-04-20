### IKVM.NET

В мире Java существует множество восхитительных библиотек, которые хотелось бы использовать в своих программах для .NET Framework. Часто оказывается, что добрые люди уже портировали нужную библиотеку на C# (актуальный для меня пример – ANTLR Runtime). Но портировано далеко не всё. Или портировано, но с ошибками. Или порт отстаёт от актуальной версии на пару-тройку цифр.

Оказывается, этому горю легко помочь! К нашим услугам проект [IKVM.NET](http://www.ikvm.net/) – реализация виртуальной Java-машины и основных классов Java поверх .NET. Поэтому мы можем просто взять уже имеющийся байт-код Java и вызывать его из своей программы так, будто это обычная .NET-сборка. Разве не замечательно?

Сначала простой пример. Пусть у нас есть следующее сверхсложное приложение на Java:

```java
package com.arsmagna;
 
public class MainClass
{
  public static void main ( String[] args )
  {
    System.out.println ( "Привет, объединённый мир Java и .NET!" );
  }
}
```

Раньше, чтобы запустить его, надо было иметь установленную среду исполнения Java (JRE). Далее, в командной строке надо было написать

```
java -jar veryComplexApp.jar
```

и на экран выводилась строка «Привет». С IKVM.NET нам не нужно иметь JRE (но нужно иметь .NET Framework!), достаточно скачать архив с [SourceForge](http://sourceforge.net/project/showfiles.php?group_id=69637), распаковать его и написать в командной строке

```
ikvm -jar veryComplexApp.jar
```

чтобы получить тот же результат. Согласитесь, это несложно.

Перейдём теперь к более интересному случаю: пусть у нас есть сверхсложная библиотека полезнейших классов на Java:

```java
package org.arsmagna;
 
public class HelloClass
{
    public String MakeHelloStringForName ( String name )
    {
        return "Hello, " + name;
    }
}
```

Мы хотим использовать эту библиотеку в своём .NET-приложении, а для этого надо преобразовать её в сборку .NET.

Как правило, библиотека скомпилирована в JAR-файл. В .NET-сборку он преобразуется элементарно:

```
ikvmc veryComplexLib.jar
```

Программа, использующая полученную сборку:

```csharp
using System;
 
using org.arsmagna;
 
class ClientApp
{
  static void Main ( string[] args )
  {
    string name = "Alexey";
    HelloClass hello = new HelloClass ();
    string greeting = hello.MakeHelloStringForName ( name );
    Console.WriteLine ( greeting );
  }
}
```

Компиляция программы:

```
csc.exe ClientApp.cs /r:veryComplexLib.dll,IKVM.OpenJDK.Core.dll
```

Главное – не за быть сослаться на сборку IKVM, вот и вся хитрость.

IKVM.NET теперь живёт на NuGet: https://www.nuget.org/packages/IKVM/.

