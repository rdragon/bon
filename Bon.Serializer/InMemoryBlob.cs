
namespace Bon.Serializer;

public class InMemoryBlob : IBlob
{
    protected readonly MemoryStream _stream = new();

    public virtual Task<EntityTag?> TryAppendAsync(Stream stream, EntityTag entityTag)
    {
        EntityTag? result = null;

        if (entityTag == GetEntityTagNow())
        {
            _stream.Position = _stream.Length;
            stream.CopyTo(_stream);
            result = GetEntityTagNow();
        }

        return Task.FromResult(result);
    }

    public Task<EntityTag> LoadToAsync(Stream stream)
    {
        _stream.Position = 0;
        _stream.CopyTo(stream);

        return GetEntityTagAsync();
    }

    public Task<EntityTag> GetEntityTagAsync() => Task.FromResult(GetEntityTagNow());

    public EntityTag GetEntityTagNow() => new(_stream.Length.ToString());
}
