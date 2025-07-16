namespace Bon.Serializer.Deserialization;

public delegate T Read<out T>(BonInput input);

public readonly struct BonInput
{
    public BinaryReader Reader { get; }

    internal BonInput(BinaryReader reader)
    {
        Reader = reader;
    }
}

internal readonly struct BonOutput(BinaryWriter writer, BonSerializerOptions? options)
{
    public BinaryWriter Writer { get; } = writer;
    public BonSerializerOptions? Options { get; } = options;
}
