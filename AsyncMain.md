### Асинхронная версия Main

```csharp
static async Task Main (string[] args)
{
    Console.WriteLine("Before");
    await Task.Delay(1000);
    Console.WriteLine("After");
}
```
