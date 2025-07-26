using System.Diagnostics.CodeAnalysis;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Keeps track of all known <see cref="Layout"/> instances.
/// </summary>
internal sealed class LayoutStore
{
    //2at
    private Layout[] _layouts = [];

    private int _count;

    // wordt enkel gebruikt tijdens startup, source generated context
    public Layout CreateLayout(IReadOnlyList<SchemaMember> members)
    {
        return AddLayout(new Layout(_count + 1, members));
    }

    public bool ContainsLayoutId(int layoutId) => layoutId - 1 < _count;

    // wordt gebruikt bij het laden uit storage
    // voegt layouts toe die mogelijk nog half zijn
    public Layout AddLayout(Layout layout)//2at check ook race condities
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

    public void Clear()
    {
        _count = 0;
    }
}
