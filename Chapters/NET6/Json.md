### Изменения в System.Text.Json в .NET 6

#### Поддержка IAsyncEnumerable

`IAsyncEnumerable<T>` is an important feature that was added with .NET Core 3.0 and C# 8. The new enhancements enable `System.Text.Json` (de)serialization with `IAsyncEnumerable<T>` objects.

The following examples use streams as a representation of any async source of data. The source could be files on a local machine, or results from a database query or web service API call.

##### Streaming serialization

`System.Text.Json` now supports serializing `IAsyncEnumerable<T>` values as JSON arrays, as you can see in the following example.

```c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

static async IAsyncEnumerable<int> PrintNumbers(int n)
{
  for (int i = 0; i < n; i++) yield return i;
}

using Stream stream = Console.OpenStandardOutput();
var data = new { Data = PrintNumbers(3) };
await JsonSerializer.SerializeAsync(stream, data); // prints {"Data":[0,1,2]}
```

`IAsyncEnumerable` values are only supported using the asynchronous serialization methods. Attempting to serialize using the synchronous methods will result in a `NotSupportedException` being thrown.

##### Streaming deserialization

Streaming deserialization required a new API that returns `IAsyncEnumerable<T>`. We added the `JsonSerializer.DeserializeAsyncEnumerable` method for this purpose, as you can see in the following example.

```c#
using System;
using System.IO;
using System.Text;
using System.Text.Json;

var stream = new MemoryStream(Encoding.UTF8.GetBytes("[0,1,2,3,4]"));
await foreach (int item in JsonSerializer.DeserializeAsyncEnumerable<int>(stream))
{
  Console.WriteLine(item);
}
```

This example will deserialize elements on-demand and can be useful when consuming particularly large data streams. It only supports reading from root-level JSON arrays, although that could be relaxed in the future based on feedback.

The existing DeserializeAsync method nominally supports `IAsyncEnumerable<T>`, but within the confines of its non-streaming method signature. It must return the final result as a single value, as you can see in the following example.

```c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""Data"":[0,1,2,3,4]}"));
var result = await JsonSerializer.DeserializeAsync<MyPoco>(stream);
await foreach (int item in result.Data)
{
  Console.WriteLine(item);
}

public class MyPoco
{
  public IAsyncEnumerable<int> Data { get; set; }
}
```

In this example, the deserializer will have buffered all `IAsyncEnumerable` contents in memory before returning the deserialized object. This is because the deserializer needs to have consumed the entire JSON value before returning a result.

#### System.Text.Json: Writable DOM Feature

The writeable JSON DOM feature adds a new straightforward and high-performance programming model for `System.Text.Json`. This new API is attractive since it avoids the complexity and ceremony of serialization and the traditional cost of a DOM.

This new API has the following benefits:

* A lightweight alternative to serialization for cases when use of POCO types is not possible or desired, or when a JSON schema is not fixed and must be inspected.
* Enables efficient modification of a subset of a large tree. For example, it is possible to efficiently navigate to a subsection of a large JSON tree and read an array or deserialize a POCO from that subsection. LINQ can also be used with that.
* Enables using the C# dynamic keyword, which allows for a loosely-typed, more script-like model.
We’re looking for feedback on support for dynamic. Please give us your feedback if dynamic support is important to you.

More details are available at dotnet/runtime #6098.

#### Writeable DOM APIs

The writeable DOM exposes the following types.

```c#
namespace System.Text.Json.Node
{
  public abstract class JsonNode {...};
  public sealed class JsonObject : JsonNode, IDictionary<string, JsonNode?> {...}
  public sealed class JsonArray : JsonNode, IList<JsonNode?> {...};
  public abstract class JsonValue : JsonNode {...};
}
```

**Example code**

The following example demonstrates the new programming model.

```c#
// Parse a JSON object
JsonNode jNode = JsonNode.Parse("{"MyProperty":42}");
int value = (int)jNode["MyProperty"];
Debug.Assert(value == 42);
// or
value = jNode["MyProperty"].GetValue<int>();
Debug.Assert(value == 42);

// Parse a JSON array
jNode = JsonNode.Parse("[10,11,12]");
value = (int)jNode[1];
Debug.Assert(value == 11);
// or
value = jNode[1].GetValue<int>();
Debug.Assert(value == 11);

// Create a new JsonObject using object initializers and array params
var jObject = new JsonObject
{
    ["MyChildObject"] = new JsonObject
    {
        ["MyProperty"] = "Hello",
        ["MyArray"] = new JsonArray(10, 11, 12)
    }
};

// Obtain the JSON from the new JsonObject
string json = jObject.ToJsonString();
Console.WriteLine(json); // {"MyChildObject":{"MyProperty":"Hello","MyArray":[10,11,12]}}

// Indexers for property names and array elements are supported and can be chained
Debug.Assert(jObject["MyChildObject"]["MyArray"][1].GetValue<int>() == 11);
```
