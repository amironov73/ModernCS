### Loop Alignment

https://devblogs.microsoft.com/dotnet/loop-alignment-in-net-6/

When writing a software, developers try their best to maximize the performance they can get from the code they have baked into the product. Often, there are various tools available to the developers to find that last change they can squeeze into their code to make their software run faster. But sometimes, they might notice slowness in the product because of a totally unrelated change. Even worse, when measured the performance of a feature in a lab, it might show instable performance results that looks like the following BubbleSort graph1. What could possibly be introducing such flakiness in the performance?

