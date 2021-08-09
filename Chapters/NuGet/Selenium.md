### Selenium WebDriver

[Selenium WebDriver](https://www.selenium.dev) — это инструмент для автоматизации действий веб-браузера. В большинстве случаев используется для тестирования Web-приложений, но этим не ограничивается. В частности, он может быть использован для решения рутинных задач администрирования сайта или регулярного получения данных из различных источников (сайтов). © Вики

Как же нам использовать это сокровище из своих программ на C#? Чрезвычайно просто! Следите за руками:

Первое. Создаем консольное приложение, можно как классический .NET Framework (включая .NET 3.5!), так и .NET 5 (или .NET Standard 2.0). Подключаем к нему NuGet-пакеты [Selenium.WebDriver](https://www.nuget.org/packages/Selenium.WebDriver) и [Selenium.WebDriver.ChromeDrive](https://www.nuget.org/packages/Selenium.WebDriver.ChromeDriver) (если собираетесь использовать Chrome) (я не настаиваю на последнем, есть другие аналогичные пакеты, возможно, они подойдут Вам больше).

Второе. Пишем простую программу:

```c#
using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using var driver = new ChromeDriver();
var wait = new WebDriverWait (driver, TimeSpan.FromSeconds(10));

driver.Navigate().GoToUrl ("https://www.google.com/ncr");
driver.FindElement (By.Name ("q")).SendKeys ("бетон" + Keys.Enter);
wait.Until (webDriver => webDriver.FindElement (By.CssSelector ("h3")).Displayed);

var firstResult = driver.FindElement (By.CssSelector ("h3"));
Console.WriteLine (firstResult.GetAttribute ("textContent"));
```

Третье. Запускаем и радуемся — всё работает!
