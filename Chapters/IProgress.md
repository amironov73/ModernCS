### Интерфейс IProgress

С вхождением в программистскую жизнь асинхронного программирования входит и проблема, как лучше сообщать пользователю о том, что программа не просто «тупит», а занята чем-то полезным. С выходом .NET Framework 4.5 Microsoft предложила своё решение – интерфейс `IProgress<T>`:

```csharp
public interface IProgress<in T>
{
  void Report(T value);
}
```

и реализующий его класс `Progress<T>`:

```csharp
public class Progress<T> : IProgress<T>
{
  public Progress();
  public Progress(Action<T> handler);
 
  protected virtual void OnReport(T value);
 
  public event EventHandler<T> ProgressChanged;
}
```

Делают они простую и нужную вещь: все переданные классу Progress делегаты (как в конструкторе, так и в событии) запускаются в том контексте синхронизации (SynchronizationContext), который был захвачен классом в его конструкторе. Вот и всё, что нужно. (Если на момент вызова конструктора контекста синхронизации не было, то делегат запускается в ThreadPool).

Пример:

```csharp
async Task<int> UploadPicturesAsync
  (
    List<Image> imageList,
    IProgress<int> progress
  )
{
  int totalCount = imageList.Count;
  int processCount = await Task.Run<int>(() =>
  {
    int tempCount = 0;
    foreach (var image in imageList)
    {
      await UploadAndProcessAsync(image);
      progress.Report(tempCount * 100 / totalCount);
      tempCount++;
    }
 
      return tempCount;
  });
   
  return processCount;
}
 
...
 
void ReportProgress(int value)
{
  progressLabel.Text = "Uploaded " + value + " images";
}
 
...
 
private async void Start_Button_Click
  (
    object sender, 
    EventArgs e
  )
{
  var progress = new Progress<int>(ReportProgress);
  int uploads = await UploadPicturesAsync
    (
      GenerateTestImages(),
      progressIndicator
    );
}
```

