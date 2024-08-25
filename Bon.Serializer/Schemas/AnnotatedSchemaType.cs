namespace Bon.Serializer.Schemas;

/// <summary>
/// A <see cref="Schemas.SchemaType"/> together with a nullability bool.
/// </summary>
public readonly record struct AnnotatedSchemaType(SchemaType SchemaType, bool IsNullable)
{
    /// <summary>
    /// Returns the type corresponding to this instance.
    /// Throws an exception if the instance does not represent a native type.
    /// </summary>
    public Type ToNativeType()
    {
        return (SchemaType, IsNullable) switch
        {
            (SchemaType.String, false) => typeof(string),
            (SchemaType.Bool, false) => typeof(bool),
            (SchemaType.Byte, false) => typeof(byte),
            (SchemaType.SByte, false) => typeof(sbyte),
            (SchemaType.Short, false) => typeof(short),
            (SchemaType.UShort, false) => typeof(ushort),
            (SchemaType.Int, false) => typeof(int),
            (SchemaType.UInt, false) => typeof(uint),
            (SchemaType.Long, false) => typeof(long),
            (SchemaType.ULong, false) => typeof(ulong),
            (SchemaType.WholeNumber, false) => typeof(ulong),
            (SchemaType.SignedWholeNumber, false) => typeof(long),
            (SchemaType.Float, false) => typeof(float),
            (SchemaType.Double, false) => typeof(double),
            (SchemaType.Decimal, false) => typeof(decimal),
            (SchemaType.Guid, false) => typeof(Guid),

            (SchemaType.String, true) => typeof(string),
            (SchemaType.Bool, true) => typeof(bool?),
            (SchemaType.WholeNumber, true) => typeof(ulong?),
            (SchemaType.SignedWholeNumber, true) => typeof(long?),
            (SchemaType.Float, true) => typeof(float?),
            (SchemaType.Double, true) => typeof(double?),
            (SchemaType.Decimal, true) => typeof(decimal?),
            (SchemaType.Guid, true) => typeof(Guid?),

            _ => throw new InvalidOperationException($"Cannot convert '{this}' to native type."),
        };
    }

    /// <summary>
    /// Returns the native schema corresponding to this instance.
    /// Throws an exception if the instance does not represent a native type.
    /// </summary>
    public NativeSchema ToNativeSchema()
    {
        return (SchemaType, IsNullable) switch
        {
            (SchemaType.String, false) => NativeSchema.String,
            (SchemaType.Bool, false) => NativeSchema.Bool,
            (SchemaType.Byte, false) => NativeSchema.Byte,
            (SchemaType.SByte, false) => NativeSchema.SByte,
            (SchemaType.Short, false) => NativeSchema.Short,
            (SchemaType.UShort, false) => NativeSchema.UShort,
            (SchemaType.Int, false) => NativeSchema.Int,
            (SchemaType.UInt, false) => NativeSchema.UInt,
            (SchemaType.Long, false) => NativeSchema.Long,
            (SchemaType.ULong, false) => NativeSchema.ULong,
            (SchemaType.WholeNumber, false) => NativeSchema.WholeNumber,
            (SchemaType.SignedWholeNumber, false) => NativeSchema.SignedWholeNumber,
            (SchemaType.Float, false) => NativeSchema.Float,
            (SchemaType.Double, false) => NativeSchema.Double,
            (SchemaType.Decimal, false) => NativeSchema.Decimal,
            (SchemaType.Guid, false) => NativeSchema.Guid,

            (SchemaType.String, true) => NativeSchema.NullableString,
            (SchemaType.Bool, true) => NativeSchema.NullableBool,
            (SchemaType.WholeNumber, true) => NativeSchema.NullableWholeNumber,
            (SchemaType.SignedWholeNumber, true) => NativeSchema.NullableSignedWholeNumber,
            (SchemaType.Float, true) => NativeSchema.NullableFloat,
            (SchemaType.Double, true) => NativeSchema.NullableDouble,
            (SchemaType.Decimal, true) => NativeSchema.NullableDecimal,
            (SchemaType.Guid, true) => NativeSchema.NullableGuid,

            _ => throw new InvalidOperationException($"Cannot convert '{this}' to native schema."),
        };
    }
}
