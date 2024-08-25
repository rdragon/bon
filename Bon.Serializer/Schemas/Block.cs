namespace Bon.Serializer.Schemas;

/// <summary>
/// A collection of schemas.
/// The schemas are saved in blocks to the storage.
/// </summary>
/// <param name="BlockId">
/// Each block has a unique ID.
/// During serialization the block ID is written to the stream.
/// During deserialization the block ID is used to determine whether the correct schemas are loaded.
/// </param>
/// <param name="Schemas">The schemas inside the block.</param>
internal readonly record struct Block(uint BlockId, IReadOnlyList<SchemaContentsData> Schemas);
