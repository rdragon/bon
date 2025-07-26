namespace Bon.Serializer.Schemas;

//2at
internal class Structure
{
    private readonly int _hash;
    private readonly byte[] _bytes;

    public Structure(Schema schema)
    {
        _bytes = new StructureHelper<Schema>(x => x, ReferenceEqualityComparer.Instance).GetBytes(schema);
        _hash = ComputeHash(_bytes);
    }

    public Structure(Layout layout)
    {
        _bytes = new StructureHelper<int>(x => x.LayoutId).GetBytes(Convert(layout));
        _hash = ComputeHash(_bytes);
    }

    public override bool Equals(object? obj)
    {
        return obj is Structure other && other._hash == _hash && other._bytes.SequenceEqual(_bytes);
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    // Fowler–Noll–Vo hash function (or should we add the nuget package System.IO.Hashing?)
    private static int ComputeHash(byte[] bytes)
    {
        const uint fnvPrime = 16777619;
        uint hash = 2166136261;

        foreach (var b in bytes)
            hash = (hash ^ b) * fnvPrime;

        return (int)hash;
    }

    private static Schema Convert(Layout layout)
    {
        return Schema.Create(SchemaType.Record, null, layout.Id, layout.Members);
    }
}
