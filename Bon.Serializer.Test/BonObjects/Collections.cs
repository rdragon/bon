namespace Bon.Serializer.Test.BonObjects;

[BonObject]
public sealed class WithArray(Dog[] values) : WithArrayBase<Dog>(values, values)
{
    [BonMember(1)]
    public Dog[] Values => _values!;
}

[BonObject]
public sealed class WithNullableArray(Dog[]? values) : WithArrayBase<Dog>(values, values)
{
    [BonMember(1)]
    public Dog[]? Values => _values;
}

[BonObject]
public sealed class WithByteArray(byte[] values) : WithArrayBase<byte>(values, values)
{
    [BonMember(1)]
    public byte[] Values => _values!;
}

[BonObject]
public sealed class WithNullableByteArray(byte[]? values) : WithArrayBase<byte>(values, values)
{
    [BonMember(1)]
    public byte[]? Values => _values;
}

[BonObject]
public sealed class WithList(List<Dog> values) : WithArrayBase<Dog>(values, values)
{
    [BonMember(1)]
    public List<Dog> Values => _values!.ToList();
}

[BonObject]
public sealed class WithIList(IList<Dog> values) : WithArrayBase<Dog>(values.AsReadOnly(), values)
{
    [BonMember(1)]
    public IList<Dog> Values => _values!.ToList();
}


[BonObject]
public sealed class WithIReadOnlyList(IReadOnlyList<Dog> values) : WithArrayBase<Dog>(values, values)
{
    [BonMember(1)]
    public IReadOnlyList<Dog> Values => _values!;
}

[BonObject]
public sealed class WithICollection(ICollection<Dog> values) : WithArrayBase<Dog>(values.ToArray(), values)
{
    [BonMember(1)]
    public ICollection<Dog> Values => _values!.ToList();
}

[BonObject]
public sealed class WithIReadOnlyCollection(IReadOnlyCollection<Dog> values) : WithArrayBase<Dog>(values.ToArray(), values)
{
    [BonMember(1)]
    public IReadOnlyCollection<Dog> Values => _values!;
}

[BonObject]
public sealed class WithIEnumerable(IEnumerable<Dog> values) : WithArrayBase<Dog>(values.ToArray(), values)
{
    [BonMember(1)]
    public IEnumerable<Dog> Values => _values!;
}

[BonObject]
public sealed class WithArrayOfNullable(Dog?[] values) : WithArrayBase<Dog?>(values, values)
{
    [BonMember(1)]
    public Dog?[] Values => _values!;
}

[BonObject]
public sealed class WithArrayOfEmptyClass(EmptyClass[] values) : WithArrayBase<EmptyClass>(values, values)
{
    [BonMember(1)]
    public EmptyClass[] Values => _values!;
}

[BonObject]
public sealed class WithListOfEmptyClass(List<EmptyClass> values) : WithArrayBase<EmptyClass>(values, values)
{
    [BonMember(1)]
    public List<EmptyClass> Values => _values!.ToList();
}

[BonObject]
public sealed class WithNullableArrayOfEmptyClass(EmptyClass[]? values) : WithArrayBase<EmptyClass>(values, values)
{
    [BonMember(1)]
    public EmptyClass[]? Values => _values;
}

[BonObject]
public sealed class WithNullableListOfEmptyClass(List<EmptyClass>? values) : WithArrayBase<EmptyClass>(values, values)
{
    [BonMember(1)]
    public List<EmptyClass>? Values => _values?.ToList();
}

[BonObject]
public sealed class WithDictionary(Dictionary<int, Dog> dictionary) : WithArrayBase<KeyValuePair<int, Dog>>(dictionary.ToArray(), dictionary)
{
    [BonMember(1)]
    public Dictionary<int, Dog> Dictionary => _values!.ToDictionary(pair => pair.Key, pair => pair.Value);
}

[BonObject]
public sealed class WithIDictionary(IDictionary<int, Dog> dictionary) : WithArrayBase<KeyValuePair<int, Dog>>(dictionary.ToArray(), dictionary)
{
    [BonMember(1)]
    public IDictionary<int, Dog> Dictionary => _values!.ToDictionary(pair => pair.Key, pair => pair.Value);
}

[BonObject]
public sealed class WithIReadOnlyDictionary(IReadOnlyDictionary<int, Dog> dictionary) : WithArrayBase<KeyValuePair<int, Dog>>(dictionary.ToArray(), dictionary)
{
    [BonMember(1)]
    public IReadOnlyDictionary<int, Dog> Dictionary => _values!.ToDictionary(pair => pair.Key, pair => pair.Value);
}

[BonObject]
public sealed class WithDictionaryOfCats(Dictionary<string, Cat> dictionary) : WithArrayBase<KeyValuePair<string, Cat>>(dictionary.ToArray(), dictionary)
{
    [BonMember(1)]
    public Dictionary<string, Cat> Dictionary => _values!.ToDictionary(pair => pair.Key, pair => pair.Value);
}

[BonObject]
public sealed class WithNullableDictionary(Dictionary<int, Dog>? dictionary) : WithArrayBase<KeyValuePair<int, Dog>>(dictionary?.ToArray(), dictionary)
{
    [BonMember(1)]
    public Dictionary<int, Dog>? Dictionary => _values?.ToDictionary(pair => pair.Key, pair => pair.Value);
}

[BonObject]
public sealed class WithNullableDictionaryOfCats(Dictionary<string, Cat>? dictionary) : WithArrayBase<KeyValuePair<string, Cat>>(dictionary?.ToArray(), dictionary)
{
    [BonMember(1)]
    public Dictionary<string, Cat>? Dictionary => _values!.ToDictionary(pair => pair.Key, pair => pair.Value);
}
