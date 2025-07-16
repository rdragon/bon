namespace Bon.Serializer.Deserialization;

/// <summary>
/// //2at
/// </summary>
internal class WeakDeserializer(DeserializerStore deserializerStore)
{
    private Dictionary<Type, Func<Schema, Delegate>>? _factories;

    private Dictionary<Type, Func<Schema, Delegate>> Factories => _factories ??= CreateFactories();

    private Dictionary<Type, Func<Schema, Delegate>> CreateFactories()
    {
        var factories = new Dictionary<Type, Func<Schema, Delegate>>();

        // Bookmark 659516266 (native serialization)
        // All native types that cannot be found in the enum 212882721 should be added here. They should be mapped to types that
        // can be found in the enum.
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

        void AddFactory<T1, T2>(Func<T2, T1> func)
        {
            factories[typeof(T1)] = schema =>
            {
                var read = deserializerStore.GetDeserializer<T2>(schema);
                return (Read<T1>)(input => func(read(input)));
            };
        }

        return factories;
    }

    public Delegate? TryCreateDeserializer(Schema sourceSchema, Type targetType)
    {
        return Factories.TryGetValue(targetType, out var factory) ? factory(sourceSchema) : null;
    }
}
