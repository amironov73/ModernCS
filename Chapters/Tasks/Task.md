### ����� Task

#### ��� ���������� Task.Run() �� Task.Factory.StartNew() ?


������� � [ReferenceSource](https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/Task.cs) � �����:

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

� [� ������ TaskFactory](https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/TaskFactory.cs):

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

�� �������� �����, ��� ������� ���� � ���, ��� Task.Run �� ������� �� ������� ������. ���������� m_defaultCreationOptions �� ��������� ����� None, �� ����� ���� ��������, ��� �������� �� StartNew, �� �� �� Run.
