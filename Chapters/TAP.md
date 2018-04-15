### Task-based Asynchronous Pattern

Вольный и сильно сокращённый пересказ одноименной книги © Stephen Toub, Microsoft.

Task-based Asynchronous Pattern (TAP) – новый паттерн асинхронного программирования от Microsoft. Он базируется на типах `Task` и `Task<TResult>` из пространства имен `System.Threading.Tasks`.

Допустим, у нас есть некоторый синхронный метод, выполнение которого требует довольно много времени (например, он читает данные по сети):

```csharp
public class MyClass
{
    public int Read(byte [] buffer, int offset, int count);
}
```

Мы хотим превратить его в асинхронный, чтобы интерфейс программы не замирал на время его выполнения. До сих пор Microsoft предлагались два асинхронных паттерна. Первый называется `IAsyncResult`-паттерн или APM. Он заключается в том, что создаются два парных метода `BeginMethodName` и `EndMethodName`.

```csharp
public class MyClass
{
    public IAsyncResult BeginRead(byte [] buffer, int offset, int count, 
        AsyncCallback callback, object state);
 
    public int EndRead(IAsyncResult asyncResult);
}
```

Второй называется Event-based Asynchronous Pattern или EAP и заключается в создании метода `MethodNameAsync` с парным событием `MethodNameCompletedHandler`.

```csharp
public class MyClass
{
    public void ReadAsync(byte [] buffer, int offset, int count);
    public event ReadCompletedEventHandler ReadCompleted;
}
 
public delegate void ReadCompletedEventHandler
   (object sender, ReadCompletedEventArgs eventArgs);
 
public class ReadCompletedEventArgs : AsyncCompletedEventArgs
{
    public int Result { get; }
}
```

TAP требует всего одного метода:

```csharp
public class MyClass
{
    public Task<int> ReadAsync(byte [] buffer, int offset, int count);
}
```

Главное требование здесь – чтобы метод возвращал `Task` или `Task<T>`. Другое требование – нельзя использовать параметры с модификаторами `ref` или `out`. Всё, что нужно, метод должен вернуть в типе `T`, параметризующем тип `Task<T>`.

Код в методе, реализующем TAP, не обязательно будет выполняться в другом потоке. В ряде случаев он может отработать синхронно. Любые исключения, возникшие в коде метода, назначаются возвращаемой задаче. Они будут повторно выброшены при попытке обращения к свойству `Result` класса `Task<T>` или при окончании ожидания в методах `Wait`, `WaitAll` и других аналогичных. Кроме того, если исключение не было выброшено, а выполнение задачи по каким-либо причинам завершилось без `Result`/`WaitAll`, исключение будет выброшено «где придётся». Обращение к полю `Exception` позволяет избежать этого – исключение будет считаться просмотренным и обработанным.

Если нам нужна возможность отмены асинхронного вызова, необходимо добавить в сигнатуру метода параметр типа `CancellationToken`:

```csharp
public Task<int> ReadAsync
    (
        byte [] buffer, int offset, int count,
        CancellationToken cancellationToken
    );
```

Если нужно отслеживание прогресса длительной операции, необходимо добавить в сигнатуру метода параметр типа `IProgress<T>`:

```csharp
public Task<int> ReadAsync
    (
       byte [] buffer, int offset, int count, 
       IProgress<int> progress
    );
```

В Framework 4.5 имеется базовый класс `Progress<T>`, от которого можно унаследовать собственную реализацию прогресс-репортера:

```csharp
public class Progress<T> : IProgress<T>
{
    public Progress();
    public Progress(Action<T> handler);
    protected virtual void OnReport(T value);
    public event EventHandler<T> ProgressChanged;
}
```

Пример «гибридной» реализации TAP, когда предварительные проверки вынесены в синхронный метод, а рабочий код – в асинхронный метод:

```csharp
public Task<int> MethodAsync(string input)
{
    if (input == null) throw new ArgumentNullException("input");
    return MethodAsyncInternal(input);
}
 
private async Task<int> MethodAsyncInternal(string input)
{
    … // code that uses await
}
```

Публичный API должен возвращать Task и Task<T> в уже запущенном состоянии.

Пример метода, реализующего паттерн TAP:

```csharp
public Task<Bitmap> RenderAsync
    (
        ImageData data,
        CancellationToken cancellationToken
    )
{
    return Task.Run(() =>
    {
        var bmp = new Bitmap(data.Width, data.Height);
        for(int y=0; y<data.Height; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            for(int x=0; x<data.Width; x++)
            {
                … // render pixel [x,y] into bmp
            }
        }
        return bmp;
    }, cancellationToken);
}
```

В .NET 4 для создания и запуска вычислительных задач (compute-bound task) рекомендуется использовать метод `Task.Factory.StartNew`, который использует планировщик задач из свойства `TaskFactory.Scheduler`. В .NET 4.5 добавлен метод `Task.Run`.

Как применяется CancellationToken:

```csharp
var cts = new CancellationTokenSource();
string result = await DownloadStringAsync(url, cts.Token);
… // at some point later, potentially on another thread
cts.Cancel();
```

Один токен можно применять к нескольким асинхронным операциям сразу:

```csharp
var cts = new CancellationTokenSource();
IList<string> results = await Task.WhenAll
    (
        from url in urls select DownloadStringAsync(url, cts.Token)
    );
…
cts.Cancel();
```

Отменять асинхронную операцию можно из любого потока.

Как применяется `IProgress<T>`:

```csharp
private async void btnDownload_Click(object sender, RoutedEventArgs e)  
{
    btnDownload.IsEnabled = false;
    try
    {
        txtResult.Text = await DownloadStringAsync(txtUrl.Text, 
            new Progress<int>(p => pbDownloadProgress.Value = p));
    }
    finally { btnDownload.IsEnabled = true; }
}
```

Как правильно обрабатывать исключения при `WaitAll`/`WhenAll` и пр.:

```csharp
Task [] asyncOps = 
    (from url in urls select DownloadStringAsync(url)).ToArray();
try
{
    string [] pages = await Task.WhenAll(asyncOps);
    ...
}
catch(Exception exc)
{
    foreach(Task<string> faulted in asyncOps.Where(t => t.IsFaulted))
    {
        … // work with faulted and faulted.Exception
    }
}
```

Как обрабатывать результаты асинхронных операций по одному в порядке завершения:

```csharp
var recommendations = new List<Task<bool>>() 
{ 
    GetBuyRecommendation1Async(symbol), 
    GetBuyRecommendation2Async(symbol),
    GetBuyRecommendation3Async(symbol)
};
while(recommendations.Count > 0)
{ 
    Task<bool> recommendation = await Task.WhenAny(recommendations);    
    try
    {
        if (await recommendation) BuyStock(symbol);
        break;
    }
    catch(WebException exc)
    {
        recommendations.Remove(recommendation);
    }
}
```

Throttling — ограничение количество одновременно выполняемых задач.

```csharp
const int CONCURRENCY_LEVEL = 15;
Uri [] urls = …;
int nextIndex = 0;
var imageTasks = new List<Task<Bitmap>>();
while(nextIndex < CONCURRENCY_LEVEL && nextIndex < urls.Length)
{
    imageTasks.Add(GetBitmapAsync(urls[nextIndex]));
    nextIndex++;
}
 
while(imageTasks.Count > 0)
{
    try
    {
        Task<Bitmap> imageTask = await Task.WhenAny(imageTasks);
        imageTasks.Remove(imageTask);
 
        Bitmap image = await imageTask;
        panel.AddImage(image);
    }
    catch(Exception exc) { Log(exc); }
 
    if (nextIndex < urls.Length)
    {
        imageTasks.Add(GetBitmapAsync(urls[nextIndex]));
        nextIndex++;
    }
}
```

Если нам достаточно первой выполнившейся задачи:

```csharp
public static async Task<T> NeedOnlyOne
    (
        params Func<CancellationToken,Task<T>> [] functions
    )
{
    var cts = new CancellationTokenSource();
    var tasks = (from function in functions
                 select function(cts.Token)).ToArray();
    var completed = await Task.WhenAny(tasks).ConfigureAwait(false);
    cts.Cancel();
    foreach(var task in tasks) 
    {
        var ignored = task.ContinueWith
            (
                t => Log(t),
                TaskContinuationOptions.OnlyOnFaulted
            );
    }
 
    return completed;
}
```

На всякий случай прикладываю документ от Microsoft: [TAP.docx](https://blog.mironov.online/wp-content/uploads/TAP.docx).


