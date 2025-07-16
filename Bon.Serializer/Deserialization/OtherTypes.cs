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
