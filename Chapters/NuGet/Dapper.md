### Dapper

Dapper — простая в освоении и быстрая ORM-библиотека, используемая в проекте StackOverflow (и созданная ради этого проекта).


GitHub: https://github.com/StackExchange/Dapper, NuGet: https://www.nuget.org/packages/Dapper/.

Поддерживает .NET Framework 4.5.1, .NET Standard 1.3 и 2.0.

Фактически библиотека представляет собой набор методов-расширений для интерфейса `IDbConnection`.

#### Выполнение запроса и маппинг результатов на список строго типизированных объектов

```csharp
// Метод-расширение:
public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, 
    object param = null, SqlTransaction transaction = null,
    bool buffered = true);

// Применение:

public class Dog
{
    public int? Age { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float? Weight { get; set; }

    public int IgnoredProperty { get { return 1; } }
}

var guid = Guid.NewGuid();
var dog = connection.Query<Dog>("select Age = @Age, Id = @Id", 
    new { Age = (int?)null, Id = guid });

Assert.Equal(1,dog.Count());
Assert.Null(dog.First().Age);
Assert.Equal(guid, dog.First().Id);
```

#### Выполнение запроса и маппинг результатов на список динамически типизируемых объектов

```csharp
// Метод-расширение:
public static IEnumerable<dynamic> Query (this IDbConnection cnn, string sql,
    object param = null, SqlTransaction transaction = null,
    bool buffered = true);

// Применение:

var rows = connection.Query("select 1 A, 2 B union all select 3, 4");

Assert.Equal(1, (int)rows[0].A);
Assert.Equal(2, (int)rows[0].B);
Assert.Equal(3, (int)rows[1].A);
Assert.Equal(4, (int)rows[1].B);
```

#### Выполнение команды, не возвращающей результатов

```csharp
// Метод-расширение:
public static int Execute(this IDbConnection cnn, string sql,
    object param = null, SqlTransaction transaction = null);

// Применение:

var count = connection.Execute(@"
  set nocount on
  create table #t(i int)
  set nocount off
  insert #t
  select @a a union all select @b
  set nocount on
  drop table #t", new {a=1, b=2 });
Assert.Equal(2, count);
```

#### Выполнение команды несколько раз

```csharp

var count = connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)",
    new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } }
  );
Assert.Equal(3, count); // 3 rows inserted: "1,1", "2,2" and "3,3"
```

