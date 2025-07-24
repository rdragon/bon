namespace Bon.Serializer.Schemas;

// Used by source generated code.
public static class NativeSchema
{
    public static Schema String { get; } = Schema.Create(SchemaType.String);
    public static Schema Byte { get; } = Schema.Create(SchemaType.Byte);
    public static Schema SByte { get; } = Schema.Create(SchemaType.SByte);
    public static Schema Short { get; } = Schema.Create(SchemaType.Short);
    public static Schema UShort { get; } = Schema.Create(SchemaType.UShort);
    public static Schema Int { get; } = Schema.Create(SchemaType.Int);
    public static Schema UInt { get; } = Schema.Create(SchemaType.UInt);
    public static Schema Long { get; } = Schema.Create(SchemaType.Long);
    public static Schema ULong { get; } = Schema.Create(SchemaType.ULong);
    public static Schema Float { get; } = Schema.Create(SchemaType.Float);
    public static Schema Double { get; } = Schema.Create(SchemaType.Double);
    public static Schema NullableDecimal { get; } = Schema.Create(SchemaType.NullableDecimal);
    public static Schema WholeNumber { get; } = Schema.Create(SchemaType.WholeNumber);
    public static Schema SignedWholeNumber { get; } = Schema.Create(SchemaType.SignedWholeNumber);
    public static Schema FractionalNumber { get; } = Schema.Create(SchemaType.FractionalNumber);

    public static Schema Create(SchemaType schemaType) => FromSchemaType(schemaType);

    public static Schema FromSchemaType(SchemaType schemaType)
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
}

// Used by source generated code.
public static class ArraySchema
{
    public static Schema ByteArray => Schema.Create(SchemaType.Array, [NativeSchema.Byte]);
}
