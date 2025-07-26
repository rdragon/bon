# Bon
A fast and efficient binary serializer for C#.

## Quick start
- Add the `Bon` nuget package to your project.
- Decorate the class you want to serialize with attributes as in the following example:
```cs
[BonObject]
class Person
{
    [BonMember(1)]
    public int Age { get; set; }
}
```

- Add the following empty partial class to your project:
```cs
[BonSerializerContext]
partial class BonSerializerContext : IBonSerializerContext { }
```

- Start serializing:
```cs
BonSerializer serializer = await BonSerializer.CreateAsync(new BonSerializerContext(), "schemas");
byte[] bytes = serializer.Serialize(new Person { Age = 42 });
Person person = await serializer.DeserializeAsync<Person>(bytes);
Console.WriteLine(person.Age);
```

## Features
### Compact output
The output of the serializer is a compact binary message. The message consists of a header and a body. Typically the header is around ten bytes, but it can be as short as one byte for simple messages. The body contains almost exclusively raw data. No type information or member IDs are stored in the body.

Here are some examples of how much space certain types require in the body (the `Person` class is from the quick start paragraph):

| Type | Bytes | Description |
| --- | --- | --- |
| `int` | 4 | `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong` take up the same amount of space as in memory. |
| `Person` | 4 | A class takes up the same amount of space as the sum of its members. |
| `Person[]` | x + n * 4 | An array is prefixed by a variable-width integer denoting its length. `x` is equal to 1, 2, 3, or 5 bytes. `n` is the length of the array. |
| `long?` | 1, 2, 3, 5, or 9 | `byte?`, `sbyte?`, ..., `ulong?` are serialized as variable-width integers. |
| `Person?` | 1 or 5 | A nullable class takes up one extra byte if the value is not null. |

### Forward and backward compatible
Adding or removing a field from a class (or struct) is supported. If during deserialization a field cannot be found in the message then that field is given a default value. If instead the message contains a field that is not found in the resulting class then that field is skipped and its value is lost.

Changing the name of a type or field is supported. These names do not affect the serialization or deserialization.

A number of type changes are also possible:
- All numerical types are compatible. [Unchecked casts](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions) are used to convert one type into the other, except when this would result in an overflow exception. See the unit tests in `Bon.Serializer.Test.BonSerialization.NativeConversions` for what happens in those cases.

- The type `bool` is considered a numerical type. Numbers for which the absolute value is strictly smaller than `1` are mapped to `false`. All other numbers are mapped to `true`. Conversely, `false` is mapped to `0` and `true` is mapped to `1`.

- The type `string` is compatible will all numerical types. Conversion examples:

| Value | Target type | Result |
| ----- | ----------- | ------ |
| `3.7` | `string` | `"3.7"` |
| `true` | `string` | `"1"` |
| `"3.7"` | `double` | `3.7` |
| `"3.7"` | `int` | `3` |

- A change in nullability is allowed.

- Changing from a single value to an array is possible. The single value is converted to an array of length one. See `ElementToCollectionTest.cs` for more details. 

- Changing from an array to a single value is possible. The first element in the array will be selected. See `CollectionToElementTest.cs` for more details.

### Nullability support
Both nullable value types and nullable reference types are supported. A non-nullable reference typed member will receive a default non-null value if the message does not contain a value for the member.

The default value for a collection is an empty collection. The default value for a class (or struct) is one with all its serializable members set to their default value.

## Serialization

### Serializable types
The following types can be serialized:
- Native types: `string`, `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `decimal`, `char`
- Commonly used library types: `Guid`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `ValueTuple<T1,T2>`, `ValueTuple<T1,T2,T3>`
- Collections: `T[]`, `List<T>`, `IList<T>`, `IReadOnlyList<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IEnumerable<T>`
- Dictionaries: `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`
- Any class, struct or interface decorated with the `[BonObject]` attribute
- Enums
- Nullable versions of all of the above

### Attributes
Serialization is controlled trough the use of the following attributes.

#### BonObject
Any class, struct or interface that you want to be serializable should be decorated with the `[BonObject]` attribute. This attribute can also be given to enums. For enums the attribute is only required if the enum cannot be found inside another serializable type.

#### BonMember and BonIgnore
You need to specify which properties and fields of a type should be serialized. This is done through the use of the `[BonMember]` and `[BonIgnore]` attributes. Every public property (or field) of a serializable type should be given exactly one of these attributes, otherwise the code will not compile. Private members cannot be serialized.

The `[BonMember]` attribute has one required parameter of type `int`. This parameter specifies the ID of the member. The ID is used during serialization and deserialization to identify the member. It has the same purpose as the name of the member when using JSON. If you change the ID of a member then any existing values stored for that member in the serialized data are no longer mapped to that member and might get lost.

Every member should have a unique non-negative ID. You are free to choose any IDs you like. The IDs are not included in the serializer output. They are written as variable-width integers to the schema file. Using very large IDs will make the schema file a few bytes per ID larger than using small IDs, but you will likely not notice this.

Every serializable member should have a setter, or there should be a suitable constructor. A suitable constructor is a constructor that has for every serializable member one argument with the same name (case insensitive) and same type. If found, this constructor will be used during deserialization, even if all members have setters.

#### BonReservedMembers
If you delete a property or field from a class (or struct) then you should not re-use its ID. Otherwise, if you deserialize older data, the new property might receive values from the deleted member. To prevent this scenario you can use the `[BonReservedMembers]` attribute. For example, decorating a type with `[BonReservedMembers(3, 5)]` makes sure the code no longer compiles if a member with ID 3 or 5 is added to the type.

#### BonInclude
To be able to serialize interfaces and abstract classes you need to use the `[BonInclude]` attribute. This attribute is used as follows:

```cs
[BonObject]
[BonInclude(1, typeof(Dog))]
[BonInclude(2, typeof(Cat))]
interface IAnimal;

[BonObject] class Dog : IAnimal;
[BonObject] class Cat : IAnimal;
```

The first parameter of the attribute is an identifier of type `int`. Each implementation should have a unique identifier within the context of a single interface or abstract class. The identifiers do not have to be unique across types. Each time an interface or abstract class is serialized the right identifier is written as variable-width integer to the serialized data. Therefore, it is beneficial to use small instead of large identifiers.

## BON format
The serializer outputs a binary message that consists of the following parts:

```
message = schema + body
```

These parts can be described as follows:

- `schema`: one or more bytes determining the schema used by the `body` part
- `body`: the actual payload of the message.

### The schema part
The `schema` part consists itself of three parts:

```
schema = schema_type + additional_data
```

These parts can be described as follows:

- `schema_type`: one byte representing one of the following values: `record`, `record?`, `union`, `array`, `dictionary`, `tuple2`, `tuple2?`, `tuple3`, `tuple3?`, `string`, `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `unsigned_whole_number`, `signed_whole_number`, `float`, `double`, `double?`, `decimal?`.
- `additional_data`: zero or more bytes containing additional data.

The additional data depends on the schema type:

| Schema type | Additional data |
| ----------- | --------------- |
| `record`, `union` | A variable-width integer equal to the ID of a schema from the schema file. |
| `array`, `dictionary`, `tuple2`, `tuple3` | One or more `schema` parts representing the type parameters of the generic type. |

The other schema types do not have any additional data.

### The body
The schema determines the format of the body. Therefore the body does not include any type information or member IDs. The different types are serialized to the body as follows.

#### The value null
The value `null` is always serialized as a single byte `FF`, regardless of the compile time type of the value.

#### Classes and structs
For non-nullable classes and structs only the values of the members (properties and fields) are serialized, without any delimiters or member IDs. For example, a class with a single `int` field takes up the same amount of space as an `int`. All members with a `[BonMember]` attribute are serialized, also members that are null. Each member that is null takes up one byte.

For nullable classes and structs the serialization starts with a single byte `00`. After that the serialization is the same as that for non-nullable classes and structs.

#### Interfaces and abstract classes
The serialization starts with a variable-width integer equal to the ID of the (non-abstract) class (or struct) that is serialized. This is the ID specified in the `[BonInclude]` attribute. Then the class is serialized in the usual way.

#### Arrays
The serialization starts with a variable-width integer equal to the number of elements in the array. Then the elements in the array are serialized one after the other without any delimiters.

#### Dictionaries
The serialization starts with a variable-width integer equal to the number of key-value pairs in the dictionary. Then keys and values in the dictionary are serialized alternatingly without any delimiters.

#### Tuples
Tuples are serialized in the same way as other classes and structs.

#### byte, sbyte, short, ushort, int, uint, long, ulong
These types are serialized in little-endian order and take up their native amount of space. For example, an `int` takes up 4 bytes and a `long` takes up 8 bytes.

The reason these types are not serialized as variable-width integers is that that would slow down the serialization and deserialization.

#### byte?, sbyte?, short?, ushort?, int?, uint?, long?, ulong?
These types are serialized as variable-width integers.

#### Unsigned variable-width integer
An unsigned variable-width integer (called `WholeNumber` in the code) takes up 1, 2, 3, 5 or 9 bytes and has the following formats:

| Format | Number range |
| ------ | ------------ |
| `0xxxxxxx` | 0 – 127 |
| `10xxxxxx B` | 128 – 16,383 |
| `110xxxxx BB` | 16,384 – 2,097,151 |
| `1110xxxx BBBB` | 2,097,152 – 2^36 - 1 |
| `11110000 BBBBBBBB` | 2^36 – 2^64 - 1 |

Here `B` represents one byte and a string like `0xxxxxxx` represents one byte with the most significant bit set to `0`.

The `x`s in the first byte are the most significant bits of the number. The `B` bytes contain the rest of the bits and are written in little-endian order. For example, the number `127` is written as `01111111` and the number `256` is written as `10000001 00000000`.

#### Signed variable-width integer
If a number is serialized as signed variable-width integer then the number is first converted to an unsigned number by applying the [ZigZag encoding](https://protobuf.dev/programming-guides/encoding/). After that, the number is serialized in the same way as an unsigned variable-width integer.

The reason for using ZigZag encoding instead of two's complement is that this prevents negative values to always take up 9 bytes.

#### Rest
- A `string` is first converted to a UTF-8 byte array and then this array is serialized like any other array.
- A char is serialized as a variable-width integer.
- A `float` takes up 4 bytes, a `float?` 5 bytes (if not null).
- A `double` takes up 8 bytes, a `double?` 9 bytes (if not null).
- A `decimal` takes up 16 bytes, a `decimal?` 17 bytes (if not null).
- A `Guid` takes up 16 bytes, a `Guid?` 17 bytes (if not null).
- An enum is serialized exactly like its underlying type.
- Both `DateTime` and `DateTimeOffset` are serialized as `long` containing the number of UTC ticks. Offset information is not included.
- Both `TimeSpan` and `TimeOnly` are serialized as `long` containing the number of ticks.
- A `DateOnly` is serialized as `int` containing the day number.

## Benchmarks
Using BenchmarkDotNet we compare the following serializers: Bon, [MessagePack](https://github.com/MessagePack-CSharp/MessagePack-CSharp), [protobuf-net](https://github.com/protobuf-net/protobuf-net) and `System.Text.Json`. The code can be found in the `Bon.Benchmarks` project.

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4037/23H2/2023Update/SunValley3)
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.400
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
```

### NumberStruct
Here are the results of deserializing an array of 100,000 instances of the following struct.

```cs
record struct NumberStruct(long Long, int Int, short Short, byte Byte, bool Bool);
```

| Method      | Mean      | Error     | StdDev    | Ratio | Gen0     | Gen2     | Allocated |
|------------ |----------:|----------:|----------:|------:|---------:|---------:|----------:|
| Bon         |  1.737 ms | 0.0250 ms | 0.0234 ms |  1.00 | 261.7188 | 261.7188 |   1.53 MB |
| MessagePack |  3.972 ms | 0.0153 ms | 0.0144 ms |  2.29 | 312.5000 | 312.5000 |   1.53 MB |
| Protobuf    |  6.487 ms | 0.1273 ms | 0.1867 ms |  3.74 |  62.5000 |  62.5000 |   1.53 MB |
| Json        | 37.840 ms | 0.0863 ms | 0.0765 ms | 21.79 | 285.7143 | 142.8571 |   8.58 MB |

The size of the serialized array is as follows.

| Method      | Size (bytes) | Ratio |
|------------ |-------------:|------:|
| Bon         |    1,600,015 |  1.00 |
| MessagePack |    2,049,080 |  1.28 |
| Protobuf    |    2,697,729 |  1.69 |
| Json        |    8,259,036 |  5.16 |

### Product
Here are the results of deserializing an array of 100,000 instances of the following class.

```cs
// `IntArray` and `Features` are of length between 1 and 10.
record class Product(int Int, int[] IntArray, Feature[] Features);
record struct Feature(int Int, float Float);
```

| Method      | Mean      | Error    | StdDev   | Ratio | Gen0      | Gen2     | Allocated |
|------------ |----------:|---------:|---------:|------:|----------:|---------:|----------:|
| Bon         |  18.81 ms | 0.372 ms | 0.786 ms |  1.00 | 1343.7500 | 437.5000 |  15.62 MB |
| MessagePack |  28.27 ms | 0.458 ms | 0.382 ms |  1.51 | 1250.0000 | 375.0000 |  15.62 MB |
| Protobuf    |  54.00 ms | 0.841 ms | 0.787 ms |  2.88 | 1222.2222 | 333.3333 |  15.62 MB |
| Json        | 254.68 ms | 2.346 ms | 2.080 ms | 13.57 | 4666.6667 | 666.6667 |  68.87 MB |

The size of the serialized array is as follows.

| Method      | Size (bytes) | Ratio |
|------------ |-------------:|------:|
| Bon         |    7,181,187 |  1.00 |
| MessagePack |    9,573,781 |  1.33 |
| Protobuf    |   10,047,057 |  1.40 |
| Json        |   30,530,749 |  4.25 |

### Person
Here are the results of deserializing an array of 100,000 instances of the following class.
```cs
// `String1` and `String2` are of length between 1 and 20.
record class Person(string String1, string String2, DateTime DateTime1, DateTime DateTime2, int Int1, int Int2);
```

| Method      | Mean      | Error    | StdDev   | Ratio | Gen0      | Gen2     | Allocated |
|------------ |----------:|---------:|---------:|------:|----------:|---------:|----------:|
| Bon         |  19.99 ms | 0.019 ms | 0.016 ms |  1.00 | 1281.2500 | 406.2500 |  14.87 MB |
| MessagePack |  27.30 ms | 0.030 ms | 0.026 ms |  1.37 | 1281.2500 | 406.2500 |  14.87 MB |
| Protobuf    |  34.22 ms | 0.079 ms | 0.062 ms |  1.71 | 1266.6667 | 400.0000 |  14.87 MB |
| Json        | 110.81 ms | 1.704 ms | 1.510 ms |  5.54 | 2600.0000 | 800.0000 |  32.13 MB |

The size of the serialized array is as follows.

| Method      | Size (bytes) | Ratio |
|------------ |-------------:|------:|
| Bon         |    4,696,186 |  1.00 |
| MessagePack |    6,341,276 |  1.35 |
| Protobuf    |    6,491,572 |  1.38 |
| Json        |   17,762,964 |  3.78 |

## Limitations
### Schema files
To be able to serialize and deserialize the serializer needs access to a schema file. This file is created and updated automatically. If you don't have access to this file you cannot deserialize any custom types.

The contents of this file remain in memory as long as the serializer is alive.

The `Bon.Azure` project contains the `AzureBlob` class that can be used to store the file in the Azure Blob Storage instead of on the file system. This project is included in the `Bon` nuget package.

### Custom serialization is not supported
Currently the only way to control the serialization of a type is through the use of attributes like `BonMemberAttribute`. There is no way to hook into the (de)serialization process.

This also means that apart from the types that are supported by default, the only types that can be serialized are types decorated with the `[BonObject]` attribute. You cannot add serialization support for types that you do not own.

There is one exception to this rule and that are enum types. Enum types do not require the `[BonObject]` attribute if they are used inside a serializable type.

### All custom types need to be defined in the same project
Currently all types marked with the `[BonObject]` attribute need to be defined in the same project, or you need to use multiple `BonSerializer` instances. This is because `BonSerializer` can only be given one `IBonSerializerContext`, and an `IBonSerializerContext` only provides information about the types inside a single project.

One way to cope with this limitation is to put all the serializable types in a separate project.

### Generic classes
Generic classes are supported. However, only closed constructed types that are used inside at least one serializable non-generic type can be serialized.

For example, suppose a non-generic type with the `[BonObject]` attribute has a member of type `Node<int>`. Then `Node<int>` can be serialized, also on its own. However, if you try to serialize `Node<long>` then you'll receive an error, except if `Node<long>` can also be found inside a serializable non-generic type.

### BON is not human readable
The output of the serializer is not human readable. However, the `BonSerializer` class does contain methods to convert BON to JSON and back. The JSON will not contain any field names.

### No cross-language support
Currently there is only one serializer for BON, so if you want to use BON to communicate between two applications, both applications need to use C#.

### .NET 8 or higher is required
The serializer uses a source generator which generates C# code that uses C# 12.0 features. Therefore the serializer can only be used in projects that target .NET 8 or higher.

## Security
Only deserialize trusted data. There is at least one vulnerability when deserializing untrusted data.

### Large memory allocations
It is possible to create messages that cause large amount of memory to be allocated. For this you can create a message that contains an array and specify in the message that the array contains more elements than it actually does. The way the deserializer works is that it first allocates an array of the appropriate size and only then starts reading the elements.

This problem could be fixed at the cost of some performance. However, this would not completely solve the issue. If a schema exists of a type without any members then this schema could be used as schema of the elements in the array. Now the elements in the array do not take up any space in the message. In this way you can make a small valid message that contains a very large array. A type without members can be useful as value type in a dictionary when you use the dictionary as hash set.
