### Классы ZipFile и ZipArchive

С .NET Framework 4.5 завезли замечательные классы `System.IO.Compression.ZipFile` и `ZipArchive`, позволяющие как создавать ZIP-архивы, так и распаковывать их.

Пример создания архива из данных, хранящихся в памяти программы:

```c#
using System.IO;
using System.IO.Compression;

class Program
{
    static void AddEntry(ZipArchive archive, string name, string content)
    {
        var entry = archive.CreateEntry(name);
        using var writer = new StreamWriter(entry.Open());
        writer.WriteLine(content);
    }

    static void Main()
    {
        using var archive = ZipFile.Open("test.zip", ZipArchiveMode.Create);
        AddEntry(archive, "readme.txt", "This is a test file");
        AddEntry(archive, "second.txt", "Second file content");
    }
}
```

Для совсем ленивых программистов предусмотрены методы ZipFile.CreateFromDirectory и ZipFile.ExtractToDirectory. Просто отлично — бери да пользуйся!

Вот бы ещё поддержку RAR и 7Z сделали бы! 🙂
