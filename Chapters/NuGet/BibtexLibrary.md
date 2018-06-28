### Библиотека BibtexLibrary

Очень простая библиотека для парсинга файлов .bib.

GitHub: https://github.com/MaikelH/BibtexLibrary, NuGet: https://www.nuget.org/packages/BibtexLibrary/.

Поддерживается .NET 4.5 и выше.

Пусть у нас есть файл Fuzzy Mining.bib следующего содержания:

```
Scopus
EXPORT DATE: 20 July 2015

@ARTICLE{Günther2007328,
author={Günther, C.W. and Van Der Aalst, W.M.P.},
title={Fuzzy mining - Adaptive process simplification based on multi-perspective metrics},
journal={Lecture Notes in Computer Science (including subseries Lecture Notes in Artificial Intelligence and Lecture Notes in Bioinformatics)},
year={2007},
volume={4714 LNCS},
pages={328-343},
note={cited By 79},
url={http://www.scopus.com/inward/record.url?eid=2-s2.0-38049156249&partnerID=40&md5=35431dc8bc35c675c89f28a854f5b15b},
document_type={Conference Paper},
source={Scopus},
}

@ARTICLE{Hong2003255,
author={Hong, T.-P. and Lin, K.-Y. and Wang, S.-L.},
title={Fuzzy data mining for interesting generalized association rules},
journal={Fuzzy Sets and Systems},
year={2003},
volume={138},
number={2},
pages={255-269},
doi={10.1016/S0165-0114(02)00272-5},
note={cited By 77},
url={http://www.scopus.com/inward/record.url?eid=2-s2.0-0042591520&partnerID=40&md5=b1c8f8d3892eb426898e9e5727f7f9be},
document_type={Article},
source={Scopus},
}

@ARTICLE{Hong20041,
author={Hong, T.-P. and Kuo, C.-S. and Wang, S.-L.},
title={A fuzzy AprioriTid mining algorithm with reduced computational time},
journal={Applied Soft Computing Journal},
year={2004},
volume={5},
number={1},
pages={1-10},
doi={10.1016/j.asoc.2004.03.009},
note={cited By 33},
url={http://www.scopus.com/inward/record.url?eid=2-s2.0-7444240843&partnerID=40&md5=e0f543b53e4808928fd58be4901f8b3f},
document_type={Article},
source={Scopus},
}
```

Вот такая простая программа разбирает данный файл:

```csharp
using System;
using System.IO;

using BibtexLibrary;

class Program
{
    static void Main()
    {
        using (StreamReader reader = new StreamReader("Fuzzy Mining.bib"))
        {
            BibtexFile file = BibtexImporter.FromStream(reader);
            foreach (BibtexEntry entry in file.Entries)
            {
                Console.WriteLine(entry.Tags["author"]);
            }
        }
    }
}
```

выводит

```
Gunther, C.W. and Van Der Aalst, W.M.P.
Hong, T.-P. and Lin, K.-Y. and Wang, S.-L.
Hong, T.-P. and Kuo, C.-S. and Wang, S.-L.
```

