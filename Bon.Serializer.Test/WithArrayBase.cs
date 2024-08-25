namespace Bon.Serializer.Test;

public class WithArrayBase<T>(IReadOnlyList<T>? values, object? rawDeserializedCollection)
{
    protected T[]? _values = values?.ToArray();

    public override bool Equals(object? other) => other is WithArrayBase<T> obj && TestHelper.SequenceEqual(_values, obj._values);

    public override int GetHashCode() => _values?.Length ?? -1;

    public override string ToString() => _values is { } ? $"{{ Values = [{string.Join(", ", _values)}] }}" : "{ Values = }";

    public T this[int index] => _values is { } ? _values[index] : throw new InvalidOperationException();

    public object? RawDeserializedCollection => rawDeserializedCollection;
}
