### Новшества в System.Linq

[New System.LINQ APIs](https://github.com/dotnet/runtime/issues/47231) have been added that have been requested and contributed by the community.

#### Enumerable support for `Index` and `Range` parameters

The `Enumerable.ElementAt` method now accepts indices from the end of the enumerable, as you can see in the following example.

```c#
Enumerable.Range(1, 10).ElementAt(^2); // returns 9
```

An `Enumerable.Take` overload has been added that accepts Range parameters. It simplifies taking slices of enumerable sequences:

```c#
source.Take(..3) instead of source.Take(3)
source.Take(3..) instead of source.Skip(3)
source.Take(2..7) instead of source.Take(7).Skip(2)
source.Take(^3..) instead of source.TakeLast(3)
source.Take(..^3) instead of source.SkipLast(3)
source.Take(^7..^3) instead of source.TakeLast(7).SkipLast(3).
```

Credit to @dixin for contributing the implementation.

##### TryGetNonEnumeratedCount

The `TryGetNonEnumeratedCount` method attempts to obtain the count of the source enumerable without forcing an enumeration. This approach can be useful in scenarios where it is useful to preallocate buffers ahead of enumeration, as you can see in the following example.

```c#
List<T> buffer = source.TryGetNonEnumeratedCount(out int count) ? new List<T>(capacity: count) : new List<T>();
foreach (T item in source)
{
  buffer.Add(item);
}
```

`TryGetNonEnumeratedCount` checks for sources implementing ICollection/ICollection<T> or takes advantage of some of the internal optimizations employed by Linq.

##### DistinctBy/UnionBy/IntersectBy/ExceptBy

New variants have been added to the set operations that allow specifying equality using key selector functions, as you can see in the following example.

```c#
Enumerable.Range(1, 20).DistinctBy(x => x % 3); // {1, 2, 3}

var first = new (string Name, int Age)[] { ("Francis", 20), ("Lindsey", 30), ("Ashley", 40) };
var second = new (string Name, int Age)[] { ("Claire", 30), ("Pat", 30), ("Drew", 33) };
first.UnionBy(second, person => person.Age); // { ("Francis", 20), ("Lindsey", 30), ("Ashley", 40), ("Drew", 33) }
```

##### MaxBy/MinBy

`MaxBy` and `MinBy` methods allow finding maximal or minimal elements using a key selector, as you can see in the following example.

```c#
var people = new (string Name, int Age)[] { ("Francis", 20), ("Lindsey", 30), ("Ashley", 40) };
people.MaxBy(person => person.Age); // ("Ashley", 40)
```

##### Chunk

`Chunk` can be used to chunk a source enumerable into slices of a fixed size, as you can see in the following example.

```c#
IEnumerable<int[]> chunks = Enumerable.Range(0, 10).Chunk(size: 3); // { {0,1,2}, {3,4,5}, {6,7,8}, {9} }
```

Credit to Robert Andersson for contributing the implementation.

##### FirstOrDefault/LastOrDefault/SingleOrDefault overloads taking default parameters

The existing `FirstOrDefault`/`LastOrDefault`/`SingleOrDefault` methods return `default(T)` if the source enumerable is empty. New overloads have been added that accept a default parameter to be returned in that case, as you can see in the following example.

```c#
Enumerable.Empty<int>().SingleOrDefault(-1); // returns -1
```

Credit to @Foxtrek64 for contributing the implementation.

##### Zip overload accepting three enumerables

The `Zip` method now supports combining three enumerables, as you can see in the following example.

```c#
var xs = Enumerable.Range(1, 10);
var ys = xs.Select(x => x.ToString());
var zs = xs.Select(x => x % 2 == 0);

foreach ((int x, string y, bool z) in Enumerable.Zip(xs,ys,zs))
{
}
```

Credit to Huo Yaoyuan for contributing the implementation.
