using System.Diagnostics;

namespace Bon.Serializer.Schemas;

internal sealed class LayoutWriter(BinaryWriter writer)
{
    public void Write(Layout layout)
    {
        WriteInt(layout.Id, min: 1);
        WriteMembers(layout.Members);
    }

    private void WriteMembers(IReadOnlyList<SchemaMember> members)
    {
        WriteInt(members.Count, max: 10_000);
        foreach (var member in members)
        {
            WriteInt(member.Id, min: 0);
            WriteSchema(member.Schema);
        }
    }

    private void WriteSchema(Schema schema)
    {
        WriteSchemaType(schema.SchemaType);
        WriteInnerSchemas(schema.InnerSchemas);
        WriteLayoutId(schema);
    }

    private void WriteInnerSchemas(IReadOnlyList<Schema> schemas)
    {
        foreach (var schema in schemas)
        {
            WriteSchema(schema);
        }
    }

    private void WriteLayoutId(Schema schema)
    {
        if (schema.IsCustomSchema)
        {
            WriteInt(schema.LayoutId, min: 1);
        }
    }

    private void WriteInt(int value, int min = int.MinValue, int max = int.MaxValue)
    {
        IntSerializer.Write(writer, value);
        Trace.Assert(min <= value && value <= max);
    }

    private void WriteSchemaType(SchemaType schemaType) => writer.Write((byte)schemaType);
}
