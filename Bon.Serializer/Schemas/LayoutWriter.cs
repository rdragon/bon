namespace Bon.Serializer.Schemas;

internal sealed class LayoutWriter(BinaryWriter writer)
{
    public void Write(Layout layout)
    {
        WriteLayoutId(layout.Id);
        WriteMembers(layout.Members);
    }

    private void WriteMembers(IReadOnlyList<SchemaMember> members)
    {
        WriteInt(members.Count, "Member count out of range", max: 10_000);
        foreach (var member in members)
        {
            WriteInt(member.Id, "Member ID out of range", min: 0);
            WriteSchema(member.Schema);
        }
    }

    public void WriteSchema(Schema schema)
    {
        WriteSchemaType(schema.SchemaType);
        WriteSchemaArguments(schema.SchemaArguments);
        WriteLayoutId(schema);
    }

    private void WriteSchemaArguments(IReadOnlyList<Schema> schemas)
    {
        foreach (var schema in schemas)
        {
            WriteSchema(schema);
        }
    }

    private void WriteLayoutId(Schema schema)
    {
        if (schema.IsCustom)
        {
            WriteLayoutId(schema.LayoutId);
        }
    }

    private void WriteLayoutId(int layoutId)
    {
        WriteInt(layoutId, "Layout ID out of range", min: 1);
    }

    private void WriteInt(int value, string? message, int min = int.MinValue, int max = int.MaxValue)
    {
        Trace.Assert(min <= value && value <= max, message);
        IntSerializer.Write(writer, value);
    }

    private void WriteSchemaType(SchemaType schemaType) => writer.Write((byte)schemaType);
}
