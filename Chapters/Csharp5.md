### Новые возможности C# 5
#### async/await
Важнейшая фича, конечно же, — async/await
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
Аналогично:
CallerFilePathAttribute, CallerLineNumberAttribute
