namespace Bon.Serializer.Schemas;

internal class StructureHelper<T>(Func<Schema, T> getKey, IEqualityComparer<T>? equalityComparer = null) where T : notnull
{
    private readonly BinaryWriter _writer = new(new MemoryStream());
    private readonly Dictionary<T, int> _ancestors = new(equalityComparer);

    public byte[] GetBytes(Schema schema)
    {
        WriteSchemaWithoutSchemaType(schema);
        return ((MemoryStream)_writer.BaseStream).ToArray();
    }

    private void WriteSchema(Schema schema)
    {
        _writer.Write((byte)schema.SchemaType);
        WriteSchemaWithoutSchemaType(schema);
    }

    private void WriteSchemaWithoutSchemaType(Schema schema)
    {
        T? key = default;

        if (schema.IsCustom)
        {
            key = getKey(schema);
            if (!_ancestors.TryAdd(key, _ancestors.Count))
            {
                WriteInt(_ancestors[key]);
                return;
            }
        }

        WriteInt(null);
        WriteInnerSchemas(schema.InnerSchemas);
        WriteMembers(schema.Members);

        if (schema.IsCustom)
        {
            _ancestors.Remove(key!);
        }
    }

    private void WriteInnerSchemas(IReadOnlyList<Schema> schemas)
    {
        WriteInt(schemas.Count);
        foreach (var schema in schemas)
        {
            WriteSchema(schema);
        }
    }

    private void WriteMembers(IReadOnlyList<SchemaMember> members)
    {
        WriteInt(members.Count);
        foreach (var member in members)
        {
            WriteInt(member.Id);
            WriteSchema(member.Schema);
        }
    }

    private void WriteInt(int? value) => IntSerializer.Write(_writer, value);
}