using System.Globalization;

namespace Bon.Serializer.Deserialization;

internal partial class NativeDeserializer(DeserializerStore deserializerStore)
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    private Dictionary<Type, NativeType>? _nativeTypes = null;

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public Delegate? TryCreateDeserializer(Schema sourceSchema, Type targetType)
    {
        if (!sourceSchema.IsNative)
        {
            return null;
        }

        return TryCreateDeserializerNow(sourceSchema, targetType);
    }

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    private Delegate? TryCreateDeserializerNow(Schema sourceSchema, Type targetType)
    {
        if (TryGetNativeType(targetType) is not { } foundTargetType)
        {
            return null;
        }

        var outputType = GetOutputType(sourceSchema.SchemaType);
        var method = deserializerStore.GetDefaultNativeReader(sourceSchema.SchemaType);

        return CreateDeserializer_ChangeOutputType(method, outputType, foundTargetType);
    }

    /// <summary>
    /// The type returned by this method can be used as target type when deserializing binary data with the given schema type.
    /// This deserialization is supported directly after startup.
    /// </summary>
    private static NativeType GetOutputType(SchemaType schemaType)
    {
        // The native types should match the types at bookmark 683558879.
        return schemaType switch
        {
            SchemaType.String => NativeType.String,
            SchemaType.Byte => NativeType.Byte,
            SchemaType.SByte => NativeType.SByte,
            SchemaType.Short => NativeType.Short,
            SchemaType.UShort => NativeType.UShort,
            SchemaType.Int => NativeType.Int,
            SchemaType.UInt => NativeType.UInt,
            SchemaType.Long => NativeType.Long,
            SchemaType.ULong => NativeType.ULong,
            SchemaType.Float => NativeType.Float,
            SchemaType.Double => NativeType.Double,
            SchemaType.NullableDecimal => NativeType.NullableDecimal,
            SchemaType.WholeNumber => NativeType.NullableULong,
            SchemaType.SignedWholeNumber => NativeType.NullableLong,
            SchemaType.FractionalNumber => NativeType.NullableDouble,
        };
    }

    private Dictionary<Type, NativeType> NativeTypes => _nativeTypes ??= new()
    {
        // The native types should match the types at bookmark 683558879.
        { typeof(string), NativeType.String },
        { typeof(byte), NativeType.Byte },
        { typeof(sbyte), NativeType.SByte },
        { typeof(short), NativeType.Short },
        { typeof(ushort), NativeType.UShort },
        { typeof(int), NativeType.Int },
        { typeof(uint), NativeType.UInt },
        { typeof(long), NativeType.Long },
        { typeof(ulong), NativeType.ULong },
        { typeof(float), NativeType.Float },
        { typeof(double), NativeType.Double },
        { typeof(decimal?), NativeType.NullableDecimal },
        { typeof(ulong?), NativeType.NullableULong },
        { typeof(long?), NativeType.NullableLong },
        { typeof(double?), NativeType.NullableDouble },
    };

    private NativeType? TryGetNativeType(Type type) => NativeTypes.TryGetValue(type, out var nativeType) ? nativeType : null;
}
