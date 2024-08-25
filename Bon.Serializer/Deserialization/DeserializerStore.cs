namespace Bon.Serializer.Deserialization;

internal sealed class DeserializerStore(
    SchemaByTypeStore schemaByTypeStore,
    DefaultValueGetterFactory defaultValueGetterFactory) : IUseReflection
{
    private SkipperStore? _skipperStore;
    private RecordDeserializer? _recordDeserializer;
    private UnionDeserializer? _unionDeserializer;

    /// <summary>
    /// The values are of type <see cref="Read{T}"/> with <c>T</c> corresponding to the target type.
    /// Reference target types are assumed to be non-nullable.
    /// Populated by <see cref="ISourceGenerationContext"/> with a deserializer for
    /// each <see cref="BonObject"/> type.
    /// New deserializers will be added on-the-fly.
    /// </summary>
    private readonly ConcurrentDictionary<(Schema SourceSchema, Type TargetType), Delegate> _deserializers = new();

    /// <summary>
    /// The values are of type <see cref="Read{T}"/> with <c>T</c> corresponding to the target type.
    /// Populated by <see cref="ISourceGenerationContext"/> with a deserializer for
    /// each <see cref="BonObject"/> type.
    /// New deserializers will be added on-the-fly.
    /// </summary>
    private readonly ConcurrentDictionary<(Schema SourceSchema, AnnotatedType TargetType), Delegate> _deserializersAnnotated = new();

    /// <summary>
    /// Contains for each enum type the underlying type and a method that adds a cast to the enum type.
    /// This dictionary becomes read-only after the source generation context has run.
    /// </summary>
    private readonly Dictionary<Type, EnumData> _enumDatas = [];

    private SkipperStore SkipperStore => _skipperStore ??= new SkipperStore(this);

    private RecordDeserializer RecordDeserializer =>
        _recordDeserializer ??= new RecordDeserializer(this, SkipperStore, defaultValueGetterFactory);

    private UnionDeserializer UnionDeserializer =>
        _unionDeserializer ??= new UnionDeserializer(this, defaultValueGetterFactory);

    /// <summary>
    /// Contains for each member of each union the type of the member.
    /// For each <see cref="BonIncludeAttribute"/> there is exactly one entry in this dictionary.
    /// This dictionary becomes read-only after the source generation context has run.
    /// </summary>
    public Dictionary<(Type Type, int MemberId), Type> MemberTypes { get; } = [];

    public void Add(Type type, bool isNullable, Delegate deserializer)
    {
        var annotatedType = new AnnotatedType(type, isNullable);
        var schema = schemaByTypeStore.GetSchemaByType(annotatedType);
        Add(schema, annotatedType, deserializer);
    }

    private void Add(Schema sourceSchema, AnnotatedType targetType, Delegate deserializer)
    {
        _deserializersAnnotated[(sourceSchema, targetType)] = deserializer;
        _deserializers[(sourceSchema, targetType.Type)] = deserializer;
    }

    private void AddNativeReader<T>(NativeSchema sourceSchema, Read<T> deserializer)
    {
        Add(sourceSchema, new AnnotatedType(typeof(T), sourceSchema.IsNullable), deserializer);
    }

    public void AddEnumData(Type type, Type underlyingType, Func<Delegate, Delegate> addEnumCast) =>
        _enumDatas[type] = new(underlyingType, addEnumCast);

    public void AddReaderFactory(Type type, Delegate factory) => RecordDeserializer.ReaderFactories[type] = factory;

    public void AddMemberType(Type type, int memberId, Type memberType) => MemberTypes[(type, memberId)] = memberType;

    /// <summary>
    /// Returns a deserializer that reads a value that has been serialized with the schema <paramref name="sourceSchema"/>.
    /// The value is then converted to the target type if it is not already of this type.
    /// </summary>
    /// <param name="sourceSchema"></param>
    /// <param name="isNullable">
    /// Whether the target type is nullable.
    /// If you do not provide this value then the target type is inspected to determine if it is nullable. Here a reference
    /// type is always assumed to be non-nullable.
    /// </param>
    public Delegate GetDeserializer(Schema sourceSchema, Type targetType, bool? isNullable)
    {
        return (Delegate)this.GetPrivateMethod(nameof(GetDeserializerNow))
            .MakeGenericMethod(targetType)
            .Invoke(this, [sourceSchema, isNullable])!;
    }

    /// <summary>
    /// Returns a deserializer that reads a value that has been serialized with the schema <paramref name="sourceSchema"/>.
    /// The value is then converted to type <typeparamref name="T"/> if it is not already of this type.
    /// </summary>
    /// <param name="sourceSchema"></param>
    /// <param name="isNullable">
    /// Whether <typeparamref name="T"/> is nullable.
    /// If you do not provide this value then <typeparamref name="T"/> is inspected to determine if it is nullable. Here a reference
    /// type is always assumed to be non-nullable.
    /// </param>
    public Read<T> GetDeserializer<T>(Schema sourceSchema, bool? isNullable) => GetDeserializerNow<T>(sourceSchema, isNullable);

    private Read<T> GetDeserializerNow<T>(Schema sourceSchema, bool? isNullable)
    {
        if (isNullable.HasValue)
        {
            var targetType = new AnnotatedType(typeof(T), isNullable.Value);

            return (Read<T>)_deserializersAnnotated.GetOrAdd((sourceSchema, targetType), Create2, this);
        }

        return (Read<T>)_deserializers.GetOrAdd((sourceSchema, typeof(T)), Create1, this);

        static Read<T> Create1((Schema SourceSchema, Type TargetType) tuple, DeserializerStore store)
        {
            return store.GetDeserializer<T>(tuple.SourceSchema, tuple.TargetType.IsNullable(false));
        }

        static Read<T> Create2((Schema SourceSchema, AnnotatedType TargetType) tuple, DeserializerStore store)
        {
            return (Read<T>)store.CreateDeserializer<T>(tuple.SourceSchema, tuple.TargetType);
        }
    }

    private Delegate CreateDeserializer<T>(Schema sourceSchema, AnnotatedType targetType)
    {
        // There are two reasosn to obtain the schema of the target type.
        // The first reason is that it is an easy way to determine the deserialization that is necessary.
        // For example, whether the NativeDeserializer should be used.
        // The second reason is that the schema provides useful information about the target type if the target type
        // is a record or union.
        var targetSchema = schemaByTypeStore.GetSchemaByType(targetType);

        if (sourceSchema is NativeSchema nativeSourceSchema && targetSchema is NativeSchema)
        {
            return CreateNativeDeserializer(nativeSourceSchema, targetType);
        }

        if (sourceSchema is ArraySchema || targetSchema is ArraySchema)
        {
            return new CollectionDeserializer(this, defaultValueGetterFactory).CreateDeserializer<T>(sourceSchema, targetSchema);
        }

        if (sourceSchema is DictionarySchema sourceDictionarySchema && targetSchema is DictionarySchema targetDictionarySchema)
        {
            return new DictionaryDeserializer(this).CreateDeserializer<T>(sourceDictionarySchema, targetDictionarySchema);
        }

        if (sourceSchema is Tuple2Schema sourceTuple2Schema && targetSchema is Tuple2Schema targetTuple2Schema)
        {
            return new Tuple2Deserializer(this, defaultValueGetterFactory).CreateDeserializer<T>(sourceTuple2Schema, targetTuple2Schema);
        }

        if (sourceSchema is Tuple3Schema sourceTuple3Schema && targetSchema is Tuple3Schema targetTuple3Schema)
        {
            return new Tuple3Deserializer(this, defaultValueGetterFactory).CreateDeserializer<T>(sourceTuple3Schema, targetTuple3Schema);
        }

        if (sourceSchema is RecordSchema sourceRecordSchema && targetSchema is RecordSchema targetRecordSchema)
        {
            return RecordDeserializer.CreateDeserializer<T>(sourceRecordSchema, targetRecordSchema);
        }

        if (sourceSchema is UnionSchema unionSourceSchema && targetSchema is UnionSchema unionTargetSchema)
        {
            return UnionDeserializer.CreateDeserializer<T>(unionSourceSchema, unionTargetSchema);
        }

        return GetSkipper<T>(sourceSchema, targetSchema.IsNullable);
    }

    /// <summary>
    /// Returns a deserializer that reads a value with schema <paramref name="sourceSchema"/> and returns a value of
    /// type <paramref name="targetType"/>.
    /// </summary>
    private Delegate CreateNativeDeserializer(NativeSchema sourceSchema, AnnotatedType targetType)
    {
        AnnotatedType? underlyingType = null;
        Func<Delegate, Delegate>? addEnumCast = null;

        if (_enumDatas.TryGetValue(targetType.Type, out var enumData))
        {
            addEnumCast = enumData.AddEnumCast;
            underlyingType = new AnnotatedType(enumData.UnderlyingType, targetType.IsNullable);
        }

        var sourceType = sourceSchema.AnnotatedSchemaType.ToNativeType();

        var deserialize = NativeDeserializer.CreateNativeDeserializer(
            _deserializers[(sourceSchema, sourceType)],
            sourceType,
            underlyingType ?? targetType);

        return addEnumCast is null ? deserialize : addEnumCast(deserialize);
    }

    public Read<T> GetSkipper<T>(Schema sourceSchema, bool targetIsNullable)
    {
        var skipper = SkipperStore.GetSkipper(sourceSchema);
        var getDefaultValue = defaultValueGetterFactory.GetDefaultValueGetter<T>(targetIsNullable);

        return (BonInput input) =>
        {
            skipper(input);

            return getDefaultValue(input);
        };
    }

    public void AddNativeReaders()
    {
        AddNativeReader(NativeSchema.String, static input => NativeSerializer.ReadString(input.Reader));
        AddNativeReader(NativeSchema.Bool, static input => NativeSerializer.ReadBool(input.Reader));
        AddNativeReader(NativeSchema.Byte, static input => NativeSerializer.ReadByte(input.Reader));
        AddNativeReader(NativeSchema.SByte, static input => NativeSerializer.ReadSByte(input.Reader));
        AddNativeReader(NativeSchema.Short, static input => NativeSerializer.ReadShort(input.Reader));
        AddNativeReader(NativeSchema.UShort, static input => NativeSerializer.ReadUShort(input.Reader));
        AddNativeReader(NativeSchema.Int, static input => NativeSerializer.ReadInt(input.Reader));
        AddNativeReader(NativeSchema.UInt, static input => NativeSerializer.ReadUInt(input.Reader));
        AddNativeReader(NativeSchema.Long, static input => NativeSerializer.ReadLong(input.Reader));
        AddNativeReader(NativeSchema.ULong, static input => NativeSerializer.ReadULong(input.Reader));
        AddNativeReader(NativeSchema.WholeNumber, static input => NativeSerializer.ReadWholeNumber(input.Reader));
        AddNativeReader(NativeSchema.SignedWholeNumber, static input => NativeSerializer.ReadSignedWholeNumber(input.Reader));
        AddNativeReader(NativeSchema.Float, static input => NativeSerializer.ReadFloat(input.Reader));
        AddNativeReader(NativeSchema.Double, static input => NativeSerializer.ReadDouble(input.Reader));
        AddNativeReader(NativeSchema.Decimal, static input => NativeSerializer.ReadDecimal(input.Reader));
        AddNativeReader(NativeSchema.Guid, static input => NativeSerializer.ReadGuid(input.Reader));

        AddNativeReader(NativeSchema.NullableString, static input => NativeSerializer.ReadNullableString(input.Reader));
        AddNativeReader(NativeSchema.NullableBool, static input => NativeSerializer.ReadNullableBool(input.Reader));
        AddNativeReader(NativeSchema.NullableWholeNumber, static input => NativeSerializer.ReadNullableWholeNumber(input.Reader));
        AddNativeReader(NativeSchema.NullableSignedWholeNumber, static input => NativeSerializer.ReadNullableSignedWholeNumber(input.Reader));
        AddNativeReader(NativeSchema.NullableFloat, static input => NativeSerializer.ReadNullableFloat(input.Reader));
        AddNativeReader(NativeSchema.NullableDouble, static input => NativeSerializer.ReadNullableDouble(input.Reader));
        AddNativeReader(NativeSchema.NullableDecimal, static input => NativeSerializer.ReadNullableDecimal(input.Reader));
        AddNativeReader(NativeSchema.NullableGuid, static input => NativeSerializer.ReadNullableGuid(input.Reader));
    }

    public int DeserializerCount => _deserializers.Count + _deserializersAnnotated.Count;

    public object LoadDefaultValue(Type type) => defaultValueGetterFactory.LoadDefaultValue(type, type.IsNullable(false));

    /// <param name="UnderlyingType">
    /// The underlying type of the enum.
    /// Nullable if the enum is nullable.
    /// </param>
    /// <param name="AddEnumCast">
    /// A method that converts a Read{UnderlyingType} to a Read{EnumType}.
    /// </param>
    private readonly record struct EnumData(Type UnderlyingType, Func<Delegate, Delegate> AddEnumCast);
}
