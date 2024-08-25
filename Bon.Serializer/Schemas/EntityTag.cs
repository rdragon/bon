namespace Bon.Serializer.Schemas;

/// <summary>
/// Also known as ETag.
/// The entity tag ensures (among other things) that no duplicate schemas are saved to the schema storage when
/// multiple clients are updating the schema storage at the same time.
/// You can think of the entity tag as a version number.
/// If the data changes, the entity tag changes as well.
/// </summary>
public readonly record struct EntityTag(string? Value);
