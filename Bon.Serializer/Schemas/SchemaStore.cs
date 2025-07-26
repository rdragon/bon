namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of a mapping between types and schemas.
/// </summary>
internal sealed class SchemaStore
{
    /// <summary>
    /// Contains for each type the schema that will be used when serializing the type.
    /// 
    /// During initialization the native types and the types from the source generation context are added to this dictionary.
    /// The latter are all the classes, structs, interfaces and enums with the <see cref="BonObjectAttribute"/> (for value
    /// types, their nullable and non-nullable versions).
    /// 
    /// Built in generic types like array and tuple types are added during serialization and deserialization.
    /// 
    /// When loading schemas from the storage this dictionary is not updated because there is no type information available in the storage.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Schema> _schemas = [];

    /// <summary>
    /// Returns the schema that will be used when serializing the given type.
    /// If no schema is found, a new schema is created.
    /// </summary>
    public Schema GetOrAddSchema(Type type) => _schemas.TryGetValue(type, out var schema) ?
        schema : GetOrAddSchema(type, CreateSchema(type));

    public void AddNativeSchemas()
    {
        // Bookmark 659516266 (native serialization)
        // All non-generic types for which the source generation context does not provide a schema should be added here.
        // This is the same set of types as can be found at bookmark 293228595.
        // This mapping should be in sync with the mapping at bookmark 988874999.
        AddSchema(typeof(string), Schema.String);
        AddSchema(typeof(bool), Schema.Byte);
        AddSchema(typeof(byte), Schema.Byte);
        AddSchema(typeof(sbyte), Schema.SByte);
        AddSchema(typeof(short), Schema.Short);
        AddSchema(typeof(ushort), Schema.UShort);
        AddSchema(typeof(int), Schema.Int);
        AddSchema(typeof(uint), Schema.UInt);
        AddSchema(typeof(long), Schema.Long);
        AddSchema(typeof(ulong), Schema.ULong);
        AddSchema(typeof(float), Schema.Float);
        AddSchema(typeof(double), Schema.Double);
        AddSchema(typeof(decimal), Schema.NullableDecimal);
        AddSchema(typeof(Guid), Schema.ByteArray);
        AddSchema(typeof(char), Schema.WholeNumber);
        AddSchema(typeof(DateTime), Schema.Long);
        AddSchema(typeof(DateTimeOffset), Schema.Long);
        AddSchema(typeof(TimeSpan), Schema.Long);
        AddSchema(typeof(DateOnly), Schema.Int);
        AddSchema(typeof(TimeOnly), Schema.Long);

        AddSchema(typeof(bool?), Schema.WholeNumber);
        AddSchema(typeof(byte?), Schema.WholeNumber);
        AddSchema(typeof(sbyte?), Schema.SignedWholeNumber);
        AddSchema(typeof(short?), Schema.SignedWholeNumber);
        AddSchema(typeof(ushort?), Schema.WholeNumber);
        AddSchema(typeof(int?), Schema.SignedWholeNumber);
        AddSchema(typeof(uint?), Schema.WholeNumber);
        AddSchema(typeof(long?), Schema.SignedWholeNumber);
        AddSchema(typeof(ulong?), Schema.WholeNumber);
        AddSchema(typeof(float?), Schema.FractionalNumber);
        AddSchema(typeof(double?), Schema.FractionalNumber);
        AddSchema(typeof(decimal?), Schema.NullableDecimal);
        AddSchema(typeof(Guid?), Schema.ByteArray);
        AddSchema(typeof(char?), Schema.WholeNumber);
        AddSchema(typeof(DateTime?), Schema.SignedWholeNumber);
        AddSchema(typeof(DateTimeOffset?), Schema.SignedWholeNumber);
        AddSchema(typeof(TimeSpan?), Schema.SignedWholeNumber);
        AddSchema(typeof(DateOnly?), Schema.SignedWholeNumber);
        AddSchema(typeof(TimeOnly?), Schema.SignedWholeNumber);
    }

    public void AddSchema(Type type, Schema schema)
    {
        if (!_schemas.TryAdd(type, schema))
        {
            throw new InvalidOperationException($"A schema for type '{type}' already exists.");
        }
    }

    private Schema GetOrAddSchema(Type type, Schema schema) => _schemas.GetOrAdd(type, schema);

    private Schema CreateSchema(Type type)
    {
        if (type.TryGetElementTypeOfArray() is Type elementType)
        {
            var innerSchema = GetOrAddSchema(elementType);

            return Schema.Create(SchemaType.Array, [innerSchema]);
        }

        if (type.TryGetInnerTypesOfDictionary() is (Type keyType, Type valueType))
        {
            var innerSchema = GetOrAddSchema(keyType);
            var innerSchema2 = GetOrAddSchema(valueType);

            return Schema.Create(SchemaType.Dictionary, [innerSchema, innerSchema2]);
        }

        if (type.TryGetTuple2Type() is { } tuple2)
        {
            var innerSchema = GetOrAddSchema(tuple2.Item1Type);
            var innerSchema2 = GetOrAddSchema(tuple2.Item2Type);
            var schemaType = tuple2.IsNullable ? SchemaType.NullableTuple2 : SchemaType.Tuple2;

            return Schema.Create(schemaType, [innerSchema, innerSchema2]);
        }

        if (type.TryGetTuple3Type() is { } tuple3)
        {
            var innerSchema = GetOrAddSchema(tuple3.Item1Type);
            var innerSchema2 = GetOrAddSchema(tuple3.Item2Type);
            var innerSchema3 = GetOrAddSchema(tuple3.Item3Type);
            var schemaType = tuple3.IsNullable ? SchemaType.NullableTuple3 : SchemaType.Tuple3;

            return Schema.Create(schemaType, [innerSchema, innerSchema2, innerSchema3]);
        }

        throw new SchemaException($"No schema for type '{type}' found. Perhaps this type is missing a [BonObject] attribute?");
    }

    public IEnumerable<KeyValuePair<Type, Schema>> Schemas => _schemas;

    public void Clear()
    {
        _schemas.Clear();
    }
}
