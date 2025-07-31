using System.Diagnostics.CodeAnalysis;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of all known <see cref="Layout"/> instances.
/// </summary>
internal sealed class LayoutStore
{
    /// <summary>
    /// The layouts that are known.
    /// At index <c>i</c> the layout with ID <c>i + 1</c> is stored.
    /// Null values are stored at indices larger than or equal to <see cref="_count"/>.
    /// </summary>
    private Layout[] _layouts = [];

    /// <summary>
    /// The number of layouts that are known.
    /// </summary>
    private int _count;

    /// <summary>
    /// Creates a new layout based on the provided members and adds it to the store.
    /// Called by the source generation context.
    /// </summary>
    public Layout CreateLayout(IReadOnlyList<SchemaMember> members)
    {
        return AddLayout(new Layout(_count + 1, members));
    }

    /// <summary>
    /// Adds the provided layout to the store.
    /// Called when a new layout was read from the storage.
    /// </summary>
    public Layout AddLayout(Layout layout)
    {
        Trace.Assert(layout.Id == _count + 1, "Unexpected layout ID");
        ResizeArray();
        _layouts[layout.Id - 1] = layout;
        _count++;
        return layout;
    }

    private void ResizeArray()
    {
        if (_count == _layouts.Length)
        {
            var layouts = new Layout[Math.Max(16, _count * 2)];
            Array.Copy(_layouts, layouts, _count);
            _layouts = layouts;
        }
    }

    public bool TryGetLayout(int layoutId, [MaybeNullWhen(false)] out Layout layout)
    {
        var result = layoutId - 1 < _count;
        layout = result ? _layouts[layoutId - 1] : default;
        return result;
    }

    public Layout GetLayout(int layoutId) => TryGetLayout(layoutId, out var layout) ? layout :
        throw new InvalidOperationException($"No layout with ID {layoutId} found.");

    public IEnumerable<Layout> Layouts => _layouts.Take(_count);

    public int Count => _count;

    public void Clear()
    {
        _count = 0;
    }
}
