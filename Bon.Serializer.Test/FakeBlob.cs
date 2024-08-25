namespace Bon.Serializer.Test;

public sealed class FakeBlob : InMemoryBlob
{
    public bool FailFirstSave { get; set; }

    public byte[] Bytes
    {
        get => _stream.ToArray();

        set
        {
            _stream.SetLength(0);
            _stream.Write(value);
        }
    }

    public override Task<EntityTag?> TryAppend(Stream stream, EntityTag entityTag)
    {
        if (FailFirstSave)
        {
            FailFirstSave = false;

            return Task.FromResult<EntityTag?>(null);
        }

        return base.TryAppend(stream, entityTag);
    }
}
