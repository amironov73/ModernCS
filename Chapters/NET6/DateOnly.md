### DateOnly и TimeOnly

https://www.infoq.com/news/2021/04/Net6-Date-Time/

New DateOnly and TimeOnly structs

Date- and time-only structs have been added, with the following characteristics:

* Each represent one half of a `DateTime`, either only the date part, or only the time part.
* `DateOnly` is ideal for birthdays, anniversary days, and business days. It aligns with SQL Server’s date type.
* `TimeOnly` is ideal for recurring meetings, alarm clocks, and weekly business hours. It aligns with SQL Server’s time type.
* Complements existing date/time types (`DateTime`, `DateTimeOffset`, `TimeSpan`, `TimeZoneInfo`).
* In `System` namespace, shipped in `CoreLib`, just like existing related types.
