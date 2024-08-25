namespace Bon.Serializer.Schemas;

/// <summary>
/// Persistent storage for the schemas.
/// The schemas are saved to the provided blob.
/// </summary>
/// <param name="blob">The blob to save the schemas to.</param>
internal sealed class SchemaStorage(IBlob blob)
{
    /// <summary>
    /// Appends the provided block to the blob if the entity tags match.
    /// Returns the entity tag of the updated blob or null if the blob was not updated.
    /// </summary>
    public async Task<EntityTag?> TryAppend(Block block, EntityTag entityTag)
    {
        var stream = GetStream(block);

        return await blob.TryAppend(stream, entityTag).ConfigureAwait(false);
    }

    private static MemoryStream GetStream(Block block)
    {
        var stream = new MemoryStream();
        BlockSerializer.Serialize(stream, block);
        stream.Position = 0;

        return stream;
    }

    public async Task<SchemaStorageResponse> Load()
    {
        var stream = new MemoryStream();
        var entityTag = await blob.LoadTo(stream).ConfigureAwait(false);
        stream.Position = 0;

        var blocks = BlockSerializer.Deserialize(stream).ToArray();

        return new SchemaStorageResponse(blocks, entityTag);
    }

    /// <summary>
    /// Returns the entity tag of the blob.
    /// </summary>
    public async Task<EntityTag> GetEntityTag() => await blob.GetEntityTag().ConfigureAwait(false);

    public sealed record class SchemaStorageResponse(
        IReadOnlyList<Block> Blocks,
        EntityTag EntityTag);
}
