### Morphology.NET

Берется с http://morphology.codeplex.com/.

Главный недостаток: спроектирован под .Net 1.1, не все исходные тексты доступны. При работе с современными версиями .Net возникают разнообразные ошибки, исправить которые не представляется возможным.

Можно попробовать пересобрать с обновленной версией http://cerebrum.codeplex.com/.

Необходимо подключить сборки Cerebrum.Integrator, Cerebrum.Management, Cerebrum.Runtime, Cerebrum.Runtime.Semiotics, Cerebrum.Typedef, Cerebrum.Vocabulary, Cerebrum.Vocabulary.Library.

Пример простейшей программы:

```csharp
using System;
using Cerebrum.Vocabulary.DataModel;
using Cerebrum.Vocabulary.Library;
 
using App = Cerebrum.Vocabulary.Library.Application;
 
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            App app = new App ( "D:/Projects/AotGames/"
               + "ConsoleApplication2/bin/Debug/"
               + "Russian Morphology Database.vnn" );
            Transform transform = new Transform 
              ( 
                 app.MasterContext, 
                 app.m_WMaster 
              );
            WordInfo[] words = transform
              .MorphologicalAnalysisOfWord 
                ( 
                  "кошке"
                );
            foreach ( WordInfo word in words )
            {
                foreach ( WordFormInfo info in
                  word.WordFormInfoList )
                {
                    Console.WriteLine ( info.Word );
                }
            }
            Console.WriteLine ();
            Console.ReadLine ();
        }
    }
}
```

Вывод программы:

```
кошке
кошке
кошке
кошке
кошке
кошке
```
