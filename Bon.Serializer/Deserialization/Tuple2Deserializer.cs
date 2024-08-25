namespace Bon.Serializer.Deserialization;

internal sealed class Tuple2Deserializer(DeserializerStore deserializerStore, DefaultValueGetterFactory defaultValueGetterFactory) : IUseReflection
{
    public Delegate CreateDeserializer<T>(Tuple2Schema sourceSchema, Tuple2Schema targetSchema)
    {
        if (typeof(T).TryGetInnerTypesOfTuple2() is not (Type item1Type, Type item2Type))
        {
            throw new InvalidOperationException();
        }

        return (Delegate)this.GetPrivateMethod(nameof(CreateTuple2ReaderFor))
            .MakeGenericMethod(item1Type, item2Type)
            .Invoke(this, [sourceSchema, targetSchema])!;
    }

    private Delegate CreateTuple2ReaderFor<T1, T2>(Tuple2Schema sourceSchema, Tuple2Schema targetSchema)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var readItem1 = deserializerStore.GetDeserializer<T1>(sourceSchema.InnerSchema1, targetSchema.InnerSchema1.IsNullable);
        var readItem2 = deserializerStore.GetDeserializer<T2>(sourceSchema.InnerSchema2, targetSchema.InnerSchema2.IsNullable);
        var getDefaultValue = defaultValueGetterFactory.GetDefaultValueGetter<(T1, T2)>(targetSchema.IsNullable);
        var sourceIsNullable = sourceSchema.IsNullable;
        var targetIsNullable = targetSchema.IsNullable;

        if (sourceIsNullable && targetIsNullable)
        {
            return (Read<(T1, T2)?>)((BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NativeSerializer.NULL)
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
            return (Read<(T1, T2)>)((BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NativeSerializer.NULL)
                {
                    return getDefaultValue(input);
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);

                return (item1, item2);
            });
        }

        if (targetIsNullable)
        {
            return (Read<(T1, T2)?>)((BonInput input) =>
            {
                var item1 = readItem1(input);
                var item2 = readItem2(input);

                return (item1, item2);
            });
        }

        return (Read<(T1, T2)>)((BonInput input) =>
        {
            var item1 = readItem1(input);
            var item2 = readItem2(input);

            return (item1, item2);
        });
    }
}
