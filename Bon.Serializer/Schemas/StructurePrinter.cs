namespace Bon.Serializer.Schemas;

internal sealed class StructurePrinter(BinaryReader reader)
{
    private readonly StringBuilder _builder = new();
    private int _indentation = 0;

    public string Run()
    {
        AppendLine("Structure");
        ReadSchema(false);
        return _builder.ToString();
    }

    private void ReadSchema(bool readSchemaType)
    {
        if (readSchemaType)
        {
            AppendLine("SchemaType." + (SchemaType)reader.ReadByte());
        }

        if (ReadInt() is { } ancestor)
        {
            AppendLine("Ancestor_" + ancestor);
            return;
        }

        _indentation += 4;
        ReadSchemaArguments();
        ReadMembers();
        _indentation -= 4;
    }

    private void ReadSchemaArguments()
    {
        var count = ReadInt();
        for (int i = 0; i < count; i++)
        {
            ReadSchema(true);
        }
    }

    private void ReadMembers()
    {
        var count = ReadInt();
        for (int i = 0; i < count; i++)
        {
            AppendLine("Member_" + ReadInt());
            ReadSchema(true);
        }
    }

    private void AppendLine(string line) => _builder.AppendLine(new string(' ', _indentation) + line);

    private int? ReadInt() => IntSerializer.Read(reader);
}
