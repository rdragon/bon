namespace Bon.Serializer.Deserialization;

internal sealed class UnionDeserializer(
     DeserializerStore deserializerStore,
     DefaultValueGetterFactory defaultValueGetterFactory) : IUseReflection
{
    public Read<T> CreateDeserializer<T>(UnionSchema sourceSchema, UnionSchema targetSchema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        var deserializers = new Dictionary<int, Read<T>>();
        var targetMembers = targetSchema.Members.ToDictionary(member => member.Id);
        var getDefaultValue = defaultValueGetterFactory.GetDefaultValueGetter<T>(targetSchema.IsNullable);

        foreach (var member in sourceSchema.Members)
        {
            if (targetMembers.TryGetValue(member.Id, out var targetMember))
            {
                var type = deserializerStore.MemberTypes[(typeof(T), member.Id)];

                deserializers[member.Id] = (Read<T>)this.GetPrivateMethod(nameof(GetRecordDeserializer))
                    .MakeGenericMethod(type)
                    .Invoke(this, [member.Schema, targetSchema.IsNullable])!;
            }
            else
            {
                deserializers[member.Id] = deserializerStore.GetSkipper<T>(member.Schema, targetSchema.IsNullable);
            }
        }

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int id)
            {
                return getDefaultValue(input);
            }

            var deserialize = deserializers[id];

            return deserialize(input);
        };
    }

    private Delegate GetRecordDeserializer<T>(Schema sourceSchema, bool isNullable)
    {
        return deserializerStore.GetDeserializer<T>(sourceSchema, isNullable);
    }
}
