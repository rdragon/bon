namespace Bon.Serializer.Deserialization;

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

        if (left.InnerSchemas.Count != right.InnerSchemas.Count)
        {
            return false;
        }

        for (int i = 0; i < left.InnerSchemas.Count; i++)
        {
            if (!Equals(left.InnerSchemas[i], right.InnerSchemas[i]))
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

        return HashCode.Combine(schema.SchemaType, GetHashCode(schema.InnerSchemas.Select(GetHashCode)));
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
