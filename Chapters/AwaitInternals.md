### Во что разворачивается await

Sinix пишет:

> Код вида
```csharp
Report("Before await");
var someValue = await GetSomeValueAsync();
Report("After await: {0}", someValue);
```
> компилятор превращает в нечто вроде
```csharp
Report("Before await");
var awaiter = GetSomeValueAsync().GetAwaiter();
awaiter.OnCompleted
  (
    () => Report("After await: {0}", awaiter.GetResult()
  );
```

#### Как можно обойтись без async/await
```csharp
private async void button1_Click(object sender, EventArgs e)
{
    var t = await mc.GetParam();
    button1.Text = t;
}
```
можно переписать как
```csharp
private /*async*/ void button1_Click(object sender, EventArgs e)
{
    var task = mc.GetParam();         
    task.ContinueWith
        (
            t => button1.Text = t.Result,
            TaskScheduler.FromCurrentSynchronizationContext()
        );
}
```
