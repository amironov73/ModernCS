### ML.NET

Сайт: https://www.microsoft.com/net/learn/apps/machine-learning-and-ai/ml-dotnet, GitHub: https://github.com/dotnet/machinelearning, NuGet: https://www.nuget.org/packages/Microsoft.ML/

Библиотека совместима с .NET Standard 2.0.

Создаём обычное консольное приложение, подключаем пакет Microsoft.ML. Скачиваем содержимое страницы https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data, сохраняем в файл iris-data.txt. Вот что там записано (сокращено):

```
5.1,3.5,1.4,0.2,Iris-setosa
4.9,3.0,1.4,0.2,Iris-setosa
4.7,3.2,1.3,0.2,Iris-setosa
7.0,3.2,4.7,1.4,Iris-versicolor
6.4,3.2,4.5,1.5,Iris-versicolor
6.9,3.1,4.9,1.5,Iris-versicolor
6.3,3.3,6.0,2.5,Iris-virginica
5.8,2.7,5.1,1.9,Iris-virginica
7.1,3.0,5.9,2.1,Iris-virginica
```

Пишем следующую простую и понятную программу:

```csharp
using System;

using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

// ШАГ 1: Задаём структуру данных

// IrisData используется для тренировки и как
// входные данные для операций предсказания
// - первые четыре свойства это признаки,
// используемые для предсказания сорта ириса
// - сорт это то, что мы хотим предсказать,
// во время обучения только устанавливается
public class IrisData
{
    // различные признаки цветка

    [Column("0")]
    public float SepalLength;

    [Column("1")]
    public float SepalWidth;

    [Column("2")]
    public float PetalLength;

    [Column("3")]
    public float PetalWidth;

    // сорт ириса

    [Column("4")]
    [ColumnName("Label")]
    public string Label;
}

// IrisPrediction это результат операции предсказания
public class IrisPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabels;
}

class Program
{
    static void Main()
    {
        // ШАГ 2: создаем канал обучения и загружаем данные
        var pipeline = new LearningPipeline();

        // файл с данными для обучения
        string dataPath = "iris-data.txt";
        pipeline.Add(new TextLoader<IrisData>(dataPath, separator: ","));

        // ШАГ 3: трансформируем данные
        // присваиваем числовые значения тексту в колонке "Label",
        // т. к. только числа могут обрабатываться во время обучения модели
        pipeline.Add(new Dictionarizer("Label"));

        // помещаем все признаки в вектор
        pipeline.Add(new ColumnConcatenator("Features", "SepalLength",
            "SepalWidth", "PetalLength", "PetalWidth"));

        // ШАГ 4: добавляем обучение
        // добавляем алгоритм обучения в канал
        // это сценарий классификации (какой это ирис?)
        pipeline.Add(new StochasticDualCoordinateAscentClassifier());

        // конвертируем "Label" обратно в оригинальный текст
        // (после конверсии в число на шаге 3)
        pipeline.Add(new PredictedLabelColumnOriginalValueConverter()
        {
            PredictedLabelColumn = "PredictedLabel"
        });

        // ШАГ 5: обучаем модель на загруженном наборе данных
        var model = pipeline.Train<IrisData, IrisPrediction>();

        // ШАГ 6: используем обученную модель для предсказания
        // числа ниже задают некий ирис, и модель должна угадать,
        // какого он сорта
        var prediction = model.Predict(new IrisData()
        {
            SepalLength = 3.3f,
            SepalWidth = 1.6f,
            PetalLength = 0.2f,
            PetalWidth = 5.1f,
        });

        Console.WriteLine($"Это {prediction.PredictedLabels}");
    }
}
```

Запускаем программу и получаем:

```
Automatically adding a MinMax normalization transform,
use 'norm=Warn' or 'norm=No' to turn this behavior off.
Using 2 threads to train.
Automatically choosing a check frequency of 2.
Auto-tuning parameters: maxIterations = 9998.
Auto-tuning parameters: L2 = 2,667734E-05.
Auto-tuning parameters: L1Threshold (L1/L2) = 0.
Using best model from iteration 1026.
Not training a calibrator because it is not needed.
Это Iris-virginica
```

Поздравляем, у нас заработало машинное обучение!
