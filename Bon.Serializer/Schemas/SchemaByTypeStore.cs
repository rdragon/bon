namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of a mapping between types and schemas.
/// </summary>
internal sealed class SchemaByTypeStore
{
    /// <summary>
    /// During initialization the native types and the types from the source generation context are added to this dictionary.
    /// The latter are all the classes, structs, interfaces and enums with the <see cref="BonObjectAttribute"/> (their nullable
    /// and non-nullable versions).
    /// 
    /// Generic types like arrays and tuples are added during serialization and deserialization.
    /// 
    /// When loading schemas from the storage this dictionary is not updated because there is no type information available in the storage.
    /// This is not a problem as the source generation context already populates this dictionary with all types with the
    /// <see cref="BonObjectAttribute"/>.
    /// </summary>
    private readonly ConcurrentDictionary<AnnotatedType, Schema> _schemaByAnnotatedType = [];

    /// <summary>
    /// Contains the same schemas as <see cref="_schemaByAnnotatedType"/> but without the nullable reference type schemas.
    /// Reference types are assumed to be non-nullable in this dictionary.
    /// The reason this dictionary exists is to avoid having to check if a type is nullable every time a schema is requested.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Schema> _schemaByType = [];

    /// <summary>
    /// Returns a schema corresponding to the given type.
    /// If a reference type is given then a non-nullable schema is returned.
    /// For generic types this method may create a new schema.
    /// </summary>
    public Schema GetSchemaByType(Type type) => _schemaByType.TryGetValue(type, out var schema) ?
        schema : GetSchemaByType(new AnnotatedType(type, type.IsNullable(false)));

    /// <summary>
    /// Returns a schema corresponding to the given type.
    /// For generic types this method may create a new schema.
    /// </summary>
    public Schema GetSchemaByType(AnnotatedType annotatedType) =>
        _schemaByAnnotatedType.TryGetValue(annotatedType, out var schema) ?
            schema : AddSchema(annotatedType.Type, GetGenericSchema(annotatedType));

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

        AddSchema(typeof(string), NativeSchema.NullableString);
        AddSchema(typeof(bool?), NativeSchema.NullableBool);
        AddSchema(typeof(byte?), NativeSchema.NullableWholeNumber);
        AddSchema(typeof(sbyte?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(short?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(ushort?), NativeSchema.NullableWholeNumber);
        AddSchema(typeof(int?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(uint?), NativeSchema.NullableWholeNumber);
        AddSchema(typeof(long?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(ulong?), NativeSchema.NullableWholeNumber);
        AddSchema(typeof(float?), NativeSchema.NullableFloat);
        AddSchema(typeof(double?), NativeSchema.NullableDouble);
        AddSchema(typeof(decimal?), NativeSchema.NullableDecimal);
        AddSchema(typeof(Guid?), NativeSchema.NullableGuid);
    }

    private void AddWeakSchemas()
    {
        // Bookmark 659516266 (char serialization)
        AddSchema(typeof(char), NativeSchema.WholeNumber);
        AddSchema(typeof(char?), NativeSchema.NullableWholeNumber);
        AddSchema(typeof(DateTime), NativeSchema.Long);
        AddSchema(typeof(DateTime?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(DateTimeOffset), NativeSchema.Long);
        AddSchema(typeof(DateTimeOffset?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(TimeSpan), NativeSchema.Long);
        AddSchema(typeof(TimeSpan?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(DateOnly), NativeSchema.Int);
        AddSchema(typeof(DateOnly?), NativeSchema.NullableSignedWholeNumber);
        AddSchema(typeof(TimeOnly), NativeSchema.Long);
        AddSchema(typeof(TimeOnly?), NativeSchema.NullableSignedWholeNumber);
    }

    public Schema AddSchema(Type type, Schema schema)
    {
        var annotatedType = new AnnotatedType(type, schema.IsNullable);

        if (!annotatedType.IsNullableReferenceType)
        {
            _schemaByType.TryAdd(annotatedType.Type, schema);
        }

        return _schemaByAnnotatedType.GetOrAdd(annotatedType, schema);
    }

    public bool TryAdd(Type type, Schema schema)
    {
        var addedSchema = AddSchema(type, schema);

        return ReferenceEquals(addedSchema, schema);
    }

    public Schema GetGenericSchema(AnnotatedType annotatedType)
    {
        var (type, isNullable) = annotatedType;

        if (type.TryGetElementTypeOfArray() is Type elementType)
        {
            var innerSchema = GetSchemaByType(elementType);

            return Schema.CreateNonCustomSchema(SchemaType.Array, isNullable, innerSchema);
        }

        if (type.TryGetInnerTypesOfDictionary() is (Type keyType, Type valueType))
        {
            var innerSchema1 = GetSchemaByType(keyType);
            var innerSchema2 = GetSchemaByType(valueType);

            return Schema.CreateNonCustomSchema(SchemaType.Dictionary, isNullable, innerSchema1, innerSchema2);
        }

        if (type.TryGetInnerTypesOfTuple2() is (Type item1Type, Type item2Type))
        {
            var innerSchema1 = GetSchemaByType(item1Type);
            var innerSchema2 = GetSchemaByType(item2Type);

            return Schema.CreateNonCustomSchema(SchemaType.Tuple2, isNullable, innerSchema1, innerSchema2);
        }

        if (type.TryGetInnerTypesOfTuple3() is (Type item1, Type item2, Type item3))
        {
            var innerSchema1 = GetSchemaByType(item1);
            var innerSchema2 = GetSchemaByType(item2);
            var innerSchema3 = GetSchemaByType(item3);

            return Schema.CreateNonCustomSchema(SchemaType.Tuple3, isNullable, innerSchema1, innerSchema2, innerSchema3);
        }

        throw new SchemaException($"No schema for type '{type}' found. Perhaps this type is missing a [BonObject] attribute?");
    }

    public void Clear()
    {
        _schemaByAnnotatedType.Clear();
        _schemaByType.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        hashCode.AddMultipleUnordered(_schemaByAnnotatedType);
        hashCode.AddMultipleUnordered(_schemaByType);
    }
}
