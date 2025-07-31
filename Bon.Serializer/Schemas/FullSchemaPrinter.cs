
namespace Bon.Serializer.Schemas;

/// <summary>
/// Prints schemas in C# syntax.
/// </summary>
internal sealed class FullSchemaPrinter
{
    private readonly StringBuilder _output = new();

    private readonly Queue<Schema> _yetToPrint = [];

    private readonly HashSet<Schema> _printed = [];

    private readonly HashSet<SchemaType> _schemaTypesToPrint = [];

    private readonly Dictionary<Schema, string> _classNames = [];

    /// <summary>
    /// Prints the given schema and all its inner schemas.
    /// Every schema is printed as a C# class.
    /// The class names include the schema type and the layout ID.
    /// </summary>
    public string Print(Schema schema)
    {
        var type = GetTypeOfSchema(schema);
        AppendLine("// " + new string('-', type.Length));
        AppendLine($"// {type}");
        AppendLine("// " + new string('-', type.Length));
        PrintSchemas();
        PrintSchemaTypes();
        return _output.ToString();
    }

    public string Print(IEnumerable<KeyValuePair<Type, Schema>> schemas)
    {
        var table = schemas.Select(pair => (Type: pair.Key.ToString(), Schema: GetTypeOfSchema(pair.Value))).ToArray();
        if (table.Length == 0)
        {
            return "";
        }

        var columnWidth = table.Max(tuple => tuple.Type.Length);
        foreach (var (type, schema) in table.OrderBy(tuple => tuple.Type))
        {
            AppendLine("// " + type.PadLeft(columnWidth) + "  " + schema);
        }

        PrintSchemas();
        PrintSchemaTypes();
        return _output.ToString();
    }

    public string Print(IEnumerable<Layout> layouts)
    {
        PrintLayouts(layouts);
        PrintSchemaTypes();

        return _output.ToString();
    }

    private void PrintLayouts(IEnumerable<Layout> layouts)
    {
        var first = true;
        foreach (var layout in layouts)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                AppendLine();
            }
            PrintLayout(layout);
        }
    }

    private void PrintSchemas()
    {
        while (_yetToPrint.TryDequeue(out var otherSchema))
        {
            PrintSchema(otherSchema);
        }
    }

    private void PrintSchema(Schema schema)
    {
        if (!_printed.Add(schema))
        {
            return;
        }

        AppendLine();
        AppendLine($"class {GetClassName(schema)}(");
        PrintMembers(schema.Members);
        AppendLine(");");
    }

    private void PrintLayout(Layout layout)
    {
        AppendLine($"class Layout_{layout.Id}(");
        PrintMembers(layout.Members);
        AppendLine(");");
    }

    private void PrintMembers(IReadOnlyList<SchemaMember> members)
    {
        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            var suffix = i < members.Count - 1 ? "," : "";
            AppendLine($"    {GetTypeOfSchema(member.Schema)} Member_{member.Id}{suffix}");
        }
    }

    private string GetTypeOfSchema(Schema schema)
    {
        if (IsSpecialNativeType(schema.SchemaType))
        {
            _schemaTypesToPrint.Add(schema.SchemaType);
        }

        return schema switch
        {
            { IsNative: true } => LimitedSchemaPrinter.PrintSchemaType(schema.SchemaType),
            { IsArray: true } => $"{GetTypeOfSchema(schema.SchemaArguments[0])}[]?",
            { IsDictionary: true } => "DICTIONARY",
            { IsTuple: true } => GetTypeOfTupleSchema(schema),
            { IsCustom: true } => GetTypeOfCustomSchema(schema),
        };
    }

    private string GetTypeOfTupleSchema(Schema schema)
    {
        var suffix = schema.IsNullable ? "?" : "";

        return $"({string.Join(", ", schema.SchemaArguments.Select(GetTypeOfSchema))}){suffix}";
    }

    private static bool IsSpecialNativeType(SchemaType schemaType)
    {
        return schemaType is SchemaType.WholeNumber or SchemaType.SignedWholeNumber or SchemaType.FractionalNumber;
    }

    private string GetTypeOfCustomSchema(Schema schema)
    {
        if (schema.SchemaType == SchemaType.NullableRecord)
        {
            return GetTypeOfCustomSchema(schema.GetClone(SchemaType.Record)) + "?";
        }

        _yetToPrint.Enqueue(schema);
        return GetClassName(schema) + (schema.SchemaType == SchemaType.Union ? "?" : "");
    }

    private string GetClassName(Schema schema)
    {
        if (_classNames.TryGetValue(schema, out var name))
        {
            return name;
        }

        name = schema.SchemaType + "_" + schema.LayoutId;
        _classNames.Add(schema, name);
        return name;
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
