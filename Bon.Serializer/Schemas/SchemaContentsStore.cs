using System.Diagnostics.CodeAnalysis;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of all the <see cref="SchemaContents"/>.
/// Initially populated by <see cref="SchemaStoreUpdater.InitializeSchemaStore"/>.
/// Later updated by <see cref="SchemaStoreUpdater.UpdateSchemaStore"/>.
/// </summary>
internal sealed class SchemaContentsStore
{
    /// <summary>
    /// //2at
    /// </summary>
    private readonly ConcurrentDictionary<int, SchemaContents> _schemaContentsById = [];

    /// <summary>
    /// //2at
    /// </summary>
    private readonly ConcurrentDictionary<SchemaContents, int> _idBySchemaContents = new(SchemaContentsEqualityComparer.Instance);

    /// <summary>
    /// Returns the ID assigned to <paramref name="schemaContents"/> if it already exists in the store.
    /// Otherwise assigns a new ID and adds the contents to the store.
    /// This method is used by the source generation context.
    /// </summary>
    public int GetOrAddContentsId(SchemaContents schemaContents)
    {
        return _idBySchemaContents.GetOrAdd(schemaContents, static (contents, store) =>
        {
            var id = store.Count + 1; // Keep the offset equal to the one used at bookmark 964817627.
            store._schemaContentsById[id] = contents;

            return id;
        }, this);
    }

    public bool ContainsContentsId(int contentsId) => _schemaContentsById.ContainsKey(contentsId);

    /// <summary>
    /// Adds <paramref name="schemaContents"/> to the store.
    /// This method is used when schemas are loaded from the storage.
    /// </summary>
    public void Add(int contentsId, SchemaContents schemaContents)
    {
        _schemaContentsById[contentsId] = schemaContents;
        _idBySchemaContents[schemaContents] = contentsId;
    }

    public bool TryGet(int contentsId, [MaybeNullWhen(false)] out SchemaContents schemaContents) =>
        _schemaContentsById.TryGetValue(contentsId, out schemaContents);

    public SchemaContents Get(int contentsId) => TryGet(contentsId, out var contents) ? contents :
        throw new InvalidOperationException($"Schema contents with ID {contentsId} not found.");

    public IEnumerable<SchemaContentsData> GetNewestSchemas(int count)
    {
        var minId = Count - count + 1; // Keep the offset equal to the one used at bookmark 964817627.

        foreach (var (id, contents) in _schemaContentsById)
        {
            if (id >= minId)
            {
                var members = contents.Members.Select(member => new SchemaMemberData(member.Id, SchemaData.Create(member.Schema))).ToArray();

                yield return new SchemaContentsData(id, members);
            }
        }
    }

    public int Count => _schemaContentsById.Count;

    public void Clear()
    {
        _idBySchemaContents.Clear();
        _schemaContentsById.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        hashCode.AddMultiple(_schemaContentsById
            .OrderBy(pair => pair.Key)
            .Select(pair => (pair.Key, SchemaContentsEqualityComparer.Instance.GetHashCode(pair.Value))));
    }
}
