namespace Bon.Serializer.Deserialization;

internal sealed class Tuple2Deserializer(DeserializerStore deserializerStore) : IUseReflection
{
    public Delegate? TryCreateDeserializer(Schema1 sourceSchema, Type targetType)
    {
        if (sourceSchema is not Tuple2Schema tuple2Schema ||
            targetType.TryGetTuple2Type() is not { } tuple2Type)
        {
            return null;
        }

        return (Delegate?)this.GetPrivateMethod(nameof(CreateTuple2ReaderFor))
            .MakeGenericMethod(tuple2Type.Item1Type, tuple2Type.Item2Type)
            .Invoke(this, [tuple2Schema, tuple2Type.IsNullable])!;
    }

    private Delegate CreateTuple2ReaderFor<T1, T2>(Tuple2Schema sourceSchema, bool targetIsNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var readItem1 = deserializerStore.GetDeserializer<T1>(sourceSchema.InnerSchema1);
        var readItem2 = deserializerStore.GetDeserializer<T2>(sourceSchema.InnerSchema2);
        var sourceIsNullable = sourceSchema.IsNullable;

        if (sourceIsNullable && targetIsNullable)
        {
            return (Read<(T1, T2)?>)(input =>
            {
                if (input.Reader.ReadByte() == NativeWriter.NULL)
                {
                    return null;
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);

                return (item1, item2);
            });
        }

        if (sourceIsNullable)
        {
            return (Read<(T1, T2)>)(input =>
            {
                if (input.Reader.ReadByte() == NativeWriter.NULL)
                {
                    return default;
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);

                return (item1, item2);
            });
        }

        if (targetIsNullable)
        {
            return (Read<(T1, T2)?>)(input =>
            {
                var item1 = readItem1(input);
                var item2 = readItem2(input);

                return (item1, item2);
            });
        }

        return (Read<(T1, T2)>)(input =>
        {
            var item1 = readItem1(input);
            var item2 = readItem2(input);

            return (item1, item2);
        });
    }
}
