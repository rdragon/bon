namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of a mapping between types and schemas.
/// </summary>
internal sealed class SchemaByTypeStore
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
    private readonly ConcurrentDictionary<Type, Schema> _schemaByType = [];

    /// <summary>
    /// Returns the schema that will be used when serializing the given type.
    /// If no schema is found, a new schema is created.
    /// </summary>
    public Schema GetSchemaByType(Type type) => _schemaByType.TryGetValue(type, out var schema) ?
        schema : AddSchema(type, CreateSchema(type));

    public void AddNativeSchemas()
    {
        // Bookmark 659516266 (native serialization)
        // All non-generic types for which the source generation context does not provide a schema should be added here.
        // This is the same set of types as can be found at bookmark 293228595.
        AddSchema(typeof(string), NativeSchema.String);
        AddSchema(typeof(bool), NativeSchema.Byte);
        AddSchema(typeof(byte), NativeSchema.Byte);
        AddSchema(typeof(sbyte), NativeSchema.SByte);
        AddSchema(typeof(short), NativeSchema.Short);
        AddSchema(typeof(ushort), NativeSchema.UShort);
        AddSchema(typeof(int), NativeSchema.Int);
        AddSchema(typeof(uint), NativeSchema.UInt);
        AddSchema(typeof(long), NativeSchema.Long);
        AddSchema(typeof(ulong), NativeSchema.ULong);
        AddSchema(typeof(float), NativeSchema.Float);
        AddSchema(typeof(double), NativeSchema.Double);
        AddSchema(typeof(decimal), NativeSchema.NullableDecimal);
        AddSchema(typeof(Guid), ArraySchema.ByteArray);
        AddSchema(typeof(char), NativeSchema.WholeNumber);
        AddSchema(typeof(DateTime), NativeSchema.Long);
        AddSchema(typeof(DateTimeOffset), NativeSchema.Long);
        AddSchema(typeof(TimeSpan), NativeSchema.Long);
        AddSchema(typeof(DateOnly), NativeSchema.Int);
        AddSchema(typeof(TimeOnly), NativeSchema.Long);

        AddSchema(typeof(bool?), NativeSchema.WholeNumber);
        AddSchema(typeof(byte?), NativeSchema.WholeNumber);
        AddSchema(typeof(sbyte?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(short?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(ushort?), NativeSchema.WholeNumber);
        AddSchema(typeof(int?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(uint?), NativeSchema.WholeNumber);
        AddSchema(typeof(long?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(ulong?), NativeSchema.WholeNumber);
        AddSchema(typeof(float?), NativeSchema.FractionalNumber);
        AddSchema(typeof(double?), NativeSchema.FractionalNumber);
        AddSchema(typeof(decimal?), NativeSchema.NullableDecimal);
        AddSchema(typeof(Guid?), ArraySchema.ByteArray);
        AddSchema(typeof(char?), NativeSchema.WholeNumber);
        AddSchema(typeof(DateTime?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(DateTimeOffset?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(TimeSpan?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(DateOnly?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(TimeOnly?), NativeSchema.SignedWholeNumber);
    }

    public Schema AddSchema(Type type, Schema schema) => _schemaByType.GetOrAdd(type, schema);

    public bool TryAdd(Type type, Schema schema)
    {
        var addedSchema = AddSchema(type, schema);

        return ReferenceEquals(addedSchema, schema);
    }

    public Schema CreateSchema(Type type)
    {
        if (type.TryGetElementTypeOfArray() is Type elementType)
        {
            var innerSchema = GetSchemaByType(elementType);

            return Schema.CreateNonCustomSchema(SchemaType.Array, innerSchema);
        }

        if (type.TryGetInnerTypesOfDictionary() is (Type keyType, Type valueType))
        {
            var innerSchema1 = GetSchemaByType(keyType);
            var innerSchema2 = GetSchemaByType(valueType);

            return Schema.CreateNonCustomSchema(SchemaType.Dictionary, innerSchema1, innerSchema2);
        }

        if (type.TryGetTuple2Type() is { } tuple2)
        {
            var innerSchema1 = GetSchemaByType(tuple2.Item1Type);
            var innerSchema2 = GetSchemaByType(tuple2.Item2Type);
            var schemaType = tuple2.IsNullable ? SchemaType.NullableTuple2 : SchemaType.Tuple2;

            return Schema.CreateNonCustomSchema(schemaType, innerSchema1, innerSchema2);
        }

        if (type.TryGetTuple3Type() is { } tuple3)
        {
            var innerSchema1 = GetSchemaByType(tuple3.Item1Type);
            var innerSchema2 = GetSchemaByType(tuple3.Item2Type);
            var innerSchema3 = GetSchemaByType(tuple3.Item3Type);
            var schemaType = tuple3.IsNullable ? SchemaType.NullableTuple3 : SchemaType.Tuple3;

            return Schema.CreateNonCustomSchema(schemaType, innerSchema1, innerSchema2, innerSchema3);
        }

        throw new SchemaException($"No schema for type '{type}' found. Perhaps this type is missing a [BonObject] attribute?");
    }

    public void Clear()
    {
        _schemaByType.Clear();
        _schemaByType.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        hashCode.AddMultipleUnordered(_schemaByType);
        hashCode.AddMultipleUnordered(_schemaByType);
    }
}
