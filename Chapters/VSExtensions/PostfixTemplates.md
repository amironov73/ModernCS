### ����������� ������� � ReSharper

��� HTML � CSS ���������� ������������� Emmet, � ��� �� ���� ��� C#? ���� ��� ���������� ����������� �������. ��������, �� �������� �������

```csharp
buffer.Length!=0.if
```

(�������� �������� �� `.if`!) � �������� `TAB`, � ReSharper ��� �� ���������� ���� ����� � ���������:

```csharp
if (buffer.Length != 0)
{
  |
}
```

(����� ������������ ����� �������� ������). � ��������, ������, ���� � ����������. �������� ��������� �������:


| ���������� | ��� �������� |
|---------|------------|
|.if | if (expr) (��. ����)
|else | if (!expr)
|.null | if (expr == null)
|.notnull | if (expr != null)
| .not | !expr
| .foreach | foreach (var x in expr)
| .for | for (var i = 0; i &lt; expr.Length; i++)
| .forr | for (var i = expr.Length � 1; i &gt;= 0; i�)
| .var | �������������� ����� ����������: var x = expr;
| .arg | ��������� ����� ������: Method(expr)
| .to | ������������: lvalue = expr;
| .await | await expr
| .cast | ((SomeType) expr)
| .field | ������ ���� ��� ���������: _field = expr;
| .prop | ������ �������� ��� ���������: Prop = expr;
| .new | new T()
| .par | ��������� ��������� � ������: (expr)
| .parse | int.Parse(expr)
| .return | return expr;
| .typeof | typeof(expr)
| .switch | switch(expr)
| .yield | yield return expr;
| .throw | throw expr;
| .using | using(var x = expr)
| .while | while(expr)
| .lock | lock(expr)
| .sel | �������� ��������� � ���������
