namespace Bon.Serializer.Schemas;

/// <summary>
/// Persistent storage for the layouts.
/// The layouts are loaded from and saved to the provided blob.
/// </summary>
/// <param name="blob">The blob to load the layouts from and save the layouts to.</param>
internal sealed class LayoutStorage(IBlob blob, LayoutStore layoutStore)
{
    /// <summary>
    /// The entity tag of the data that was loaded from the storage.
    /// </summary>
    private EntityTag _entityTag;

    /// <summary>
    /// The number of bytes that have been read from the blob.
    /// </summary>
    private long _bytesRead;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Appends the provided layouts to the blob if the entity tags match.
    /// Returns true if the layouts were appended to the blob.
    /// </summary>
    public async Task<bool> TrySaveAsync(IEnumerable<Layout> layouts)
    {
        var stream = WriteToStream(layouts);
        var maybe = await blob.TryAppendAsync(stream, _entityTag).ConfigureAwait(false);

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

    /// <summary>
    /// Loads the layouts from the blob and updates the entity tag and the number of bytes read.
    /// Every layout that is read is added to the layout store.
    /// Keeps track of the number of bytes read, so that the next time the reading
    /// can be resumed at the correct position.
    /// However, each time this method is called, the complete blob contents are downloaded again.
    /// </summary>
    public async Task LoadLayoutsAsync()
    {
        var stream = new MemoryStream();
        var entityTag = await blob.LoadToAsync(stream).ConfigureAwait(false);
        Trace.Assert(stream.Length >= _bytesRead, "Blob has decreased in size");
        stream.Position = _bytesRead;
        var reader = new BinaryReader(stream);
        var layoutReader = new LayoutReader(layoutStore, reader, true);
        layoutReader.ReadManyLayouts();
        _entityTag = entityTag;
        _bytesRead = stream.Length;
    }

    /// <summary>
    /// Obtains the entity tag of the blob and calls <see cref="LoadLayoutsAsync"/> if the entity tag has changed.
    /// A semaphore is used to prevent multiple threads from loading the layouts at the same time.
    /// </summary>
    public async Task LoadLatestLayoutsAsync()
    {
        var entityTag = await blob.GetEntityTagAsync().ConfigureAwait(false);

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

            await LoadLayoutsAsync().ConfigureAwait(false);
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
