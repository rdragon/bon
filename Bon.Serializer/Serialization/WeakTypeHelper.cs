namespace Bon.Serializer.Serialization;

internal static class WeakTypeHelper
{
    public static Delegate? TryCreateDeserializer(DeserializerStore deserializerStore, Schema sourceSchema, Type targetType)
    {
        if (TryGetHelper(targetType) is { } weakHelper)
        {
            return weakHelper.CreateReader(deserializerStore, sourceSchema);
        }

        return null;
    }

    public static WeakHelper? TryGetHelper(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
        {
            return typeof(WeakTypeHelper).CallPrivateStaticMethod<WeakHelper>(
                nameof(CreateForKeyValuePair),
                type.GetGenericArguments());
        }

        if (type.IsNullableValueType())
        {
            var unwrapped = type.UnwrapNullable(out _);

            if (unwrapped.IsGenericType && unwrapped.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return typeof(WeakTypeHelper).CallPrivateStaticMethod<WeakHelper>(
                    nameof(CreateForNullableKeyValuePair),
                    unwrapped.GetGenericArguments());
            }
        }

        return null;
    }

    private static WeakHelper CreateForKeyValuePair<TKey, TValue>()
    {
        return CreateWeakHelper<KeyValuePair<TKey, TValue>, (TKey, TValue)>(
            pair => (pair.Key, pair.Value),
            tuple => new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2));
    }

    private static WeakHelper CreateForNullableKeyValuePair<TKey, TValue>()
    {
        return CreateWeakHelper<KeyValuePair<TKey, TValue>?, (TKey, TValue)?>(
            maybe => maybe is { } pair ? (pair.Key, pair.Value) : null,
            maybe => maybe is { } tuple ? new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2) : null);
    }

    private static WeakHelper CreateWeakHelper<TWeak, TWire>(Func<TWeak, TWire> toWire, Func<TWire, TWeak> toWeak)
    {
        Writer transformWriter(Writer writer)
        {
            var writeWire = writer.Convert<TWire>().Write;
            var writeWeak = (BinaryWriter binaryWriter, TWeak value) => writeWire(binaryWriter, toWire(value));
            return new Writer(writeWeak, writer.SimpleWriterType);
        }

        Delegate createReader(DeserializerStore deserializerStore, Schema schema)
        {
            var readWire = deserializerStore.GetDeserializer<TWire>(schema);
            var readWeak = (Read<TWeak>)(input => toWeak(readWire(input)));
            return readWeak;
        }

        return new WeakHelper(typeof(TWire), transformWriter, createReader);
    }
}

internal sealed record class WeakHelper(
    Type WireType,
    Func<Writer, Writer> TransformWriter,
    Func<DeserializerStore, Schema, Delegate> CreateReader);
