### LiteDB

[LiteDB](http://www.litedb.org/) — встраиваемая (serverless) СУБД, не требующая установки. Она представляет собой простую сборку для декстопного .NET 3.5 размером менее 250 Кб, написанную полностью на C# и не требующую больше никаких компонентов (есть также вариант для Net Standard 1.4). Её можно подлючить к своему проекту [с помощью NuGet](https://www.nuget.org/packages/LiteDB/) или простым копированием.

LiteDB очень простая и сравнительно быстрая база данных NoSQL. Её можно применять в следующих сценариях:

* Мобильное приложение;
* Небольшое десктопное или портативное приложение;
* В качестве файлового формата приложений;
* Небольшое веб-приложение;
* База данных одновременно используется не более чем одним пользователем.

Кроме того:

* Подходит для портативных приложений UWP или Xamarin iOS/Android;
* Поддерживает транзакции;
* Одна база — один файл (как SQLite);
* Возможность восстановления данных при сбое записи (режим журналирования);
* Работает с простыми POCO-классами, преобразуя их в BsonDocument;
* Fluent API для пользовательского маппинга классов в BsonDocument;
* Ссылки между коллекциями (DbRef);
* Можно хранить файлы и потоковые данные (подобно GridFS в MongoDB);
* Поддерживает LINQ;
* Бесплатна для всех, включая коммерческие проекты.

https://www.nuget.org/packages/LiteDB/

Простейший пример

```csharp
// Create your POCO class entity
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] Phones { get; set; }
    public bool IsActive { get; set; }
}
 
// Open database (or create if doesn't exist)
using(var db = new LiteDatabase(@"C:\Temp\MyData.db"))
{
    // Get a collection (or create, if doesn't exist)
    var col = db.GetCollection<Customer>("customers");
 
    // Create your new customer instance
    var customer = new Customer
    { 
        Name = "John Doe", 
        Phones = new string[] { "8000-0000", "9000-0000" }, 
        IsActive = true
    };
     
    // Insert new customer document (Id will be auto-incremented)
    col.Insert(customer);
     
    // Update a document inside a collection
    customer.Name = "Joana Doe";
     
    col.Update(customer);
     
    // Index document using document Name property
    col.EnsureIndex(x => x.Name);
     
    // Use LINQ to query documents
    var results = col.Find(x => x.Name.StartsWith("Jo"));
}
```

Требования к POCO-классам

* Classes must be public with a public parameterless constructor;
* Properties must be public;
* Properties can be read-only or read/write;
* The class must have an Id property, \<ClassName\>Id property or any property with [BsonId] attribute or mapped by fluent api;
* A property can be decorated with [BsonIgnore] to not be mapped to a document field;
* A property can be decorated with [BsonField] to customize the name of the document field;
* No circular references are allowed;
* Max depth of 20 inner classes;
* Class fields are not converted to document;
* You can use BsonMapper global instance (BsonMapper.Global) or a custom instance and pass to LiteDatabase in constructor. Keep this instance in a single place to avoid re-creating all mapping each time you use database.

Хранение файлов

```csharp
// Store files
using(var db = new LiteDatabase("MyData.db"))
{
    // Upload a file from file system
    db.FileStore.Upload("/my/file-id", @"C:\Temp\picture1.jgn");
     
    // Upload a file from Stream
    db.FileStore.Upload("/my/file-id", myStream);
     
    // Open as an stream
    var stream = db.FileStore.OpenRead("/my/file-id");
     
    // Write to another stream
    stream.CopyTo(Response.Output);
     
}
```

Настройка маппинга

```csharp
// Custom entity mapping to BsonDocument
             
// Re-use mapper from global instance
var mapper = BsonMapper.Global;            
 
mapper.Entity<Customer>()
        .Key(x => x.CustomerKey)
        .Field(x => x.Name, "customer_name")
        .Ignore(x => x.Age)
        .Index(x => x.Name, unique);
             
using(var db = new LiteDatabase(@"MyData.db"))
{
    var doc = mapper.ToDocument(new Customer { ... });
    var json = JsonSerializer.Serialize(doc, true);
     
    /* json:
    {
        "_id": 1,
        "customer_name": "John Doe"
    }
    */
}
```

Поток как база данных (БД в памяти)

```csharp
// In memory database
var mem = new MemoryStream();
 
using(var db = new LiteDatabase(mem))
{
    ...
}
 
// Get database as binary array
var bytes = mem.ToArray();
 
// LiteDB support any Stream read/write as input
// You can implement your own IDiskService to persist data
```

DbRef для ссылок между коллекциями

```csharp
public class Order
{
    public ObjectId Id { get; set; }
    public DateTime OrderDate { get; set; }
    public Customer Customer { get; set; }
    public List<Product> Products { get; set; }
}        
 
// Re-use mapper from global instance
var mapper = BsonMapper.Global;
 
// Produts and Customer are other collections (not embedded document)
mapper.Entity<Order>()
    .DbRef(x => x.Products, "products")
    .DbRef(x => x.Customer, "customers");
             
using(var db = new LiteDatabase(@"MyData.db"))
{
    var orders = db.GetCollection<Order>("orders");
         
    // When query Order, includes references
    var query = orders
        .Include(x => x.Customer)
        .Include(x => x.Products)
        .Find(x => x.OrderDate <= DateTime.Now);
 
    // Each instance of Order will load Customer/Products references                   
}
```

##### Строка подключения

* Filename (string): Full path or relative path from DLL directory.
* Journal (bool): Enabled or disable double write check to ensure durability (default: true)
* Password (string): Encrypt (using AES) your datafile with a password (default: null — no encryption)
* Cache Size (int): Max number of pages in cache. After this size, flush data to disk to avoid too memory usage (default: 5000)
* Timeout (TimeSpan): Timeout for waiting unlock operations (thread lock and locked file)
* Mode (Exclusive|ReadOnly|Shared): How datafile will be open (defult: Shared in NET35 and Exclusive in NetStandard)
* Initial Size (string|long): If database is new, initialize with allocated space — support KB, MB, GB (default: null)
* Limit Size (string|long): Max limit of datafile — support KB, MB, GB (default: null)
* Upgrade (bool): If true, try upgrade datafile from old version (v2) (default: null)
* Log (byte): Debug messages from database — use LiteDatabase.Log (default: Logger.NONE)
* Async (bool): Support «sync over async» file stream creation to use in UWP access any disk folder (only for NetStandard, default: false)

##### Командная оболочка

Имеется shell, который позволяет манипулировать базами LiteDB из командной строки.

The commands here works only in console application LiteDB.Shell.exe:

* help [full] — Display basic or full commands help
* open \<filename|connectionString\> — Open new database. If not exists, create a new one. Can be passed a connection string with all options (like password or timeout)
* close — Close current database
* pretty — Show JSON in pretty format
* ed — Open notepad.exe with last command to edit
* run \<filename\> — Read filename and add each line as a new command
* spool [on|off] — Turn on/off spool of all commands
* timer — Show a timer on command prompt
* -- — Comments (ignore rest of line)
* /\<command\>/ — Supports multiline command
* upgrade /\<filename|connectionString\>/ — Upgrade datafile from LiteDB v2
* version — Show LiteDB assembly version
* quit — Exit shell application

##### Collections commands

Syntax: `db.<collection>.<command>`

* insert \<jsonDoc\> — Find a document using query filter syntax
  * db.customer.insert { _id:1, Name: "John Doe", Age: 38 }
  * db.customer.insert { Name: "John Doe" } — Auto create Id as ObjectId

* bulk \<filename.json\> — Insert all documents inside a JSON file. JSON file must be an JSON array of documents
  * db.customer.bulk my-documents.json

* update \<jsonDoc\> — Update a existing document using _id
  * db.customer.update { _id:1, Name: "Joana Doe", Age: 29 }

* delete \<filter\> — Delete documents using filter syntax
  * db.customer.delete _id = 1
  * db.customer.delete Name like "Jo"

* ensureIndex \<field\> [true|\<jsonOptions\>] — Create a new index on collection in field. Support json options See Indexes
  * db.customers.ensureIndex Name true — Create a unique index
  * db.customers.ensureIndex Name { unique: true, removeAccents: false }

* indexes — List all indexes on collections
  * db.customers.indexes

* dropIndex \<field\> — Drop an index
  * db.customers.dropIndex Name

* drop — Drop a collection and all documents inside. Return false in not exists
  * db.customer.drop

* rename — Rename a collection. Return true if success or false in an error occurs
  * db.customer.rename my_customers

* count \<filter\> — Count documents with defined filter. See \<filter\> syntax below
  * db.customer.count Name = "John Doe"
  * db.customer.count Age between [20, 40]

* min \<field\> — Get lowest value in field index
  * db.customer.min _id
  * db.customer.min Age

* max \<field\> — Get biggest value in field index
  * db.customer.max _id
  * db.customer.max Age

* find [filter][skip N][limit M] — Find documents using query filter syntax. See \<filter\> syntax below
  * db.customer.find
  * db.customer.find limit 10
  * db.customer.find Name = "John Doe"
  * db.customer.find Age between [20, 40]
  * db.customer.find Name LIKE "John" and Age \> 30

* \<filter\> = \<field\> [=|>|>=|<|<=|!=|like|contains|in|between] \<jsonValue\>
* \<filter\> = \<filter\> [and|or] \<filter\>
* \<jsonDoc\> and \<jsonValue\> are JSON extended format. See Data Structure.

##### FileStorage

Syntax: fs.\<command\>

* upload \<fileId\> \<filename\> — Upload a new local file to database. If fileId exists, override data content
  * fs.upload my-file picture.jpg
  * fs.upload $/photos/pic-001 C:\Temp\mypictures\picture-1.jpg

* download \<fileId\> \<filename\> — Download existing database file to a local file.
  * fs.download my-file copy-picture.jpg
  * fs.download $/photos/pic-001 C:\Temp\mypictures\copy-picture-1.jpg

* update \<fileId\> \<jsonDoc\> — Update file metadata
  * fs.update my-file { user: "John", machine: "srv001" }

* delete \<fileId\> — Delete a file
  *fs.delete my-file

* find [fileId] — List all files inside database or starting with fileId parameter
  * fs.find
  * fs.find $/photos/

##### Database utils

Syntax: `db.<command>`

* userversion [N] — Get/Set user database file version
* shrink [password] — Reduce database removing empty pages and change password (optional). If password was not provide, new datafile will be not encrypted.


