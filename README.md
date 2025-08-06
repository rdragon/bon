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
BonSerializer serializer = await BonSerializer.CreateAsync(new BonSerializerContext(), new FileSystemBlob());
byte[] bytes = serializer.Serialize(new Person { Age = 42 });
Person? person = serializer.Deserialize<Person>(bytes);
Console.WriteLine(person?.Age); // 42
```

## Features
### Compact output
The output of the serializer is a compact binary message. The message consists of a header and a body. The header is a few bytes long (the minimum is one byte) and specifies the schema used by the body. The body contains almost exclusively raw data. No type information or member IDs are stored in the body.

Here are some examples of how much space certain types require in the body (the `Person` class is from the quick start paragraph):

| Type | Bytes | Description |
| --- | --- | --- |
| `int` | 4 | `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` take up the same amount of space as in memory. |
| `long?` | 1, 2, 3, 5, or 9 | `byte?`, `sbyte?`, ..., `ulong?` are serialized as variable-width integers. |
| `Person` | 1 + 4 | An instance of a class takes up the same amount of space as the sum of its members, plus one extra byte to indicate that the value is not null. |
| `null` | 1 | A null value always takes up a single byte, no matter the type. |
| `Person[]` | x + n * 5 | An array is prefixed by a variable-width integer denoting its length. For example, `x = 1` if `n < 128`. |
| `(int, int)` | 4 + 4 | A tuple takes up the same amount of space as the sum of its members. The same is true for structs. |

### Schema file
Even for a complex class the header of a binary message is fairly short, typically around 3 bytes. This is possible due to a schema file that contains the details about how each type is serialized. This schema file can (for example) be stored in the Azure Blob Storage, see the `Bon.Azure` project. The `BonSerializer.CreateAsync` method loads the schema file and updates it if it is missing information about any of the serializable types.

### Forward and backward compatible
Adding or removing a member from a class (or struct) is supported. Members that are missing from a message are set to `default`. Data that cannot be mapped to a member is lost.

Changing the name of a type or member is supported. Names do not affect the serialization or deserialization.

A number of type changes are also possible:
- All numerical types are compatible. [Unchecked casts](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/numeric-conversions) are used to convert one type into the other.

- The type `string` is compatible will all numerical types. Conversion examples:

| Value | Target type | Result |
| ----- | ----------- | ------ |
| `3.7` | `string` | `"3.7"` |
| `"3.7"` | `double` | `3.7` |
| `"3.7"` | `int` | `3` |
| `true` | `string` | `"1"` |
| `false` | `string` | `"0"` |

- A change in nullability is allowed.

### Serializable types
The following types can be serialized:
- Native types: `string`, `bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `decimal`, `char`
- Commonly used library types: `Guid`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `ValueTuple<T1,T2>`, `ValueTuple<T1,T2,T3>`
- Collections: `T[]`, `List<T>`, `IList<T>`, `IReadOnlyList<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IEnumerable<T>`, `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`
- Any class, struct or interface decorated with the `[BonObject]` attribute
- Enums

## Attributes
Serialization is controlled trough the use of the following attributes.

### BonObject
Any class, struct or interface that you want to be serializable should be decorated with the `[BonObject]` attribute. This attribute can also be given to enums. For enums the attribute is only required if the enum cannot be found inside another serializable type.

### BonMember and BonIgnore
You need to specify which members of a type should be serialized. This is done through the use of the `[BonMember]` and `[BonIgnore]` attributes. Every public property or field of a serializable type should be given one of these attributes, otherwise the code will not compile. Private members cannot be serialized.

The `[BonMember]` attribute has one required parameter of type `int`. This parameter specifies the ID of the member. The ID is used during serialization and deserialization to identify the member. It has the same purpose as the name of the member when using JSON. Changing the ID of a member is identical to deleting the member and creating a new member. This will cause data to be lost.

Every member should have a unique non-negative ID. You are free to choose any IDs you like. The IDs are not included in the serializer output. They are written as variable-width integers to the schema file.

Every serializable member should have a setter, or there should be a suitable constructor. A suitable constructor is a constructor that has for every serializable member one argument with the same name (case insensitive) and same type. If found, this constructor will be used during deserialization, even if all members have setters.

### BonReservedMembers
If you delete a member from a class (or struct) then you should not re-use its ID. Otherwise, if you deserialize older data, the new member might receive incorrect values. To prevent this scenario you can use the `[BonReservedMembers]` attribute. For example, decorating a type with `[BonReservedMembers(3, 5)]` makes sure the code no longer compiles if a member with ID 3 or 5 is added to the type.

### BonInclude
To be able to serialize interfaces and abstract classes you need to use the `[BonInclude]` attribute. This attribute is used as follows:

```cs
[BonObject]
[BonInclude(1, typeof(Cat))]
[BonInclude(2, typeof(Dog))]
interface IAnimal;

[BonObject] class Cat([property: BonMember(1)] int Age) : IAnimal;
[BonObject] class Dog : IAnimal;
```

The first parameter of the attribute is an identifier of type `int`. Each implementation should have a unique identifier within the context of a single interface or abstract class. The identifiers do not have to be unique across types. Each time an interface or abstract class is serialized the right identifier is written as variable-width integer to the serialized data. Therefore, it is beneficial to use small instead of large identifiers.

## BON format
The serializer outputs a binary message that consists of a header and a body.

### The header
The header consists solely of a schema. A schema looks as follows:

```
schema = schema_type + [schema_arguments] + [layout_id]
```

These parts can be described as follows:

- `schema_type`: one byte representing the type of the schema, e.g. `int`, `record`, or `array`.
- `schema_arguments`: optional, only for generic types. One or more `schema` parts representing the type parameters of the generic type.
- `layout_id`: optional, only for custom classes, structs and interfaces. A variable-width integer equal to the ID of a layout from the schema file.

### The body
The schema determines the format of the body. Therefore the body does not include any type information or member IDs. The different types are serialized to the body as follows.

#### The value null
The value `null` is serialized as a single byte `FF`.

#### Classes and structs
Classes and nullable structs start with a single byte `00`, denoting the value is not null. Then, the members of the type are serialized, ordered by member ID. The members are serialized without any delimiters or member IDs. Also, no member count is included. Each member that is null takes up one byte.

The reason why no delimiters or member IDs are included is that the schema already contains all this information.

Non-nullable structs are serialized in the same way, except that the starting byte `00` is omitted.

#### Interfaces and abstract classes
The serialization starts with a variable-width integer equal to the ID of the (non-abstract) class (or struct) that is serialized. This is the ID specified in the `[BonInclude]` attribute. Then the class is serialized in the usual way.

#### Arrays
The serialization starts with a variable-width integer equal to the number of elements in the array. Then the elements in the array are serialized one after the other without any delimiters.

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

The reason for using ZigZag encoding instead of two's complement is that this prevents negative values from always taking up 9 bytes.

#### String
A `string` is serialized starting with a nullability byte followed by the output of `BinaryWriter.Write(string)`. UTF-8 encoding is used. The `BinaryWriter` method uses a "7-bit encoded int" for the length of the byte array, see `BinaryWriter.Write7BitEncodedInt(int)`.

#### Rest
- A char is serialized as a variable-width integer.
- A `float` takes up 4 bytes, a `float?` 5 bytes (if not null).
- A `double` takes up 8 bytes, a `double?` 9 bytes (if not null).
- A `decimal` takes up 16 bytes, a `decimal?` 17 bytes (if not null).
- An enum is serialized exactly like its underlying type.
- Both `DateTime` and `DateTimeOffset` are serialized as `long` containing the number of UTC ticks. Offset information is not included.
- Both `TimeSpan` and `TimeOnly` are serialized as `long` containing the number of ticks.
- A `DateOnly` is serialized as `int` containing the day number.

## Limitations
### Schema file
To be able to serialize and deserialize the serializer needs access to a schema file. This file is created and updated automatically. If you don't have access to this file you cannot deserialize any custom types.

The contents of this file remain in memory as long as the serializer is alive.

The `Bon.Azure` project contains the `AzureBlob` class that can be used to store the file in the Azure Blob Storage instead of on the file system. This project is included in the `Bon` nuget package.

### Custom serialization is not supported
Currently the only way to control the serialization of a type is through the use of attributes. There is no way to hook into the (de)serialization process.

This also means that apart from the types that are supported by default, the only types that can be serialized are types decorated with the `[BonObject]` attribute. You cannot add serialization support for types that you do not own.

### All custom types need to be defined in the same project
Currently all types marked with the `[BonObject]` attribute need to be defined in the same project, or you need to use multiple `BonSerializer` instances, each with their own schema file. This limitation will be removed in the future.

### Generic classes
Generic classes are supported. However, only closed constructed types that are used inside at least one serializable non-generic type can be serialized.

For example, suppose a non-generic type with the `[BonObject]` attribute has a member of type `Node<int>`. Then `Node<int>` can be serialized, also on its own. However, if you try to serialize `Node<long>` then you'll receive an error.

### BON is not human readable
The output of the serializer is not human readable. However, the `BonSerializer` class does contain methods to convert BON to JSON and back. The JSON will not contain any type or member names.

### No cross-language support
Currently there is only one serializer for BON, so if you want to use BON to communicate between two applications, both applications need to use C#.

### .NET 8 or higher is required
The serializer uses a source generator which generates C# code that uses C# 12.0 features. Therefore the serializer can only be used in projects that target .NET 8 or higher.

## Security
Only deserialize trusted data. There is at least one vulnerability when deserializing untrusted data.

### Large memory allocations
It is possible to create messages that cause large amount of memory to be allocated. For this you can create a message that contains an array and specify in the message that the array contains more elements than it actually does. The way the deserializer works is that it first allocates an array of the appropriate size and only then starts reading the elements.

This problem could be fixed at the cost of some performance. However, this would not completely solve the issue. If a schema exists of a type without any members then this schema could be used as schema of the elements in the array. Now the elements in the array do not take up any space in the message. In this way you can make a small valid message that contains a very large array. A type without members can be useful as value type in a dictionary when you use the dictionary as hash set.
