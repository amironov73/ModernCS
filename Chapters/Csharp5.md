### ����� ����������� C# 5
#### async/await
��������� ����, ������� ��, � async/await
```csharp
async Task Demo()
{
    using (HttpClient client = new HttpClient())
    {
       string responseBody = await client.GetStringAsync("http://www.contoso.com/");

       Console.WriteLine(responseBody);
    }
}
```
#### [CallerMemberNameAttribute](CallerMemberNameAttribute)
����������:
CallerFilePathAttribute, CallerLineNumberAttribute
