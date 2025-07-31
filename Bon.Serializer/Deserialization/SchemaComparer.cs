namespace Bon.Serializer.Deserialization;

/// <summary>
/// Compares schemas based on their structure.
/// Two schemas have the same structure if they have the same schema type and if their schema
/// arguments and members have the same structure and if their members have the same IDs.
/// Layout IDs are heavily used by this comparer.
/// For custom schemas, instead of comparing their members, the layout IDs are compared.
/// This means the comparison is relatively fast and does not need to take into account
/// that schemas can be recursive.
/// This comparer is used to fetch the right deserializer method (see the deserializer store).
/// </summary>
internal static class SchemaComparer
{
    public static bool Equals(Schema left, Schema right)
    {
        if (left.SchemaType != right.SchemaType)
        {
            return false;
        }

        if (left.IsCustom || left.IsNative)
        {
            return left.LayoutId == right.LayoutId;
        }

        if (left.SchemaArguments.Count != right.SchemaArguments.Count)
        {
            return false;
        }

        for (int i = 0; i < left.SchemaArguments.Count; i++)
        {
            if (!Equals(left.SchemaArguments[i], right.SchemaArguments[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static int GetHashCode(Schema schema)
    {
        if (schema.IsCustom || schema.IsNative)
        {
            return HashCode.Combine(schema.SchemaType, schema.LayoutId);
        }

        return HashCode.Combine(schema.SchemaType, GetHashCode(schema.SchemaArguments.Select(GetHashCode)));
    }

    private static int GetHashCode(IEnumerable<int> numbers)
    {
        var hash = new HashCode();
        foreach (var num in numbers)
        {
            hash.Add(num);
        }
        return hash.ToHashCode();
    }
}
