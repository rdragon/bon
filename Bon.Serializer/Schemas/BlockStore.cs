namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of all schema blocks that were loaded from the storage.
/// </summary>
internal sealed class BlockStore
{
    /// <summary>
    /// The IDs of all blocks that were loaded from the storage.
    /// </summary>
    private readonly ConcurrentDictionary<uint, EmptyStruct> _blockIds = [];

    /// <summary>
    /// The ID of the last block loaded from the storage.
    /// </summary>
    public uint LastBlockId { get; set; }

    /// <summary>
    /// The entity tag of the schema data that was loaded from the storage.
    /// </summary>
    public EntityTag EntityTag { get; set; }

    public void AddBlockId(uint blockId) => _blockIds.TryAdd(blockId, default);

    public bool ContainsBlockId(uint blockId) => _blockIds.ContainsKey(blockId);

    public void Clear()
    {
        _blockIds.Clear();
        LastBlockId = 0;
        EntityTag = default;
    }

    public void AppendHash(ref HashCode hashCode)
    {
        hashCode.Add(LastBlockId);
        hashCode.Add(EntityTag);
        hashCode.AddMultiple(_blockIds.Keys.Order());
    }

    private readonly struct EmptyStruct;
}
