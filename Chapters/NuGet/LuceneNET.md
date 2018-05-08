### Lucene.NET

Сайт: https://lucenenet.apache.org/, GitHub: https://github.com/apache/lucenenet, NuGet: https://www.nuget.org/packages/Lucene.Net/

Актуальная версия: 4.8.0 (beta5)

Имеются библиотеки:

* **Lucene.Net** - Core library
* **Lucene.Net.Analysis.Common** - Analyzers for indexing content in different languages and domains
* **Lucene.Net.Analysis.Kuromoji** - Japanese Morphological Analyzer
* **Lucene.Net.Analysis.Phonetic** - Analyzer for indexing phonetic signatures (for sounds-alike search)
* **Lucene.Net.Analysis.SmartCn** - Analyzer for indexing Chinese
* **Lucene.Net.Analysis.Stempel** - Analyzer for indexing Polish
* **Lucene.Net.Benchmark** - System for benchmarking Lucene
* **Lucene.Net.Classification** - Classification module for Lucene
* **Lucene.Net.Codecs** - Lucene codecs and postings formats
* **Lucene.Net.Expressions** - Dynamically computed values to sort/facet/search on based on a pluggable grammar
* **Lucene.Net.Facet** - Faceted indexing and search capabilities
* **Lucene.Net.Grouping** - Collectors for grouping search results
* **Lucene.Net.Highlighter** - Highlights search keywords in results
* **Lucene.Net.ICU** - Specialized ICU (International Components for Unicode) Analyzers and Highlighters
* **Lucene.Net.Join** - Index-time and Query-time joins for normalized content
* **Lucene.Net.Memory** - Single-document in-memory index implementation
* **Lucene.Net.Misc** - Index tools and other miscellaneous code
* **Lucene.Net.Queries** - Filters and Queries that add to core Lucene
* **Lucene.Net.QueryParser** - Text to Query parsers and parsing framework
* **Lucene.Net.Replicator** - Files replication utility
* **Lucene.Net.Sandbox** - Various third party contributions and new ideas
* **Lucene.Net.Spatial** - Geospatial search
* **Lucene.Net.Suggest** - Auto-suggest and Spellchecking support

Поддерживает .NET Framework 3.5, 4.0.

Простая программа, которая сначала создает индекс для текстового файла, а затем выводит номера строк, в которых содержится искомое слово.

```csharp
using System;
using System.IO;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

using IODirectory = System.IO.Directory;
using LuceneDirectory = Lucene.Net.Store.Directory;

class Program
{
    static void Main()
    {
        // стоп-слова, не включаемые в индекс
        CharArraySet stopWords = new CharArraySet
            (
                LuceneVersion.LUCENE_48,
                new[] { "в", "к", "не", "по" },
                true
            );
        // разбирает текст по правилам русского языка
        Analyzer analyzer = new RussianAnalyzer
            (
                LuceneVersion.LUCENE_48,
                stopWords
            );

        // где хранится индекс
        string path = Path.Combine
            (
                IODirectory.GetCurrentDirectory(),
                "Index"
            );
        // удаляем предыдущий индекс, если он существует
        if (IODirectory.Exists(path))
        {
            IODirectory.Delete(path, true);
        }

        LuceneDirectory indexDirectory = FSDirectory.Open(path);
        IndexWriterConfig config = new IndexWriterConfig
            (
                LuceneVersion.LUCENE_48,
                analyzer
            );
        IndexWriter writer = new IndexWriter(indexDirectory, config);

        // заполняем индекс строками
        int lineNumber = 1;
        foreach (var line in File.ReadLines("TechInfo.txt"))
        {
            Document doc = new Document
            {
                new Int32Field("LineNumber", lineNumber, Field.Store.YES),
                new TextField("LineText", line, Field.Store.YES)
            };
            writer.AddDocument(doc);
            lineNumber++;
        }
        writer.Flush(false, false);
        writer.Dispose();

        // открываем индекс на чтение
        IndexReader reader = DirectoryReader.Open(indexDirectory);
        IndexSearcher searcher = new IndexSearcher(reader);
        QueryParser parser = new QueryParser
            (
                LuceneVersion.LUCENE_48,
                "LineText",
                analyzer
            );
        // искомое слово
        string searchTerm = "ключ";
        Query query = parser.Parse(searchTerm);

        // отбираем 10 наиболее подходящих строк
        TopDocs topDocs = searcher.Search(query, 10);
        foreach (ScoreDoc found in topDocs.ScoreDocs)
        {
            // степень соответствия
            float score = found.Score;
            Document doc = searcher.Doc(found.Doc);
            lineNumber = doc.GetField("LineNumber").GetInt32ValueOrDefault();
            string lineText = doc.Get("LineText");
            Console.WriteLine($"{lineNumber} [{score:F2}]: {lineText.Trim()}");
        }

        reader.Dispose();
        indexDirectory.Dispose();
    }
}
```

Выводит:
```
39 [2,32]: Ключ "/u <username>"
72 [1,45]: Изменение  значения  ключа  на  "1"  позволяет  устранить  проблему  при
```

