namespace Bon.Serializer.Schemas;

/// <summary>
/// The collection of members inside a <see cref="CustomSchema"/>.
/// </summary>
public readonly struct SchemaContents(IReadOnlyList<SchemaMember> members)
{
    public IReadOnlyList<SchemaMember> Members { get; } = members;
}

public sealed class SchemaContentsEqualityComparer : IEqualityComparer<SchemaContents>
{
    public static SchemaContentsEqualityComparer Instance { get; } = new();

    public bool Equals(SchemaContents x, SchemaContents y)
    {
        if (x.Members.Count != y.Members.Count)
        {
            return false;
        }

        var ancestors = new Dictionary<Ancestor, int>(AncestorEqualityComparer.Instance);

        for (int i = 0; i < x.Members.Count; i++)
        {
            var member1 = x.Members[i];
            var member2 = y.Members[i];

            if (member1.Id != member2.Id || !member1.Schema.Equals(member2.Schema, ancestors))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(SchemaContents obj)
    {
        var hashCode = new HashCode();
        var ancestors = new Dictionary<Schema, int>(ReferenceEqualityComparer.Instance);

        foreach (var member in obj.Members)
        {
            hashCode.Add(member.Id);
            member.Schema.AppendHashCode(ancestors, ref hashCode);
        }

        return hashCode.ToHashCode();
    }
}
