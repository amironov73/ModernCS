### Новые API для математических вычислений в .NET 6

Новые методы:

* **SinCos** for simultaneously computing Sin and Cos
* **ReciprocalEstimate** for computing an approximate of 1 / x
* **ReciprocalSqrtEstimate** for computing an approximate of 1 / Sqrt(x)

Новые перегрузки существующих методов:

* **Clamp**, **DivRem**, **Min**, and **Max** supporting nint and nuint
* **Abs** and **Sign** supporting nint
* **DivRem** variants which return a tuple
