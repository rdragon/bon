namespace Bon.Serializer.Deserialization;

internal sealed class DictionaryDeserializer(DeserializerStore deserializerStore) : IUseReflection
{
    public Delegate? TryCreateDeserializer(Schema sourceSchema, Type targetType)
    {
        if (!sourceSchema.IsDictionary || !targetType.IsGenericType)
        {
            return null;
        }

        var arguments = targetType.GetGenericArguments();

        if (arguments.Length != 2)
        {
            return null;
        }

        var keyType = arguments[0];
        var valueType = arguments[1];

        return (Delegate?)this.GetPrivateMethod(nameof(TryCreateDictionaryReaderFor))
            .MakeGenericMethod(keyType, valueType)
            .Invoke(this, [sourceSchema, targetType])!;
    }

    private Read<Dictionary<TKey, TValue>?>? TryCreateDictionaryReaderFor<TKey, TValue>(Schema sourceSchema, Type targetType)
        where TKey : notnull
    {
        // See bookmark 662741575 for all places where a dictionary is serialized/deserialized.

        if (!typeof(Dictionary<TKey, TValue>).IsAssignableTo(targetType))
        {
            return null;
        }

        var readKey = deserializerStore.GetDeserializer<TKey>(sourceSchema.InnerSchemas[0]);
        var readValue = deserializerStore.GetDeserializer<TValue>(sourceSchema.InnerSchemas[1]);

        return input =>
        {
            if (IntSerializer.Read(input.Reader) is not int count)
            {
                return null;
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