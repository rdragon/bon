using System.Globalization;
//1at
namespace Bon.Serializer.Deserialization;

internal static partial class NativeDeserializer
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public static Delegate? TryCreateDeserializer(DeserializerStore deserializerStore, NativeSchema sourceSchema, Type targetType)
    {
        if (TryGetTargetType(targetType) is not { } foundTargetType)
        {
            return null;
        }

        var outputType = GetOutputType(sourceSchema.SchemaType);
        var method = deserializerStore.GetDefaultNativeReader(sourceSchema.SchemaType);

        return AddTransformation(method, outputType, foundTargetType);
    }

    /// <summary>
    /// The type returned by this method can be used as target type when deserializing binary data with the given schema type.
    /// This deserialization is supported directly after startup.
    /// </summary>
    private static OutputType GetOutputType(SchemaType schemaType)
    {
        return schemaType switch
        {
            SchemaType.String => OutputType.String,
        };

        //return schemaType switch
        //{
        //    // These output types should match the types at bookmark 683558879.
        //    SchemaType.String => typeof(string),
        //    SchemaType.Bool => typeof(bool),
        //    SchemaType.Byte => typeof(byte),
        //    SchemaType.SByte => typeof(sbyte),
        //    SchemaType.Short => typeof(short),
        //    SchemaType.UShort => typeof(ushort),
        //    SchemaType.Int => typeof(int),
        //    SchemaType.UInt => typeof(uint),
        //    SchemaType.Long => typeof(long),
        //    SchemaType.ULong => typeof(ulong),
        //    SchemaType.WholeNumber => typeof(ulong?),
        //    SchemaType.SignedWholeNumber => typeof(long?),
        //    SchemaType.Float => typeof(float),
        //    SchemaType.Double => typeof(double),
        //    SchemaType.Decimal => typeof(decimal?),
        //    SchemaType.Guid => typeof(Guid?),
        //    SchemaType.NullableDouble => typeof(double?),
        //    _ => throw new ArgumentException($"No default output type for '{schemaType}' found"),
        //};
    }

    private static TargetType? TryGetTargetType(Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => TargetType.String,
            _ => null,
        };
    }
}
