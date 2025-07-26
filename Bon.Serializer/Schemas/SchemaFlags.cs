namespace Bon.Serializer.Schemas;

[Flags]
internal enum SchemaFlags
{
    None = 0,
    IsNullable = 1,
    IsNative = 2,
    IsCustom = 4,
    IsTuple = 8,
}
