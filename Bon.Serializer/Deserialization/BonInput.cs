namespace Bon.Serializer.Deserialization;

public readonly struct BonInput
{
    public BinaryReader Reader { get; }

    internal BonInput(BinaryReader reader)
    {
        Reader = reader;
    }
}
