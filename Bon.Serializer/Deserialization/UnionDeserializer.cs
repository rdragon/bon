namespace Bon.Serializer.Deserialization;

internal sealed class UnionDeserializer(
     DeserializerStore deserializerStore) : IUseReflection
{
    public Read<T?>? TryCreateDeserializer<T>(Schema sourceSchema, Schema? targetSchema)
    {
        if (sourceSchema is not UnionSchema unionSourceSchema || targetSchema is not UnionSchema unionTargetSchema)
        {
            return null;
        }

        return CreateDeserializer<T>(unionSourceSchema, unionTargetSchema);
    }

    public Read<T?> CreateDeserializer<T>(UnionSchema sourceSchema, UnionSchema targetSchema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        var deserializers = new Dictionary<int, Read<T?>>();
        var targetMembers = targetSchema.Members.ToDictionary(member => member.Id);

        foreach (var member in sourceSchema.Members)
        {
            if (targetMembers.TryGetValue(member.Id, out var targetMember))
            {
                var type = deserializerStore.MemberTypes[(typeof(T), member.Id)];

                deserializers[member.Id] = (Read<T?>)this.GetPrivateMethod(nameof(GetRecordDeserializer))
                    .MakeGenericMethod(type)
                    .Invoke(this, [member.Schema])!;
            }
            else
            {
                deserializers[member.Id] = deserializerStore.GetSkipper<T>(member.Schema);
            }
        }

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int id)
            {
                return default;
            }

            var deserializer = deserializers[id];

            return deserializer(input);
        };
    }

    private Delegate GetRecordDeserializer<T>(Schema sourceSchema) => deserializerStore.GetDeserializer<T>(sourceSchema);
}
