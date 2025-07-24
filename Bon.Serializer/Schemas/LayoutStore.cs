using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of all the <see cref="Layout"/>s.
/// Initially populated by <see cref="StoreUpdater.InitializeStores"/>.
/// Later updated by <see cref="StoreUpdater.UpdateLayoutStore"/>.
/// </summary>
internal sealed class LayoutStore
{
    private readonly ConcurrentDictionary<int, Layout> _layoutById = [];

    private readonly Dictionary<Layout, int> _layouts = [];//1at layout equality comparer toevoegen

    public List<Schema> PartialSchemas { get; } = [];

    private List<Layout> _layoutsToSave = [];

    public void Add(Schema schema)
    {
        Trace.Assert(schema.LayoutId == 0);
        var layoutId = AddNow(schema);
        schema.LayoutId = layoutId;
    }

    private int AddNow(Schema schema)
    {
        if (_layouts.TryGetValue(new Layout(0, schema.Members), out var layoutId))
        {
            return layoutId;
        }

        var layout = new Layout(_layouts.Count + 1, schema.Members);
        _layoutsToSave.Add(layout);
        Add(layout);
        return layout.Id;
    }

    public bool ContainsLayoutId(int layoutId) => _layoutById.ContainsKey(layoutId);

    public void Add(Layout layout)//2at check ook race condities
    {
        if (_layoutById.ContainsKey(layout.Id))
        {
            return;
        }

        Trace.Assert(layout.Id == _layoutById.Count + 1);
        _layoutById[layout.Id] = layout;
        _layouts[layout] = layout.Id;

        FillPartialSchemas(layout);
    }

    private void FillPartialSchemas(Layout layout)
    {
        for (int i = 0; i < PartialSchemas.Count; i++)
        {
            if (PartialSchemas[i].LayoutId == layout.Id)
            {
                PartialSchemas[i].Members = layout.Members;
                PartialSchemas[i] = PartialSchemas[^1];
                i--;
            }
        }
    }

    public bool TryGet(int layoutId, [MaybeNullWhen(false)] out Layout layout) =>
        _layoutById.TryGetValue(layoutId, out layout);

    public Layout Get(int layoutId) => TryGet(layoutId, out var layout) ? layout :
        throw new InvalidOperationException($"No layout with ID {layoutId} found.");

    public IReadOnlyList<Layout> PopLayoutsToSave()
    {
        var result = _layoutsToSave;
        _layoutsToSave = null!;
        return result;
    }

    public int Count => _layoutById.Count;

    public void Clear()
    {
        _layouts.Clear();
        _layoutById.Clear();
        PartialSchemas.Clear();
        _layoutsToSave.Clear();
    }

    public void AppendHash(ref HashCode hashCode)
    {
        hashCode.AddMultiple(_layoutById
            .OrderBy(pair => pair.Key)
            .Select(pair => (pair.Key, SchemaContentsEqualityComparer.Instance.GetHashCode(pair.Value))));
    }
}
