namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a single blob of binary data.
/// This blob is used to store the schemas.
/// </summary>
public interface IBlob
{
    /// <summary>
    /// Appends the data from the provided stream to the blob if the entity tags match.
    /// Returns the entity tag of the updated blob or null if the blob was not updated.
    /// </summary>
    /// <param name="stream">
    /// The stream that contains the data to append.
    /// </param>
    /// <param name="entityTag">
    /// The blob will only be updated if the entity tag of the blob matches this entity tag.
    /// </param>
    Task<EntityTag?> TryAppend(Stream stream, EntityTag entityTag);

    /// <summary>
    /// Loads the blob into the provided stream.
    /// Returns the entity tag of the blob.
    /// </summary>
    Task<EntityTag> LoadTo(Stream stream);

    /// <summary>
    /// Returns the entity tag of the blob.
    /// </summary>
    Task<EntityTag> GetEntityTag();
}
