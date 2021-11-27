// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Sprache;

class JsonEntity
{
    public virtual JsonEntity this [string name] => 
        throw new NotImplementedException ("[name]");

    public virtual JsonEntity this [int index] =>
        throw new NotImplementedException ("[index]");
}

sealed class JsonNull : JsonEntity
{
    public override string ToString() => "null";
}

sealed class JsonBool : JsonEntity
{
    public bool Value { get; }

    public JsonBool (bool value) => Value = value;

    public override string ToString() => Value.ToString().ToLowerInvariant();
}

sealed class JsonNumber : JsonEntity
{
    public double Value { get; }

    public JsonNumber (double value) => Value = value;

    public override string ToString() => 
        Value.ToString (CultureInfo.InvariantCulture);
}

sealed class JsonString : JsonEntity
{
    public string Value { get; }

    public JsonString (string value) => Value = value;

    public override string ToString() => $"\"{Value}\"";
}

sealed class JsonProperty : JsonEntity
{
    public JsonString Name { get; }
    public JsonEntity Value { get; }

    public JsonProperty(JsonString name, JsonEntity value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString() => $"{Name}:{Value}";
}

sealed class JsonArray : JsonEntity
{
    public JsonEntity[] Items { get; }

    public JsonArray(JsonEntity[] items)
    {
        Items = items;
    }

    public override JsonEntity this [int index] => Items[index];

    public override string ToString() => 
        "[" + string.Join (',', Items.AsEnumerable()) + "]";
}

sealed class JsonObject : JsonEntity
{
    public Dictionary<string, JsonProperty> Properties { get; }

    public JsonObject(JsonProperty[] properties)
    {
        Properties = new Dictionary<string, JsonProperty>();
        foreach (var property in properties)
        {
            Properties.Add (property.Name.Value, property);
        }
    }

    public override JsonEntity this [string name] => Properties[name].Value;

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append ('{');
        var first = true;
        foreach (var property in Properties.Values)
        {
            if (!first)
            {
                builder.Append (',');
            }

            builder.Append (property);

            first = false;
        }
        builder.Append ('}');
        return builder.ToString();
    }
}

static class JsonGrammar
{
    private static readonly Parser<JsonNull> Null =
        Parse.String ("null").Return (new JsonNull());

    private static readonly Parser<JsonBool> Bool =
        Parse.String ("true").Return (new JsonBool (true))
            .Or (Parse.String ("false").Return (new JsonBool (false)));

    private static readonly Parser<JsonNumber> Number =
        from text in Parse.DecimalInvariant
        let value = double.Parse (text, CultureInfo.InvariantCulture)
        select new JsonNumber (value);

    private static readonly Parser<JsonString> String =
        from open in Parse.Char ('"') 
        from value in Parse.CharExcept ('"').Many().Text()
        from close in Parse.Char ('"')
        select new JsonString (value);

    private static readonly Parser<JsonProperty> Property =
        from name in String
        from colon in Parse.Char (':').Token()
        from value in Entity
        select new JsonProperty (name, value);

    private static readonly Parser<JsonArray> Array =
        from open in Parse.Char ('[').Token()
        from items in Entity.DelimitedBy 
            (Parse.Char (',').Token())
        from close in Parse.Char (']').Token()
        select new JsonArray (items.ToArray());

    private static readonly Parser<JsonObject> Object =
        from open in Parse.Char ('{').Token()
        from properties in Property.DelimitedBy
            (Parse.Char (',').Token())
        from close in Parse.Char ('}').Token()
        select new JsonObject (properties.ToArray());

    private static readonly Parser<JsonEntity> Entity
        = Null.Or<JsonEntity> (Bool).Or (Number).Or (String)
            .Or (Property).Or (Array).Or (Object);

    public static JsonEntity ParseInput (string input) => 
        Entity.End().Parse (input);
}

static class Program
{
    static void ParseFile (string fileName)
    {
        try
        {
            var input = File.ReadAllText (fileName);
            var parsed = JsonGrammar.ParseInput (input);
            Console.WriteLine (parsed["array"][1]);
            Console.WriteLine (parsed["object"]["fifth"]);
            Console.WriteLine (parsed);
        }
        catch (Exception exception)
        {
            Console.WriteLine ($"{fileName} :: {exception}");
        }
    }

    static void Main ()
    {
        ParseFile ("first.json");
    }
}
