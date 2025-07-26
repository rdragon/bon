namespace Bon.Serializer;

/// <summary>
/// //2at
/// </summary>
public sealed class BonFacade
{
    private readonly DeserializerStore _deserializerStore;
    private readonly WriterStore _writerStore;

    internal BonFacade(
        DeserializerStore deserializerStore,
        WriterStore writerStore)
    {
        _deserializerStore = deserializerStore;
        _writerStore = writerStore;
    }

    public void AddWriter(Type type, Delegate writer) => _writerStore.Add(type, writer);

    public void AddWeakDeserializerFactory<T1, T2>(Func<T2, T1> func) => _deserializerStore.AddWeakDeserializerFactory(func);

    public void AddReaderFactory(Type type, Delegate factory) => _deserializerStore.AddReaderFactory(type, factory);

    public void AddMemberType(Type unionType, int memberId, Type memberType) =>
        _deserializerStore.MemberTypes[(unionType, memberId)] = memberType;

    public void AddDeserializer(Type type, Delegate deserializer) =>
        _deserializerStore.AddDeserializer(type, deserializer);
}
