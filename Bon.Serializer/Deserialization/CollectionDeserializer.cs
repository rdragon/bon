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
        if (TryParseAsCollectionType(typeof(T)) is { } tuple)
        {
            return (Delegate)this.GetPrivateMethod(nameof(CreateCollectionDeserializerFor))
                .MakeGenericMethod(tuple.ElementType)
                .Invoke(this, [sourceSchema, tuple.CollectionKind])!;
        }

        if (sourceSchema is ArraySchema)
        {
            return CreateCollectionToElementReader<T>(sourceSchema);
        }

        return null;
    }

    private Delegate CreateCollectionDeserializerFor<T>(Schema sourceSchema, CollectionKind collectionKind)
    {
        if (sourceSchema is ArraySchema sourceArraySchema)
        {
            return CreateCollectionReaderFor<T>(sourceArraySchema, collectionKind);
        }

        return CreateElementToCollectionReaderFor<T>(sourceSchema, collectionKind);
    }

    private Read<T?> CreateCollectionToElementReader<T>(Schema sourceSchema)
    {
        var readArray = deserializerStore.GetDeserializer<T[]>(sourceSchema);

        return (BonInput input) =>
        {
            var array = readArray(input);

            return array.Length > 0 ? array[0] : default;
        };
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

    private Delegate CreateCollectionReaderFor<T>(ArraySchema sourceSchema, CollectionKind collectionKind)
    {
        return collectionKind switch
        {
            CollectionKind.Array => CreateArrayReaderFor<T>(sourceSchema),
            CollectionKind.List => CreateListReaderFor<T>(sourceSchema),
        };
    }

    private Read<T[]?> CreateArrayReaderFor<T>(ArraySchema sourceSchema)
    {
        if (typeof(T) == typeof(byte) && sourceSchema.InnerSchema == NativeSchema.Byte)
        {
            return (Read<T[]?>)(object)ReadByteArray;
        }

        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema.InnerSchema);

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

            var array = new T[count];

            for (var i = 0; i < count; i++)
            {
                array[i] = readElement(input);
            }

            return array;
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

    private Delegate CreateElementToCollectionReaderFor<T>(Schema sourceSchema, CollectionKind collectionKind)
    {
        return collectionKind switch
        {
            CollectionKind.Array => CreateElementToArrayReaderFor<T>(sourceSchema),
            CollectionKind.List => CreateElementToListReaderFor<T>(sourceSchema),
        };
    }

    private Read<T[]?> CreateElementToArrayReaderFor<T>(Schema sourceSchema)
    {
        // Bookmark 943797192
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema);

        return (BonInput input) =>
        {
            var element = readElement(input);

            if (element is null)
            {
                return null;
            }

            return [element];
        };
    }

    private Read<List<T>?> CreateElementToListReaderFor<T>(Schema sourceSchema)
    {
        // Bookmark 943797192
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema);

        return (BonInput input) =>
        {
            var element = readElement(input);

            if (element is null)
            {
                return null;
            }

            return [element];
        };
    }

    public enum CollectionKind { Array, List }

}
