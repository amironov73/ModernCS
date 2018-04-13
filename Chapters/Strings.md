### Рекомендации Microsoft по работе со строками в .NET

Microsoft выложила рекомендации по работе со строками в .NET 2.0 (надо полагать, и в последующих версиях тоже):

* НАДО использовать StringComparison.Ordinal или OrdinalIgnoreCase для сравнения — это безопасный метод для сравнения строк (culture-agnostic string matching).
* НАДО использовать StringComparison.Ordinal или OrdinalIgnoreCase, где это возможно, т. к. они работают намного быстрее.
* НАДО использовать StringComparison.CurrentCulture для строк, которые показываются пользователю (например, в списках).
* НАДО использовать StringComparison.Ordinal или StringComparison.OrdinalIgnoreCase, когда результат сравнения не лингвистический (например, сравнение имён переменных).
* НАДО использовать ToUpperInvariant, а не ToLowerInvariant для нормализации строк перед сравнением.
* НЕЛЬЗЯ использовать перегрузки операций сравнения строк, которые не указывают явно или неявно механизм сравнения.
* НЕЛЬЗЯ в большинстве случаев использовать StringComparison.InvariantCulture. Одним из немногих исключений является сохранение лингвистически-нагруженных, но при этом культурно-агностических данных.

Пример, как предлагается использовать операции сравнения строк:
```csharp
String protocol = MyGetUrlProtocol(); 
 
if (String.Compare(protocol, "ftp", StringComparsion.Ordinal) != 0)
{
   throw new InvalidOperationException();
}
...
if (String.EndsWith(filename, "txt", StringComparison.OrdinalIgnoreCase))
{
   reader = File.OpenText(filename);   
}
```

| Что за данные | Как мы хотим использовать | Какой вид StringComparison использовать |
|---------------|---------------------------|--------|
| <ul><li>Внутренние идентификаторы, чувствительные к регистру</li><li>Чувствительные к регистру идентификаторы в стандартах вроде XML и HTTP</li></li><li>Элементы обеспечения безопасности, чувствительные к регистру (например, пароли)</li></ul> | Нелингвистические идентификаторы, в которых каждый байт означает только самого себя | **Ordinal** |
| <ul><li>Нечувствительные к регистру внутренние идентификаторы</li><li>Нечувствительные к регистру идентификаторы в стандартах вроде XML или HTTP</li><li>Пути к файлам</li><li>Ключи или значения реестра</li><li>Переменные окружения среды</li><li>Идентификаторы ресурсов (например, имена handle’ов)</li><li>Нечувствительные к регистру элементы безопасности (например, логин пользователя)</li></ul> | Нелингвистические идентификаторы, в которых неважен регистр символов (характерный для Windows способ хранения данных) | **OrdinalIgnoreCase** |
| <ul><li>Некие сохранённые лингвистически-релевантные данные</li><li>Отображение лингвистических данных, требующее фиксированного порядка сортировки</li></ul> | Культурно-агностические данные, которые при этом лингвистически актуальны | **InvariantCulture**<br/>или<br/>**InvariantCultureIgnoreCase** |
| <ul><li>Данные, показываемые пользователю</li><li>Большая часть пользовательского ввода</li></ul> | Данные, обрабатываемые, как принято в данной местности | **CurrentCulture**<br/>или<br/>**CurrentCultureIgnoreCase** |

| Метод | Поведение по умолчанию |
|-------|------------------------|
| String.Compare | CurrentCulture |
| String.CompareTo | CurrentCulture |
| String.Equals | Ordinal |
| String.ToUpper | CurrentCulture |
| String.ToLower | CurrentCulture |
| String.StartsWith | CurrentCulture |
| String.EndsWith | CurrentCulture |
| String.IndexOf(string) | CurrentCulture |
| String.IndexOf(char) | Ordinal |
| String.LastIndexOf(string) | CurrentCulture |
| String.LastIndexOf(char) | Ordinal |

Почему это важно. Потому что, например, в турецком языке есть две строчных буквы `i`, одна с точкой, другая без. Соответственно есть и две прописных буквы, тоже с точкой и без, отчего внезапно слово `file` капитализируется в `FİLE`. Обескураживает неподготовленных.
