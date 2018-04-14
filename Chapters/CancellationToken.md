### Почему CancellationToken — структура, отдельная от CancellationTokenSource?

[Народ интересуется](http://stackoverflow.com/questions/14215784/why-cancellationtoken-is-separate-from-cancellationtokensource): зачем CancellationToken отделили и к тому же сделали структурой?

Если взглянуть [на исходники CancellationToken](http://referencesource.microsoft.com/#mscorlib/system/threading/CancellationToken.cs), то мы обнаружим следующее (куча лишнего кода убрана):

```csharp
public struct CancellationToken
{
    private CancellationTokenSource m_source;
 
    public bool IsCancellationRequested 
        {
            get
            {
                return m_source != null && m_source.IsCancellationRequested;
            }
        }
 
     public bool CanBeCanceled
        {
            get
            {
                return m_source != null && m_source.CanBeCanceled;
            }
        }
 
        public WaitHandle WaitHandle
        {
            get
            {
                if (m_source == null)
                {
                    InitializeDefaultSource();
                }
  
                return m_source.WaitHandle;
            }
        }
    // и так далее в том же стиле
}
```

Т. е. вся актуальная работа происходит с источником, а токен содержит лишь одно поле – указатель на источник. Перерасхода памяти не происходит. Оверхеда, считай, нет.

Зато, отдавая в какой-нибудь метод токен, мы имеем уверенность, что никто, кроме нас, не установит его в значение «отменено». 🙂 Это можно сделать лишь из CancellationTokenSource.
