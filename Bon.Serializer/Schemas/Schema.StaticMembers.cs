namespace Bon.Serializer.Schemas;

// Used by source generated code.
partial class Schema
{
    public static Schema Create(
        SchemaType schemaType,
        IReadOnlyList<Schema>? innerSchemas = null,
        int layoutId = 0,
        IReadOnlyList<SchemaMember>? members = null,
        bool forceNewSchema = false)
    {
        if (!forceNewSchema && schemaType.IsNativeSchema())
        {
            return GetNativeSchema(schemaType);
        }

        return new Schema
        {
            SchemaType = schemaType,
            InnerSchemas = innerSchemas ?? [],
            LayoutId = layoutId,
            Members = members ?? []
        };
    }

    public static Schema String { get; } = ForceCreate(SchemaType.String);
    public static Schema Byte { get; } = ForceCreate(SchemaType.Byte);
    public static Schema SByte { get; } = ForceCreate(SchemaType.SByte);
    public static Schema Short { get; } = ForceCreate(SchemaType.Short);
    public static Schema UShort { get; } = ForceCreate(SchemaType.UShort);
    public static Schema Int { get; } = ForceCreate(SchemaType.Int);
    public static Schema UInt { get; } = ForceCreate(SchemaType.UInt);
    public static Schema Long { get; } = ForceCreate(SchemaType.Long);
    public static Schema ULong { get; } = ForceCreate(SchemaType.ULong);
    public static Schema Float { get; } = ForceCreate(SchemaType.Float);
    public static Schema Double { get; } = ForceCreate(SchemaType.Double);
    public static Schema NullableDecimal { get; } = ForceCreate(SchemaType.NullableDecimal);
    public static Schema WholeNumber { get; } = ForceCreate(SchemaType.WholeNumber);
    public static Schema SignedWholeNumber { get; } = ForceCreate(SchemaType.SignedWholeNumber);
    public static Schema FractionalNumber { get; } = ForceCreate(SchemaType.FractionalNumber);
    public static Schema ByteArray { get; } = Create(SchemaType.Array, [Byte]);

    public static Schema GetNativeSchema(SchemaType schemaType)
    {
        return schemaType switch
        {
            SchemaType.String => String,
            SchemaType.Byte => Byte,
            SchemaType.SByte => SByte,
            SchemaType.Short => Short,
            SchemaType.UShort => UShort,
            SchemaType.Int => Int,
            SchemaType.UInt => UInt,
            SchemaType.Long => Long,
            SchemaType.ULong => ULong,
            SchemaType.Float => Float,
            SchemaType.Double => Double,
            SchemaType.NullableDecimal => NullableDecimal,
            SchemaType.WholeNumber => WholeNumber,
            SchemaType.SignedWholeNumber => SignedWholeNumber,
            SchemaType.FractionalNumber => FractionalNumber,
        };
    }

    private static Schema ForceCreate(SchemaType schemaType) => Create(schemaType, forceNewSchema: true);
}
