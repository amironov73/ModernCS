### StringTemplate

StringTemplate — лёгкий и простой, но довольно гибкий шаблонизатор, который нетрудно встроить в своё приложение, чтобы, например, генерировать HTML-страницы «на лету». Вообще-то, как и «родительский» проект ANTLR, StringTemplate пришел из мира Java, но в виду огромной полезности был портирован на .NET, Python и т. д.

Сайт проекта: http://www.stringtemplate.org/, GitHub: https://github.com/antlr/stringtemplate4, NuGet: https://www.nuget.org/packages/Antlr4.StringTemplate/.

Имеется расширение для Visual Studio, позволяющее редактировать шаблоны StringTemplate со всеми удобствами: https://visualstudiogallery.msdn.microsoft.com/5ca30e58-96b4-4edf-b95e-3030daf474ff.

Простейший пример:

```csharp
using System;
using Antlr4.StringTemplate;
 
class Program
{
    private static string source = @"Hello, $name$!";
 
    static void Main()
    {
        Template template = new Template(source, '$', '$');
        template.Add("name", "World");
        string output = template.Render();
        Console.WriteLine(output);
    }
}
```

Обратите внимание, здесь явно заданы ограничители `"$"`. По умолчанию, StringTemplate использует ограничители `"<>"`, но, как видно, их с лёгкостью можно заменить на любые другие.

Чуть более сложный пример с обращением к свойствам составного объекта:

```csharp
using System;
using Antlr4.StringTemplate;
 
class Program
{
    private static string source
        = @"Hello, $name.FirstName$ $name.LastName$!";
 
    static void Main()
    {
        Template template = new Template(source, '$', '$');
        template.Add("name", 
            new
            {
                FirstName="Alexey",
                LastName ="Mironov"
            });
        string output = template.Render();
        Console.WriteLine(output);
    }
}
```

Попробуем теперь сгенерировать HTML-страничку по некоторым данным. Допустим, у нас есть некий список книг (с полями `Author`, `Title` и `Year`) и такой вот шаблон:

```html
<html lang="ru" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>$title$</title>
</head>
<body>
<h1>Вот какой-то список книг</h1>
    <table>
        <tr>
            <th>№</th>
            <th>Автор</th>
            <th>Заглавие</th>
            <th>Год издания</th>
        </tr>
        $books:{book|
            <tr>
                <td>$i$</td>
                <td>$book.Author$</td>
                <td>$book.Title$</td>
                <td>$book.Year$</td>
            </tr>
        }$
    </table>
</body>
</html>
```

Самое важное здесь — конструкция `$books:{book| … }$`, которая означает «выполни указанный шаблон для коллекции `books`, используя в качестве имени текущего элемента `book`».

Атрибут `i` создан StringTemplate автоматически, в нём хранится порядковый номер текущего элемента коллекции (нумерация с единицы).

Программа для рендеринга страницы выглядит потрясающе просто:

```csharp
using System;
using System.IO;
using Antlr4.StringTemplate;
 
public class BookInfo
{
    public string Author { get; set; }
 
    public string Title { get; set; }
 
    public string Year { get; set; }
}
 
class Program
{
    static void Main()
    {
        // в реальности мы, конечно же, будем извлекать список
        // из базы данных или веб-сервиса
        BookInfo[] books =
        {
            new BookInfo
            {
                Author = "Пушкин, Александр Сергеевич",
                Title = "Сказка о рыбаке и рыбке",
                Year = "1833"
            },
            new BookInfo
            {
                Author = "Гоголь, Николай Васильевич",
                Title = "Шинель",
                Year = "1843"
            },
            new BookInfo
            {
                Author = "Шукшин, Василий Макарович",
                Title = "А поутру они проснулись",
                Year = "1973"
            },
        };
 
        string source = File.ReadAllText("Template.html");
 
        Template template = new Template(source, '$', '$');
        template.Add("title", "Пример StringTemplate");
        template.Add("books", books);
        string output = template.Render();
        Console.WriteLine(output);
    }
}
```

#### Шпаргалка по выражениям StringTemplate

| Синтаксис | Описание |
|-----------|----------|
| \<attribute\> | Значение атрибута (тупо вызывается `ToString()`). Если атрибута с указанным именем нет, выдаётся пустая строка |
| \<i\>, \<i0\> | Номер итерации при обработке коллекции. Нумерация с единицы и с нуля. Работает только внутри блока `{ … }` |
| \<attribute.property\> | Значение свойства объекта. Если нет свойства с указанным именем, выдаётся пустая строка |
| \<attribute.(expr)\> | Косвенное вычисление свойства объекта. Всё как в \<attribute.property\>, только имя свойства вычисляется как выражение |
| \<multi-valued-attribute\> | Конкатенация строк списка |
| \<multi-valued-attribute; separator=expr\> | Конкатенация строк списка с использованием указанного разделителя |
| \<[mine, yours]\> | Создание нового списка (можно указывать списки в качестве элементов) |
| \<template(argument-list)\> | Вызов вложенного шаблона с указанными аргументами |
| \<(expr)(argument-list)\> | Вызов вложенного шаблона, имя шаблона вычисляется выражением |
| \<attribute:template(argument-list)\> | Вызов шаблона для указанного атрибута (с аргументами). Если значение атрибута пустое, ничего не происходит |
| \<attribute:(expr)(argument-list)\> | Вызов шаблона, имя которого вычисляется выражением, для указанного атрибута |
| \<attribute:t1(argument-list): ... :tN(argument-list)\> | Вызов нескольких шаблонов для указанного атрибута |
| \<attribute:{x \| anonymous-template}\> | Применение анонимного шаблона к каждому элементу атрибута-списка. Задание имени для текущего значения |
| \<a1,a2,...,aN:{argument-list \| anonymous-template}\> | Применение анонимного шаблона к перечисленным атрибутам (возможно, спискам) |
| \<attribute:t1(),t2(),…,tN()\> | Применение альтернативного списка шаблонов к элемента атрибута (возможно, с аргументами) |
| \\\< or \\\> | Экранирование символа |
| \<\\ \>, \<\\n\>, \<\\t\>, \<\\r\> | Специальные символы |
| <\uXXXX> | Символы Unicode. Можно задать таким образом целую строку |
| \<\\\\\> | Игнорировать последующий перевод строки. Позволяет использовать внутри шаблона переводы строк для повышения читаемости |
| \<! comment !\> | Комментарий, игнорируется движком StringTemplate |

#### Функции

| Синтаксис | Описание |
|-----------|----------|
| \<first(attr)\> | Первый элемент списка. Второй элемент можно получить так: \<first(rest(attr))\> |
| \<length(attr)\> | Длина списка. Обратите внимание, для строк не делается исключения! \<length("foo")\> выдаст 1. `null` тоже считается. 300 `null` это 300 `null` |
| \<strlen(attr)\> | Длина строки. Если подсунуть не строку, будет выдано исключение |
| \<last(attr)\> | Последний элемент списка |
| \<rest(attr)\> | Элементы списка, кроме первого |
| \<reverse(attr)\> | Список, развёрнутый наоборот |
| \<trunc(attr)\> | Элементы списка, кроме последнего |
| \<strip(attr)\> | Выбросить все null из списка |
| \<trim(attr)\> | Удалить начальные конечные пробелы из строки. Если подсунуть не строку, будет выдано исключение |

#### Управляющие конструкции

Предусмотрен оператор `if` следующего вида:

```xml
<if(attribute)>subtemplate 
<else>subtemplate2 
<endif>
```

Допускается применение `elseif`:

```xml
<if(x)>subtemplate 
<elseif(y)>subtemplate2 
<elseif(z)>subtemplate3 
<else>subtemplate4 
<endif>
```

### Импорт шаблонов

```
import "directory"
import "template file.st"
import "group file.stg"
```
 

#### Зарезервированные слова

Нельзя использовать следующие слова в качестве шаблонов или атрибутов: `true`, `false`, `import`, `default`, `key`, `group`, `implements`, `first`, `last`, `rest`, `trunc`, `strip`, `trim`, `length`, `strlen`, `reverse`, `if`, `else`, `elseif`, `endif`, `delimiters`.
