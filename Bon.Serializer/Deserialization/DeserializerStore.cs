namespace Bon.Serializer.Deserialization;
//1at: isnullable
internal sealed class DeserializerStore(
    SchemaByTypeStore schemaByTypeStore) : IUseReflection
{
    private SkipperStore? _skipperStore;
    private RecordDeserializer? _recordDeserializer;
    private UnionDeserializer? _unionDeserializer;

    /// <summary>
    /// Contains all deserializer methods.
    /// The methods are of type <see cref="Read{T}"/>.
    /// These methods read binary data with schema SourceSchema and return a value of type TargetType.
    /// Initially populated by <see cref="AddNativeReaders"/> and the source generation context.
    /// New methods will be added on-the-fly.
    /// </summary>
    private readonly ConcurrentDictionary<(Schema SourceSchema, Type TargetType), Delegate> _deserializers = new();

    private readonly Dictionary<SchemaType, (Delegate DefaultReader, Action<BonInput> Skipper)> _nativeMethods = [];

    /// <summary>
    /// Contains for each enum type the underlying type and a method that adds a cast to the enum type.
    /// This dictionary becomes read-only after the source generation context has run.
    /// </summary>
    private readonly Dictionary<Type, EnumData> _enumDatas = [];

    private SkipperStore SkipperStore => _skipperStore ??= new SkipperStore(this);

    private RecordDeserializer RecordDeserializer =>
        _recordDeserializer ??= new RecordDeserializer(this, SkipperStore);

    private UnionDeserializer UnionDeserializer =>
        _unionDeserializer ??= new UnionDeserializer(this);

    /// <summary>
    /// Contains for each member of each union the type of the member.
    /// For each <see cref="BonIncludeAttribute"/> there is exactly one entry in this dictionary.
    /// This dictionary becomes read-only after the source generation context has run.
    /// </summary>
    public Dictionary<(Type Type, int MemberId), Type> MemberTypes { get; } = [];

    /// <param name="deserializer">
    /// A method that reads binary data with as schema the schema that is used when serializing values
    /// of the target type and that outputs a value of the target type.
    /// </param>
    public void Add(Type targetType, Delegate deserializer)
    {
        var schema = schemaByTypeStore.GetSchemaByType(targetType);
        Add(schema, targetType, deserializer);
    }

    /// <param name="deserializer">
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </param>
    private void Add(Schema sourceSchema, Type targetType, Delegate deserializer)
    {
        _deserializers[(sourceSchema, targetType)] = deserializer;
    }

    public void AddEnumData(Type type, Type underlyingType, Func<Delegate, Delegate> addEnumCast) =>
        _enumDatas[type] = new(underlyingType, addEnumCast);

    public void AddReaderFactory(Type type, Delegate factory) => RecordDeserializer.ReaderFactories[type] = factory;

    public void AddMemberType(Type type, int memberId, Type memberType) => MemberTypes[(type, memberId)] = memberType;

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public Read<T> GetDeserializer<T>(Schema sourceSchema, bool? isNullable = null)
    {
        return (Read<T>)_deserializers.GetOrAdd((sourceSchema, typeof(T)), CreateMethod, this);

        static Read<T> CreateMethod((Schema SourceSchema, Type TargetType) tuple, DeserializerStore store)
        {
            return (Read<T>)store.CreateDeserializer<T>(tuple.SourceSchema, tuple.TargetType);
        }
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public Delegate GetDeserializer(Schema sourceSchema, Type targetType, bool allowNewDeserializer = true)
    {
        var key = (sourceSchema, targetType);

        if (!allowNewDeserializer)
        {
            return _deserializers[key];
        }

        return _deserializers.GetOrAdd(key, CreateMethod, this);

        static Delegate CreateMethod((Schema SourceSchema, Type TargetType) tuple, DeserializerStore store)
        {
            return store.CreateDeserializerSlow(tuple.SourceSchema, tuple.TargetType);
        }
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    private Delegate CreateDeserializerSlow(Schema sourceSchema, Type targetType)
    {
        return (Delegate)this.GetPrivateMethod(nameof(CreateDeserializer))
            .MakeGenericMethod(targetType)
            .Invoke(this, [sourceSchema, targetType])!;
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    private Delegate CreateDeserializer<T>(Schema sourceSchema, Type targetType)
    {
        // There are two reasons to obtain the schema of the target type.
        // The first reason is that it is an easy way to determine the deserialization that is necessary.
        // For example, whether the NativeDeserializer should be used.
        // The second reason is that the schema provides useful information about the target type if the target type
        // is a record or union.
        var targetSchema = schemaByTypeStore.GetSchemaByType(targetType);

        if (sourceSchema is NativeSchema nativeSchema && TryCreateNativeDeserializer(nativeSchema, targetType) is { } readNative)
        {
            return readNative;
        }

        if (sourceSchema is ArraySchema || targetSchema is ArraySchema)
        {
            return new CollectionDeserializer(this).CreateDeserializer<T>(sourceSchema, targetSchema);
        }

        if (sourceSchema is DictionarySchema sourceDictionarySchema && targetSchema is DictionarySchema targetDictionarySchema)
        {
            return new DictionaryDeserializer(this).CreateDeserializer<T>(sourceDictionarySchema, targetDictionarySchema);
        }

        if (sourceSchema is Tuple2Schema sourceTuple2Schema && targetSchema is Tuple2Schema targetTuple2Schema)
        {
            return new Tuple2Deserializer(this).CreateDeserializer<T>(sourceTuple2Schema, targetTuple2Schema);
        }

        if (sourceSchema is Tuple3Schema sourceTuple3Schema && targetSchema is Tuple3Schema targetTuple3Schema)
        {
            return new Tuple3Deserializer(this).CreateDeserializer<T>(sourceTuple3Schema, targetTuple3Schema);
        }

        if (sourceSchema is RecordSchema sourceRecordSchema && targetSchema is RecordSchema targetRecordSchema)
        {
            return RecordDeserializer.CreateDeserializer<T>(sourceRecordSchema, targetRecordSchema);
        }

        if (sourceSchema is UnionSchema unionSourceSchema && targetSchema is UnionSchema unionTargetSchema)
        {
            return UnionDeserializer.CreateDeserializer<T>(unionSourceSchema, unionTargetSchema);
        }

        return GetSkipper<T>(sourceSchema);
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    private Delegate? TryCreateNativeDeserializer(NativeSchema sourceSchema, Type targetType)
    {
        Func<Delegate, Delegate>? addEnumCast = null;

        if (_enumDatas.TryGetValue(targetType, out var enumData))
        {
            addEnumCast = enumData.AddEnumCast;
            targetType = enumData.UnderlyingType;
        }

        var method = NativeDeserializer.TryCreateDeserializer(this, sourceSchema, targetType);

        return addEnumCast is null || method is null ? method : addEnumCast(method);
    }

    public Read<T?> GetSkipper<T>(Schema sourceSchema)
    {
        var skipper = SkipperStore.GetSkipper(sourceSchema);

        return (BonInput input) =>
        {
            skipper(input);

            return default;
        };
    }

    public void AddNativeReaders()
    {
        // These output types should match the types at bookmark 683558879.
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
        AddNativeReader(NativeSchema.DoubleMaybe, static input => NativeSerializer.ReadNullableDouble(input.Reader));
        //1at simpel: pas de methods aan hier, bijv wholenumber en decimal.
    }

    private void AddNativeReader<T>(NativeSchema sourceSchema, Read<T> reader)
    {
        Add(sourceSchema, typeof(T), reader);
        _nativeMethods[sourceSchema.SchemaType] = (reader, SkipValue);

        void SkipValue(BonInput input) => reader(input);
    }

    public int DeserializerCount => _deserializers.Count;

    public Action<BonInput> GetNativeSkipper(SchemaType schemaType) => _nativeMethods[schemaType].Skipper;

    public Delegate GetDefaultNativeReader(SchemaType schemaType) => _nativeMethods[schemaType].DefaultReader;

    /// <summary>
    /// Returns a method of type <c>Read&lt;T?&gt;</c>.
    /// </summary>
    public Delegate LoadDefaultValueGetter(Type targetType)
    {
        return (Delegate)typeof(DeserializerStore).GetPrivateStaticMethod(nameof(GetDefaultValueGetter))
            .MakeGenericMethod(targetType)
            .Invoke(this, [])!;
    }

    private static Read<T?> GetDefaultValueGetter<T>() => (BonInput _) => default;

    /// <param name="UnderlyingType">
    /// The underlying type of the enum.
    /// Nullable if the enum is nullable.
    /// </param>
    /// <param name="AddEnumCast">
    /// A method that converts a Read{UnderlyingType} to a Read{EnumType}.
    /// </param>
    private readonly record struct EnumData(Type UnderlyingType, Func<Delegate, Delegate> AddEnumCast);
}
