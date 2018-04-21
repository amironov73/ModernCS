### FastJSON

Ещё одна конкурирующая реализация работы с JSON для .NET. Позиционируется как небольшая (всего 3 класса плюс два хелпера, сборка имеет размер всего 25 Кб), очень быстрая (в большинстве случаев) и поддерживающая полиморфизм (т. е. без проблем разбирающаяся с полиморфными коллекциями). Работает, начиная с .NET 2.0, в т. ч. на MonoDroid и Silverlight.

* CodePlex: http://fastjson.codeplex.com/
* GitHub: https://github.com/mgholam/fastJSON
* NuGet: https://www.nuget.org/packages/fastJSON/
*Статья на CodeProject: http://www.codeproject.com/Articles/159450/fastJSON (на всякий случай, в виде PDF: fastJSON.pdf)

Примерный код для сериализации и десериализации:

```csharp
// объект в строку
string jsonText = fastJSON.JSON.Instance.ToJSON(c);
 
// строку в объект
var newobj = fastJSON.JSON.Instance.ToObject(jsonText);
```

