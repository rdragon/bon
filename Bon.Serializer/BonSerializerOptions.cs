namespace Bon.Serializer;

/// <summary>
/// Currently there is no customization possible and this class is empty.
/// </summary>
public sealed class BonSerializerOptions
{
    /// <summary>
    /// A hidden option that is used by unit tests.
    /// If false then when serializing a byte, sbyte, short, ushort, int, uint, long or ulong the resulting schema type will
    /// be that of the type that was serialized.
    /// If true then the resulting schema type can be smaller.
    /// For example, if the value fits in a byte, its type doesn't matter, the resulting schema type will be that of a byte.
    /// </summary>
    internal bool AllowSchemaTypeOptimization { get; set; } = true;

    /// <summary>
    /// A hidden option that is used by unit tests.
    /// If false then the header is not included in the serialized message.
    /// </summary>
    internal bool IncludeHeader { get; set; } = true;
}
