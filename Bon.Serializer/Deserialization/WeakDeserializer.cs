namespace Bon.Serializer.Deserialization;

/// <summary>
/// //2at
/// </summary>
internal class WeakDeserializer
{
    private readonly DeserializerStore _deserializerStore;

    /// <summary>
    /// //2at
    /// </summary>
    private readonly Dictionary<Type, Func<Schema, Delegate>> _factories = [];

    public WeakDeserializer(DeserializerStore deserializerStore)
    {
        _deserializerStore = deserializerStore;
        AddNativeFactories();
    }

    private void AddNativeFactories()
    {
        // Bookmark 659516266 (native serialization)
        // All native types that cannot be found in the NativeType enum (bookmark 212882721) should be added here.
        // They should be mapped to types that can be found in the enum.
        // This mapping should be in sync with the mapping at bookmark 988874999.
        AddFactory<bool, byte>(x => x != 0);
        AddFactory<decimal, decimal?>(x => x ?? 0);
        AddFactory<char, ulong?>(x => (char)(x ?? 0));
        AddFactory<DateTime, long>(x => x.ToDateTime());
        AddFactory<DateTimeOffset, long>(x => x.ToDateTimeOffset());
        AddFactory<TimeSpan, long>(x => x.ToTimeSpan());
        AddFactory<DateOnly, int>(x => x.ToDateOnly());
        AddFactory<TimeOnly, long>(x => x.ToTimeOnly());
        AddFactory<Guid, byte[]?>(x => x?.ToGuid() ?? default);

        AddFactory<bool?, ulong?>(x => x is { } y ? (y != 0) : null);
        AddFactory<byte?, ulong?>(x => (byte?)x);
        AddFactory<sbyte?, long?>(x => (sbyte?)x);
        AddFactory<short?, long?>(x => (short?)x);
        AddFactory<ushort?, ulong?>(x => (ushort?)x);
        AddFactory<int?, long?>(x => (int?)x);
        AddFactory<uint?, ulong?>(x => (uint?)x);
        AddFactory<float?, double?>(x => (float?)x);
        AddFactory<char?, ulong?>(x => (char?)x);
        AddFactory<DateTime?, long?>(x => x?.ToDateTime());
        AddFactory<DateTimeOffset?, long?>(x => x?.ToDateTimeOffset());
        AddFactory<TimeSpan?, long?>(x => x?.ToTimeSpan());
        AddFactory<DateOnly?, int?>(x => x?.ToDateOnly());
        AddFactory<TimeOnly?, long?>(x => x?.ToTimeOnly());
        AddFactory<Guid?, byte[]?>(x => x?.ToGuid());
    }

    public void AddFactory<T1, T2>(Func<T2, T1> func)
    {
        _factories[typeof(T1)] = schema =>
        {
            var read = _deserializerStore.GetDeserializer<T2>(schema);
            return (Read<T1>)(input => func(read(input)));
        };
    }

    public Delegate? TryCreateDeserializer(Schema sourceSchema, Type targetType) =>
        _factories.TryGetValue(targetType, out var factory) ? factory(sourceSchema) : null;
}
