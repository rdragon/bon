namespace Bon.Serializer.Deserialization;

internal sealed class DictionaryDeserializer(DeserializerStore deserializerStore) : IUseReflection
{
    public Delegate CreateDeserializer<T>(DictionarySchema sourceSchema, DictionarySchema targetSchema)
    {
        var arguments = typeof(T).GetGenericArguments();
        var keyType = arguments[0];
        var valueType = arguments[1];

        return (Delegate)this.GetPrivateMethod(nameof(CreateDictionaryReaderFor))
            .MakeGenericMethod(keyType, valueType)
            .Invoke(this, [sourceSchema, targetSchema])!;
    }

    private Read<Dictionary<TKey, TValue>?> CreateDictionaryReaderFor<TKey, TValue>(DictionarySchema sourceSchema, DictionarySchema targetSchema) where TKey : notnull
    {
        // See bookmark 662741575 for all places where a dictionary is serialized/deserialized.

        var readKey = deserializerStore.GetDeserializer<TKey>(sourceSchema.InnerSchema1, targetSchema.InnerSchema1.IsNullable);
        var readValue = deserializerStore.GetDeserializer<TValue>(sourceSchema.InnerSchema2, targetSchema.InnerSchema2.IsNullable);
        var targetIsNullable = targetSchema.IsNullable;

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
            {
                return targetIsNullable ? null : [];
            }

            var dictionary = new Dictionary<TKey, TValue>(count);

            for (var i = 0; i < count; i++)
            {
                var key = readKey(input);
                var value = readValue(input);
                dictionary[key] = value;
            }

            return dictionary;
        };
    }
}