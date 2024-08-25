namespace Bon.Serializer.Deserialization;

internal sealed class Tuple3Deserializer(DeserializerStore deserializerStore, DefaultValueGetterFactory defaultValueGetterFactory) : IUseReflection
{
    public Delegate CreateDeserializer<T>(Tuple3Schema sourceSchema, Tuple3Schema targetSchema)
    {
        if (typeof(T).TryGetInnerTypesOfTuple3() is not (Type item1Type, Type item2Type, Type item3Type))
        {
            throw new InvalidOperationException();
        }

        return (Delegate)this.GetPrivateMethod(nameof(CreateTuple3ReaderFor))
            .MakeGenericMethod(item1Type, item2Type, item3Type)
            .Invoke(this, [sourceSchema, targetSchema])!;
    }

    private Delegate CreateTuple3ReaderFor<T1, T2, T3>(Tuple3Schema sourceSchema, Tuple3Schema targetSchema)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var readItem1 = deserializerStore.GetDeserializer<T1>(sourceSchema.InnerSchema1, targetSchema.InnerSchema1.IsNullable);
        var readItem2 = deserializerStore.GetDeserializer<T2>(sourceSchema.InnerSchema2, targetSchema.InnerSchema2.IsNullable);
        var readItem3 = deserializerStore.GetDeserializer<T3>(sourceSchema.InnerSchema3, targetSchema.InnerSchema3.IsNullable);
        var getDefaultValue = defaultValueGetterFactory.GetDefaultValueGetter<(T1, T2, T3)>(targetSchema.IsNullable);
        var sourceIsNullable = sourceSchema.IsNullable;
        var targetIsNullable = targetSchema.IsNullable;

        if (sourceIsNullable && targetIsNullable)
        {
            return (Read<(T1, T2, T3)?>)((BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NativeSerializer.NULL)
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
            return (Read<(T1, T2, T3)>)((BonInput input) =>
            {
                var firstByte = input.Reader.ReadByte();

                if (firstByte == NativeSerializer.NULL)
                {
                    return getDefaultValue(input);
                }

                var item1 = readItem1(input);
                var item2 = readItem2(input);
                var item3 = readItem3(input);

                return (item1, item2, item3);
            });
        }

        if (targetIsNullable)
        {
            return (Read<(T1, T2, T3)?>)((BonInput input) =>
            {
                var item1 = readItem1(input);
                var item2 = readItem2(input);
                var item3 = readItem3(input);

                return (item1, item2, item3);
            });
        }

        return (Read<(T1, T2, T3)>)((BonInput input) =>
        {
            var item1 = readItem1(input);
            var item2 = readItem2(input);
            var item3 = readItem3(input);

            return (item1, item2, item3);
        });
    }
}
