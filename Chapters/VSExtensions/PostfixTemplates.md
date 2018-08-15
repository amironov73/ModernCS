### Постфиксные шаблоны в ReSharper

Для HTML и CSS существует замечательный Emmet, а что же есть для C#? Есть так называемые постфиксные шаблоны. Например, мы набираем сначала

```csharp
buffer.Length!=0.if
```

(обратите внимание на `.if`!) и нажимаем `TAB`, а ReSharper тут же раскрывает этот текст в следующее:

```csharp
if (buffer.Length != 0)
{
  |
}
```

(здесь вертикальная черта означает курсор). В принципе, удобно, хоть и непривычно. Доступны следующие шаблоны:


| Сокращение | Что означает |
|---------|------------|
|.if | if (expr) (см. выше)
|else | if (!expr)
|.null | if (expr == null)
|.notnull | if (expr != null)
| .not | !expr
| .foreach | foreach (var x in expr)
| .for | for (var i = 0; i &lt; expr.Length; i++)
| .forr | for (var i = expr.Length — 1; i &gt;= 0; i—)
| .var | Инициализирует новую переменную: var x = expr;
| .arg | Формирует вызов метода: Method(expr)
| .to | Присваивание: lvalue = expr;
| .await | await expr
| .cast | ((SomeType) expr)
| .field | Вводит поле для выражения: _field = expr;
| .prop | Вводит свойство для выражения: Prop = expr;
| .new | new T()
| .par | Заключает выражение в скобки: (expr)
| .parse | int.Parse(expr)
| .return | return expr;
| .typeof | typeof(expr)
| .switch | switch(expr)
| .yield | yield return expr;
| .throw | throw expr;
| .using | using(var x = expr)
| .while | while(expr)
| .lock | lock(expr)
| .sel | Помечает выражение в редакторе
