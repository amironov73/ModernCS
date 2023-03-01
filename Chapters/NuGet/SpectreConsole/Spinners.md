### Крутилки (Spinners)

```csharp
AnsiConsole.Status()
    .Spinner (Spinner.Known.Star)
    .Start ("Thinking...", ctx => {
        // Omitted
    });
```

#### Собственный спиннер

```csharp
public sealed class MySpinner
    : Spinner
{
    // The interval for each frame
    public override TimeSpan Interval => TimeSpan.FromMilliseconds (100);
    
    // Whether or not the spinner contains unicode characters
    public override bool IsUnicode => false;

    // The individual frames of the spinner
    public override IReadOnlyList<string> Frames => 
        new List<string>
        {
            "A", "B", "C",
        };
}
```
