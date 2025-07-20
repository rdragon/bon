namespace Bon.Serializer.Deserialization;

// See bookmark 791351735 for all places where an array is serialized/deserialized.
internal sealed class CollectionDeserializer(
    DeserializerStore deserializerStore) : IUseReflection
{
    /// <summary>
    /// Returns a method that reads binary data formatted according to the source schema and outputs a value of the target type.
    /// </summary>
    public Delegate? TryCreateDeserializer<T>(Schema sourceSchema)
    {
        if (sourceSchema is ArraySchema arraySchema && TryParseAsCollectionType(typeof(T)) is { } tuple)
        {
            return (Delegate)this.GetPrivateMethod(nameof(CreateCollectionReaderFor))
                .MakeGenericMethod(tuple.ElementType)
                .Invoke(this, [arraySchema, tuple.CollectionKind])!;
        }

        return null;
    }

    private static (Type ElementType, CollectionKind CollectionKind)? TryParseAsCollectionType(Type type)
    {
        if (type.IsArray && type.GetElementType() is Type elementType)
        {
            return (elementType, CollectionKind.Array);
        }

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var genericArguments = type.GetGenericArguments();
            elementType = genericArguments[0];

            if (genericTypeDefinition == typeof(IEnumerable<>) ||
                genericTypeDefinition == typeof(IReadOnlyList<>) ||
                genericTypeDefinition == typeof(IReadOnlyCollection<>))
            {
                return (elementType, CollectionKind.Array);
            }

            if (genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IList<>) ||
                genericTypeDefinition == typeof(List<>))
            {
                return (elementType, CollectionKind.List);
            }
        }

        return null;
    }

    private Delegate CreateCollectionReaderFor<TElement>(ArraySchema sourceSchema, CollectionKind collectionKind)
    {
        return collectionKind switch
        {
            CollectionKind.Array => CreateArrayReaderFor<TElement>(sourceSchema),
            CollectionKind.List => CreateListReaderFor<TElement>(sourceSchema),
        };
    }

    private Read<TElement[]?> CreateArrayReaderFor<TElement>(ArraySchema sourceSchema)
    {
        if (typeof(TElement) == typeof(byte) && sourceSchema.InnerSchema == NativeSchema.Byte)
        {
            return (Read<TElement[]?>)(object)CreateByteArrayReader();
        }

        var readElement = deserializerStore.GetDeserializer<TElement>(sourceSchema.InnerSchema);

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int count)
            {
                return null;
            }

            if (count == 0)
            {
                return [];
            }

            var array = new TElement[count];

            for (var i = 0; i < count; i++)
            {
                array[i] = readElement(input);
            }

            return array;
        };
    }

    private Read<List<T>?> CreateListReaderFor<T>(ArraySchema sourceSchema)
    {
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema.InnerSchema);

        return (BonInput input) =>
        {
            if (IntSerializer.Read(input.Reader) is not int count)
            {
                return null;
            }

            var list = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                list.Add(readElement(input));
            }

            return list;
        };
    }

    private static Read<byte[]?> CreateByteArrayReader() => input => ReadByteArray(input.Reader);

    public static byte[]? ReadByteArray(BinaryReader reader)
    {
        if (IntSerializer.Read(reader) is not int count)
        {
            return null;
        }

        return reader.ReadBytes(count);
    }

    public enum CollectionKind { Array, List }
}
