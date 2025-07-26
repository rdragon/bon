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

    private long _bytesRead;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Appends the provided  to the blob if the entity tags match.//2at
    /// </summary>
    public async Task<bool> TrySave(IEnumerable<Layout> layouts)
    {
        var stream = WriteToStream(layouts);
        var maybe = await blob.TryAppend(stream, _entityTag).ConfigureAwait(false);

        if (maybe is { } entityTag)
        {
            _entityTag = entityTag;
            _bytesRead += stream.Length;
            return true;
        }

        return false;
    }

    private static MemoryStream WriteToStream(IEnumerable<Layout> layouts)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var layoutWriter = new LayoutWriter(writer);
        var timestamp = (ulong?)DateTimeOffset.UtcNow.Ticks;

        foreach (var layout in layouts)
        {
            WholeNumberSerializer.Write(writer, timestamp);
            timestamp = null;
            layoutWriter.Write(layout);
        }

        stream.Position = 0;

        return stream;
    }

    public async Task LoadLayouts()
    {
        var stream = new MemoryStream();
        var entityTag = await blob.LoadTo(stream).ConfigureAwait(false);
        Trace.Assert(stream.Length >= _bytesRead, "Blob has decreased in size");
        stream.Position = _bytesRead;
        var reader = new BinaryReader(stream);
        var layoutReader = new LayoutReader(layoutStore, reader, true);
        layoutReader.ReadManyLayouts();
        _entityTag = entityTag;
        _bytesRead = stream.Length;
    }

    public async Task LoadLatestLayouts()
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

            await LoadLayouts().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Clear()
    {
        _entityTag = default;
        _bytesRead = 0;
    }
}
