namespace Bon.Serializer.Schemas;

internal sealed class StructureFactory
{
    /// <summary>
    /// Creates a structure for a layout.
    /// The layout IDs are not part of the structure, but they are used to spot recursive schemas.
    /// If two schemas have the same layout ID then their members are the same.
    /// </summary>
    public static Structure Create(Layout layout)
    {
        return Create(Schema.Create(SchemaType.Record, null, layout.Id, layout.Members), false);
    }

    /// <summary>
    /// Creates a structure for a schema.
    /// The layout IDs are not part of the structure, but they are used to spot recursive schemas.
    /// If two schemas have the same layout ID then their members are the same.
    /// </summary>
    /// <param name="includeSchemaType">
    /// Whether to include the schema type of the schema in the structure.
    /// Schema types of inner schemas are always included.
    /// </param>
    public static Structure Create(Schema schema, bool includeSchemaType)
    {
        return new Structure(new StructureFactory().GetBytes(schema, includeSchemaType));
    }

    private readonly BinaryWriter _writer = new(new MemoryStream());
    private readonly Dictionary<int, int> _ancestors = [];

    public byte[] GetBytes(Schema schema, bool includeSchemaType)
    {
        WriteSchema(schema, includeSchemaType);
        return ((MemoryStream)_writer.BaseStream).ToArray();
    }

    private void WriteSchema(Schema schema, bool includeSchemaType)
    {
        if (includeSchemaType)
        {
            _writer.Write((byte)schema.SchemaType);
        }

        if (schema.IsCustom)
        {
            // Bookmark 557955753
            if (!_ancestors.TryAdd(schema.LayoutId, _ancestors.Count))
            {
                WriteInt(_ancestors[schema.LayoutId]);
                return;
            }
        }

        WriteInt(null);
        WriteSchemaArguments(schema.SchemaArguments);
        WriteMembers(schema.Members);

        if (schema.IsCustom)
        {
            _ancestors.Remove(schema.LayoutId);
        }
    }

    private void WriteSchemaArguments(IReadOnlyList<Schema> schemaArguments)
    {
        WriteInt(schemaArguments.Count);
        foreach (var schema in schemaArguments)
        {
            WriteSchema(schema, true);
        }
    }

    private void WriteMembers(IReadOnlyList<SchemaMember> members)
    {
        WriteInt(members.Count);
        foreach (var member in members)
        {
            WriteInt(member.Id);
            WriteSchema(member.Schema, true);
        }
    }

    private void WriteInt(int? value) => IntSerializer.Write(_writer, value);
}