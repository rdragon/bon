namespace Bon.Serializer.Deserialization;

internal sealed partial class DeserializerStore(
    SchemaStore schemaStore) : IUseReflection
{
    private SkipperStore? _skipperStore;
    private RecordDeserializer? _recordDeserializer;
    private UnionDeserializer? _unionDeserializer;
    private CollectionDeserializer? _collectionDeserializer;
    private DictionaryDeserializer? _dictionaryDeserializer;
    private Tuple2Deserializer? _tuple2Deserializer;
    private Tuple3Deserializer? _tuple3Deserializer;
    private NativeDeserializer? _nativeDeserializer;
    private WeakDeserializer? _weakDeserializer;

    /// <summary>
    /// Contains all deserializer methods.
    /// The methods are of type <see cref="Read{T}"/>.
    /// These methods read binary data with schema SourceSchema and return a value of type TargetType.
    /// Initially populated by <see cref="AddNativeReaders"/> and the source generation context.
    /// New methods will be added on-the-fly.
    /// </summary>
    private readonly ConcurrentDictionary<(Schema SourceSchema, Type TargetType), Delegate> _deserializers = new();

    /// <summary>
    /// //2at
    /// </summary>
    private readonly Dictionary<SchemaType, (Delegate DefaultReader, Action<BonInput> Skipper)> _nativeMethods = [];

    /// <summary>
    /// Contains for each member of each record and union the type of the member.
    /// For each <see cref="BonIncludeAttribute"/> and <see cref="BonMemberAttribute"/> there is one entry in this dictionary.
    /// This dictionary becomes read-only after the source generation context has run.
    /// This dictionary gives easy access to the types of these members, which is helpful during deserialization.
    /// </summary>
    public Dictionary<(Type Type, int MemberId), Type> MemberTypes { get; } = [];

    private SkipperStore SkipperStore => _skipperStore ??= new SkipperStore(this);
    private RecordDeserializer RecordDeserializer => _recordDeserializer ??= new RecordDeserializer(this, SkipperStore);
    private CollectionDeserializer CollectionDeserializer => _collectionDeserializer ??= new CollectionDeserializer(this);
    private DictionaryDeserializer DictionaryDeserializer => _dictionaryDeserializer ??= new DictionaryDeserializer(this);
    private UnionDeserializer UnionDeserializer => _unionDeserializer ??= new UnionDeserializer(this);
    private Tuple2Deserializer Tuple2Deserializer => _tuple2Deserializer ??= new Tuple2Deserializer(this);
    private Tuple3Deserializer Tuple3Deserializer => _tuple3Deserializer ??= new Tuple3Deserializer(this);
    private NativeDeserializer NativeDeserializer => _nativeDeserializer ??= new NativeDeserializer(this);
    private WeakDeserializer WeakDeserializer => _weakDeserializer ??= new WeakDeserializer(this);

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public Read<T> GetDeserializer<T>(Schema sourceSchema)
    {
        return (Read<T>)_deserializers.GetOrAdd((sourceSchema, typeof(T)), CreateMethod, this);

        static Read<T> CreateMethod((Schema SourceSchema, Type TargetType) tuple, DeserializerStore store)
        {
            return (Read<T>)store.CreateDeserializer<T>(tuple.SourceSchema);
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
            .Invoke(this, [sourceSchema])!;
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    private Delegate CreateDeserializer<T>(Schema sourceSchema)
    {
        var targetType = typeof(T);

        // The target schema is required for the record and union deserialization.
        // This call also makes sure that the target type is deserializable.
        var targetSchema = schemaStore.GetOrAddSchema(targetType);

        return
            WeakDeserializer.TryCreateDeserializer(sourceSchema, targetType) ??
            NativeDeserializer.TryCreateDeserializer(sourceSchema, targetType) ??
            CollectionDeserializer.TryCreateDeserializer<T>(sourceSchema) ??
            DictionaryDeserializer.TryCreateDeserializer(sourceSchema, targetType) ??
            Tuple2Deserializer.TryCreateDeserializer(sourceSchema, targetType) ??
            Tuple3Deserializer.TryCreateDeserializer(sourceSchema, targetType) ??
            RecordDeserializer.TryCreateDeserializer<T>(sourceSchema, targetSchema) ??
            UnionDeserializer.TryCreateDeserializer<T>(sourceSchema, targetSchema) ??
            GetSkipper<T>(sourceSchema);
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the schema and returns the default value for the target type.
    /// </summary>
    public Read<T?> GetSkipper<T>(Schema schema)
    {
        var skipper = SkipperStore.GetSkipper(schema);

        return input =>
        {
            skipper(input);

            return default;
        };
    }

    /// <summary>
    /// //2at
    /// </summary>
    public Action<BonInput> GetNativeSkipper(SchemaType schemaType) => _nativeMethods[schemaType].Skipper;

    /// <summary>
    /// //2at
    /// </summary>
    public Delegate GetDefaultNativeReader(SchemaType schemaType) => _nativeMethods[schemaType].DefaultReader;

    public int DeserializerCount => _deserializers.Count;

    /// <summary>
    /// Returns a method of type <c>Read&lt;T?&gt;</c> that generates the default value corresponding to <c>T</c>.
    /// </summary>
    public Delegate LoadDefaultValueGetter(Type type)
    {
        return (Delegate)typeof(DeserializerStore).GetPrivateStaticMethod(nameof(GetDefaultValueGetter))
            .MakeGenericMethod(type)
            .Invoke(this, [])!;
    }

    public void AddWeakDeserializerFactory<T1, T2>(Func<T2, T1> func) => WeakDeserializer.AddFactory(func);

    private static Read<T?> GetDefaultValueGetter<T>() => (BonInput _) => default;
}
