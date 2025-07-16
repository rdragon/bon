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
    /// </summary>
    public Schema GetSchemaByType(Type type) => _schemaByType.TryGetValue(type, out var schema) ?
        schema : AddSchema(type, CreateSchema(type));

    public void AddBuiltInSchemas()
    {
        AddNativeSchemas();
        AddWeakSchemas();
    }

    /// <summary>
    /// Adding the native schemas is required for two reasons.
    /// The first reason is that when serializing a native value that is not a field in some other type (e.g. an array of ints) the native
    /// value needs to have a schema to use in the header.
    /// The second reason is that during deserialization the schema of the target type is used to determine the kind of deserialization
    /// that is necessary, e.g. whether <see cref="NativeDeserializer"/> is being used.
    /// </summary>
    private void AddNativeSchemas()
    {
        AddSchema(typeof(string), NativeSchema.String);

        AddSchema(typeof(bool), NativeSchema.Bool);
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
        AddSchema(typeof(decimal), NativeSchema.Decimal);
        AddSchema(typeof(Guid), NativeSchema.Guid);

        AddSchema(typeof(bool?), NativeSchema.WholeNumber);
        AddSchema(typeof(byte?), NativeSchema.WholeNumber);
        AddSchema(typeof(sbyte?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(short?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(ushort?), NativeSchema.WholeNumber);
        AddSchema(typeof(int?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(uint?), NativeSchema.WholeNumber);
        AddSchema(typeof(long?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(ulong?), NativeSchema.WholeNumber);
        AddSchema(typeof(float?), NativeSchema.DoubleMaybe);
        AddSchema(typeof(double?), NativeSchema.DoubleMaybe);
        AddSchema(typeof(decimal?), NativeSchema.Decimal);
        AddSchema(typeof(Guid?), NativeSchema.Guid);
    }

    private void AddWeakSchemas()
    {
        // Bookmark 659516266 (char serialization)
        AddSchema(typeof(char), NativeSchema.WholeNumber);
        AddSchema(typeof(char?), NativeSchema.WholeNumber);
        AddSchema(typeof(DateTime), NativeSchema.Long);
        AddSchema(typeof(DateTime?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(DateTimeOffset), NativeSchema.Long);
        AddSchema(typeof(DateTimeOffset?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(TimeSpan), NativeSchema.Long);
        AddSchema(typeof(TimeSpan?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(DateOnly), NativeSchema.Int);
        AddSchema(typeof(DateOnly?), NativeSchema.SignedWholeNumber);
        AddSchema(typeof(TimeOnly), NativeSchema.Long);
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

        if (type.TryGetInnerTypesOfTuple2() is (Type item1Type, Type item2Type))
        {
            var innerSchema1 = GetSchemaByType(item1Type);
            var innerSchema2 = GetSchemaByType(item2Type);

            return Schema.CreateNonCustomSchema(SchemaType.Tuple2, innerSchema1, innerSchema2);
        }

        if (type.TryGetInnerTypesOfTuple3() is (Type item1, Type item2, Type item3))
        {
            var innerSchema1 = GetSchemaByType(item1);
            var innerSchema2 = GetSchemaByType(item2);
            var innerSchema3 = GetSchemaByType(item3);

            return Schema.CreateNonCustomSchema(SchemaType.Tuple3, innerSchema1, innerSchema2, innerSchema3);
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
