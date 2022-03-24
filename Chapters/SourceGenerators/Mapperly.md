### Mapperly

Как известно, работа программиста по бо́льшей части состоит в перекладывании данных из одних объектов в другие. Это довольно скучное рутинное занятие, от которого раньше мы спасались с помощью библиотек вроде AutoMapper. С появлением генераторов исходного кода стало возможно делать то же самое быстрее и с меньшей нагрузкой на память.

[Mapperly](https://github.com/riok/mapperly) – пожалуй, самый популярный генератор «перекладывателей данных». Важнейшее его достоинство – крайняя простота применения. Достаточно подключить к проекту NuGet-пакет [Riok.Mapperly](https://www.nuget.org/packages/Riok.Mapperly/) и завести в проекте partial-класс, помеченный атрибутом Mapper:

```c#
using Riok.Mapperly.Abstractions;
 
var mapper = new CarMapper();
 
var dto = new CarDto
{
    Model = "КрАЗ-256Б",
    Wheels = 6,
    Length = 8190,
    Year = 1966
};
 
var description = mapper.DtoToDescription (dto);
Console.WriteLine (description);
 
description = new CarDescription
{
    Model = "КрАЗ-256Б",
    Wheels = 6,
    Year = 1966,
    Weight = 12050
};
 
dto = mapper.DescriptionToDto (description);
Console.WriteLine (dto);
 
return 0;
 
public sealed class CarDescription
{
    public int Wheels { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Weight { get; set; }
 
    public override string ToString()
    {
        return $"Wheels: {Wheels}, Model: {Model}, Year: {Year}, Weight: {Weight}";
    }
}
 
public sealed class CarDto
{
    public int Wheels { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Length { get; set; }
 
    public override string ToString()
    {
        return $"Wheels: {Wheels}, Model: {Model}, Year: {Year}, Length: {Length}";
    }
}
 
[Mapper]
public partial class CarMapper
{
    public partial CarDescription DtoToDescription (CarDto dto);
    public partial CarDto DescriptionToDto (CarDescription dto);
}
```
