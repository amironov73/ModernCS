### Класс Task

#### Чем отличается Task.Run() от Task.Factory.StartNew() ?


Смотрим в [ReferenceSource](https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/Task.cs) и видим:

```csharp
public static Task Run(Action action)
{
    StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;

    return Task.InternalStartNew
        (
            creatingTask: null,
            action: action,
            state: null,
            cancellationToken: default(CancellationToken),
            scheduler: TaskScheduler.Default,
            options: TaskCreationOptions.DenyChildAttach,
            internalOptions: InternalTaskOptions.None,
            stackMark: ref stackMark
        );
}
```

и [в классе TaskFactory](https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/TaskFactory.cs):

```csharp
public Task StartNew(Action action)
{
    StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;

    Task currTask = Task.InternalCurrent;
    return Task.InternalStartNew
        (
            creatingTask: currTask,
            action: action,
            state: null, 
            cancellationToken: m_defaultCancellationToken, 
            scheduler: GetDefaultScheduler(currTask),
            options: m_defaultCreationOptions,
            internalOptions: InternalTaskOptions.None, 
            stackMark: ref stackMark
        );
}
```

По большому счету, вся разница лишь в том, что Task.Run не зависит от текущей задачи. Переменная m_defaultCreationOptions по умолчанию равна None, но может быть изменена, что скажется на StartNew, но не на Run.
