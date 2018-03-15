### CallerMemberNameAttribute

� .NET Framework �������� ������������� ������� CallerMemberNameAttribute, � ������� �������� ����� ����������� �������� �����:
```csharp
using System;
using System.Runtime.CompilerServices;
 
class Book
{
    private string _author, _title;
 
    public string Author
    {
        get { return _author; }
        set
        {
            if (_author != value)
            {
                _author = value;
                OnPropertyChanged(_author);
            }
        }
    }
 
    public string Title
    {
        get { return _title; }
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(_title);
            }
        }
    }
 
    private void OnPropertyChanged
        (
            string propertyValue,
            [CallerMemberName] string propertyName = null
        )
    {
        Console.WriteLine
            (
                "{0} changed to {1}",
                propertyName,
                propertyValue
            );
    }
}
 
class Program
{
    static void Main()
    {
        Book book = new Book
        {
            Author = "������, �. �.",
            Title = "������ � ������ � �����"
        };
    }
}
```
��� � ��������� �������, �� ������� ����� ��������
```
Author changed to ������, �. �.
Title changed to ������ � ������ � �����
```
