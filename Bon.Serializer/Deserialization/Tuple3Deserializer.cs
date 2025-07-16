namespace Bon.Serializer.Deserialization;

internal sealed class Tuple3Deserializer(DeserializerStore deserializerStore) : IUseReflection
{
    public Delegate? TryCreateDeserializer(Schema sourceSchema, Type targetType)
    {
        if (sourceSchema is not Tuple3Schema tuple3Schema ||
            targetType.TryGetTuple3Type() is not { } tuple3Type)
        {
            return null;
        }

        return (Delegate?)this.GetPrivateMethod(nameof(CreateTuple3ReaderFor))
            .MakeGenericMethod(tuple3Type.Item1Type, tuple3Type.Item2Type, tuple3Type.Item3Type)
            .Invoke(this, [tuple3Schema, tuple3Type.IsNullable])!;
    }

    private Delegate CreateTuple3ReaderFor<T1, T2, T3>(Tuple3Schema sourceSchema, bool targetIsNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var readItem1 = deserializerStore.GetDeserializer<T1>(sourceSchema.InnerSchema1);
        var readItem2 = deserializerStore.GetDeserializer<T2>(sourceSchema.InnerSchema2);
        var readItem3 = deserializerStore.GetDeserializer<T3>(sourceSchema.InnerSchema3);
        var sourceIsNullable = sourceSchema.IsNullable;

        if (sourceIsNullable && targetIsNullable)
        {
            return (Read<(T1, T2, T3)?>)(input =>
            {
                if (input.Reader.ReadByte() == NativeWriter.NULL)
                {
                    return null;
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);
                var item3 = readItem3(input);

                return (item1, item2, item3);
            });
        }

        if (sourceIsNullable)
        {
            return (Read<(T1, T2, T3)>)(input =>
            {
                if (input.Reader.ReadByte() == NativeWriter.NULL)
                {
                    return default;
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);
                var item3 = readItem3(input);

                return (item1, item2, item3);
            });
        }

        if (targetIsNullable)
        {
            return (Read<(T1, T2, T3)?>)(input =>
            {
                var item1 = readItem1(input);
                var item2 = readItem2(input);
                var item3 = readItem3(input);

                return (item1, item2, item3);
            });
        }

        return (Read<(T1, T2, T3)>)(input =>
        {
            var item1 = readItem1(input);
            var item2 = readItem2(input);
            var item3 = readItem3(input);

            return (item1, item2, item3);
        });
    }
}
