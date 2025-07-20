namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a string, bool, byte, sbyte, short, ushort, int, uint, long, ulong, float, double, or decimal.
/// </summary>
public sealed class NativeSchema(SchemaType schemaType) : Schema(schemaType)
{
    public override IEnumerable<Schema> GetInnerSchemas() => [];

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return obj is NativeSchema other && SchemaType == other.SchemaType;
    }

    // These properties are used by the source generated code.
    // For every native schema type there should be a property with the correct name.
    public static NativeSchema String { get; } = new(SchemaType.String);
    public static NativeSchema Byte { get; } = new(SchemaType.Byte);
    public static NativeSchema SByte { get; } = new(SchemaType.SByte);
    public static NativeSchema Short { get; } = new(SchemaType.Short);
    public static NativeSchema UShort { get; } = new(SchemaType.UShort);
    public static NativeSchema Int { get; } = new(SchemaType.Int);
    public static NativeSchema UInt { get; } = new(SchemaType.UInt);
    public static NativeSchema Long { get; } = new(SchemaType.Long);
    public static NativeSchema ULong { get; } = new(SchemaType.ULong);
    public static NativeSchema Float { get; } = new(SchemaType.Float);
    public static NativeSchema Double { get; } = new(SchemaType.Double);
    public static NativeSchema NullableDecimal { get; } = new(SchemaType.NullableDecimal);
    public static NativeSchema WholeNumber { get; } = new(SchemaType.WholeNumber);
    public static NativeSchema SignedWholeNumber { get; } = new(SchemaType.SignedWholeNumber);
    public static NativeSchema FractionalNumber { get; } = new(SchemaType.FractionalNumber);

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public static NativeSchema Create(SchemaType schemaType)
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
