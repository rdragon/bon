
namespace Bon.Serializer.Schemas;

internal class SchemaPrinter
{
    private readonly StringBuilder _output = new();

    private readonly Queue<CustomSchema> _yetToPrint = [];

    private readonly HashSet<Schema1> _printed = [];

    private readonly HashSet<SchemaType> _schemaTypesToPrint = [];

    private readonly Dictionary<Schema1, string> _classNames = [];

    public string Print(Schema1 schema)
    {
        AppendLine($"{GetType(schema)} message;");
        PrintSchemas();
        PrintSchemaTypes();

        return _output.ToString();
    }

    private void PrintSchemas()
    {
        while (_yetToPrint.TryDequeue(out var otherSchema))
        {
            PrintSchema(otherSchema);
        }
    }

    private void PrintSchema(CustomSchema schema)
    {
        if (!_printed.Add(schema))
        {
            return;
        }

        AppendLine();
        AppendLine($"record class {GetClassName(schema)}(");
        PrintMembers(schema.Members);
        AppendLine(");");
    }

    private void PrintMembers(IReadOnlyList<SchemaMember1> members)
    {
        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            var suffix = i < members.Count - 1 ? "," : "";
            AppendLine($"    {GetType(member.Schema)} Member_{member.Id}{suffix}");
        }
    }

    private string GetType(Schema1 schema)
    {
        if (IsCustomNativeType(schema.SchemaType))
        {
            _schemaTypesToPrint.Add(schema.SchemaType);
        }

        return schema switch
        {
            NativeSchema nativeSchema => GetType(nativeSchema),
            ArraySchema arraySchema => $"{GetType(arraySchema.InnerSchema)}[]?",
            DictionarySchema => "DICTIONARY",
            Tuple2Schema tuple2Schema => $"({GetType(tuple2Schema.InnerSchema1)}, {GetType(tuple2Schema.InnerSchema2)})",
            Tuple3Schema tuple3Schema => $"({GetType(tuple3Schema.InnerSchema1)}, {GetType(tuple3Schema.InnerSchema2)}, {GetType(tuple3Schema.InnerSchema3)})",
            CustomSchema customSchema => GetType(customSchema),
        };
    }

    private static string GetType(NativeSchema nativeSchema)
    {
        var schemaType = nativeSchema.SchemaType;
        var text = schemaType.ToString();

        return IsCustomNativeType(schemaType) ? text : text.ToLowerInvariant();
    }

    private static bool IsCustomNativeType(SchemaType schemaType)
    {
        return schemaType is SchemaType.WholeNumber or SchemaType.SignedWholeNumber or SchemaType.FractionalNumber;
    }

    private string GetType(CustomSchema customSchema)
    {
        if (customSchema.SchemaType == SchemaType.NullableRecord)
        {
            return GetType(SetSchemaType((RecordSchema)customSchema, SchemaType.Record)) + "?";
        }

        _yetToPrint.Enqueue(customSchema);
        return GetClassName(customSchema) + (customSchema.SchemaType == SchemaType.Union ? "?" : "");
    }

    private static RecordSchema SetSchemaType(RecordSchema recordSchema, SchemaType schemaType)
    {
        return new RecordSchema(schemaType)
        {
            Members = recordSchema.Members,
            LayoutId = recordSchema.LayoutId
        };
    }

    private string GetClassName(Schema1 schema)
    {
        if (_classNames.TryGetValue(schema, out var name))
        {
            return name;
        }

        name = (schema is UnionSchema ? "I" : "") + ConvertToName(_classNames.Count);
        _classNames.Add(schema, name);
        return name;
    }

    private static string ConvertToName(int value)
    {
        var quotient = value / 26;
        var remainder = value % 26;

        var restOfName = quotient > 0 ? ConvertToName(quotient - 1) : "";
        var letter = (char)('A' + remainder);
        return restOfName + letter;
    }

    private void PrintSchemaTypes()
    {
        if (_schemaTypesToPrint.Count == 0)
        {
            return;
        }

        AppendLine();

        foreach (var schemaType in _schemaTypesToPrint)
        {
            AppendLine($"class {schemaType};");
        }
    }

    private void AppendLine(string line = "") => _output.AppendLine(line);
}
