namespace Bon.Serializer.Schemas;

public sealed partial class Schema
{
    private SchemaType _schemaType;

    private SchemaFlags _schemaFlags;

    //2at
    public IReadOnlyList<Schema> InnerSchemas { get; set; } = null!;

    //2at, always set except
    public int LayoutId { get; set; }

    //2at
    public IReadOnlyList<SchemaMember> Members { get; set; } = null!;

    public SchemaType SchemaType
    {
        get => _schemaType;

        private set
        {
            _schemaType = value;
            _schemaFlags = value.GetSchemaFlags();
        }
    }

    public bool IsCustom => (_schemaFlags & SchemaFlags.IsCustom) != 0;

    public bool IsRecord => SchemaType == SchemaType.Record || SchemaType == SchemaType.NullableRecord;

    public bool IsNative => (_schemaFlags & SchemaFlags.IsNative) != 0;

    public bool IsTuple => (_schemaFlags & SchemaFlags.IsTuple) != 0;

    public bool IsTuple2 => SchemaType == SchemaType.Tuple2 || SchemaType == SchemaType.NullableTuple2;

    public bool IsTuple3 => SchemaType == SchemaType.Tuple3 || SchemaType == SchemaType.NullableTuple3;

    public bool IsNullable => (_schemaFlags & SchemaFlags.IsNullable) != 0;

    public bool IsArray => SchemaType == SchemaType.Array;

    public bool IsDictionary => SchemaType == SchemaType.Dictionary;

    public bool IsUnion => SchemaType == SchemaType.Union;

    public override bool Equals(object? obj) => obj is Schema schema && SchemaComparer.Equals(this, schema);

    public override int GetHashCode() => SchemaComparer.GetHashCode(this);

    public Schema GetClone(SchemaType schemaType) => Create(schemaType, InnerSchemas, LayoutId, Members);
}

public readonly record struct SchemaMember(int Id, Schema Schema);

public readonly record struct Layout(int Id, IReadOnlyList<SchemaMember> Members);
