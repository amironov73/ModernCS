### Как узнать, что сборка Mixed Mode

Самый простой способ – использовать утилиту PEVerify.exe из поставки Visual Studio. Как правило, она расположена в папке `C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\` (если у нас установлена Visual Studio 2012) или `C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\` (если у нас Visual Studio 2013). Просто выполните в командной строке

```
PEVerify.exe DevExpress.SpellChecker.v13.2.Core.dll
```

Если сборка Pure Managed, утилита выведет

```
All Classes and Methods in DevExpress.SpellChecker.v13.2.Core.dll Verified.
```

Если сборка Mixed Mode, т. е. содержит неуправляемый код

```
PEVerify.exe DevExpress.Data.v13.2.dll
```

то утилита выведет

```
[IL]: Ошибка: [DevExpress.Data.v13.2.dll : DevExpress.Utils.Design.DXAssemblyResolver::Init][смещение 0x0000004A] Метод не отображается.
[IL]: Ошибка: [DevExpress.Data.v13.2.dll : DevExpress.Utils.ResourceImageHelper::CreateTransparentImageFromResources][смещение 0x00000054] Метод не отображается.
2 Error(s) Verifying DevExpress.Data.v13.2.dll
```

Короче, для нативных DLL и Mixed Mode сборок посыплются самые разнообразные ошибки.

Для более детального изучения сборки можно использовать утилиту CorFlags, как правило, лежащую рядом с PEVerify.

```
CorFlags.exe DevExpress.SpellChecker.v13.2.Core.dll

Microsoft (R) .NET Framework CorFlags Conversion Tool.  Version  4.0.30319.18020
Copyright (c) Microsoft Corporation.  All rights reserved.

Version   : v4.0.30319
CLR Header: 2.5
PE        : PE32
CorFlags  : 0x9
ILONLY    : 1
32BITREQ  : 0
32BITPREF : 0
Signed    : 1
```

Отметим, что компилятор C++/CLI некорректно устанавливает младший байт CorFlags в 1 (что означает Pure Managed), даже если сборка Mixed Mode, так что на CorFlags в этом смысле полагаться нельзя. Зато, если младший бит не установлен, то сборка однозначно не Pure Managed.

