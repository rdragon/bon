namespace Bon.Serializer.Schemas;

/// <summary>
/// Persistent storage for the layouts.
/// The layouts are saved to the provided blob.
/// </summary>
/// <param name="blob">The blob to save the layouts to.</param>
internal sealed class LayoutStorage(IBlob blob, LayoutStore layoutStore)
{
    /// <summary>
    /// The entity tag of the schema data that was loaded from the storage.
    /// </summary>
    private EntityTag _entityTag;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Appends the provided block to the blob if the entity tags match.//2at
    /// </summary>
    public async Task<bool> TrySave(IEnumerable<Layout> layouts)
    {
        var maybe = await blob.TryAppend(GetStream(layouts), _entityTag).ConfigureAwait(false);

        if (maybe is { } entityTag)
        {
            _entityTag = entityTag;
            return true;
        }

        return false;
    }

    private static MemoryStream GetStream(IEnumerable<Layout> layouts)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var layoutWriter = new LayoutWriter(writer);

        foreach (var layout in layouts)
        {
            layoutWriter.Write(layout);
        }

        stream.Position = 0;

        return stream;
    }

    public async Task Load()
    {
        var stream = new MemoryStream();
        var entityTag = await blob.LoadTo(stream).ConfigureAwait(false);
        stream.Position = 0;
        var reader = new BinaryReader(stream);
        var layoutReader = new LayoutReader(layoutStore, reader);

        while (stream.Position < stream.Length)
        {
            layoutReader.Read();
        }

        _entityTag = entityTag;
    }

    public async Task LoadIfNecessary()
    {
        var entityTag = await blob.GetEntityTag().ConfigureAwait(false);

        if (entityTag == _entityTag)
        {
            return;
        }

        await _semaphore.WaitAsync().ConfigureAwait(false);

        try
        {
            if (entityTag == _entityTag)
            {
                return;
            }

            await Load().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
