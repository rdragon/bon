namespace Bon.Serializer.Deserialization;

// See bookmark 791351735 for all places where an array is serialized/deserialized.
internal sealed class CollectionDeserializer(
    DeserializerStore deserializerStore) : IUseReflection
{
    public Delegate CreateDeserializer<T>(Schema sourceSchema, Schema targetSchema)
    {
        if (targetSchema is ArraySchema)
        {
            var (elementType, collectionKind) = GetElementType(typeof(T));

            return (Delegate)this.GetPrivateMethod(nameof(CreateCollectionDeserializerFor))
                .MakeGenericMethod(elementType)
                .Invoke(this, [sourceSchema, targetSchema, collectionKind])!;
        }

        return CreateCollectionToElementReader<T>(sourceSchema);
    }

    private Delegate CreateCollectionDeserializerFor<T>(Schema sourceSchema, ArraySchema targetSchema, CollectionKind collectionKind)
    {
        if (sourceSchema is ArraySchema sourceArraySchema)
        {
            return CreateCollectionReaderFor<T>(sourceArraySchema, targetSchema, collectionKind);
        }

        return CreateElementToCollectionReaderFor<T>(sourceSchema, targetSchema, collectionKind);
    }

    private Read<T?> CreateCollectionToElementReader<T>(Schema sourceSchema)
    {
        var readArray = deserializerStore.GetDeserializer<T[]>(sourceSchema, false);

        return (BonInput input) =>
        {
            var array = readArray(input);

            return array.Length > 0 ? array[0] : default;
        };
    }

    private static (Type ElementType, CollectionKind CollectionKind) GetElementType(Type collectionType)
    {
        if (collectionType.IsArray && collectionType.GetElementType() is Type elementType)
        {
            return (elementType, CollectionKind.Array);
        }

        if (collectionType.IsGenericType)
        {
            var genericTypeDefinition = collectionType.GetGenericTypeDefinition();
            var genericArguments = collectionType.GetGenericArguments();
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

        throw new ArgumentOutOfRangeException(nameof(collectionType), collectionType, null);
    }

    private Delegate CreateCollectionReaderFor<T>(ArraySchema sourceSchema, ArraySchema targetSchema, CollectionKind collectionKind)
    {
        return collectionKind switch
        {
            CollectionKind.Array => CreateArrayReaderFor<T>(sourceSchema, targetSchema),
            CollectionKind.List => CreateListReaderFor<T>(sourceSchema, targetSchema),
            _ => throw new ArgumentOutOfRangeException(nameof(collectionKind), collectionKind, null),
        };
    }

    private Read<T[]?> CreateArrayReaderFor<T>(ArraySchema sourceSchema, ArraySchema targetSchema)
    {
        var targetIsNullable = targetSchema.IsNullable;

        if (typeof(T) == typeof(byte) && sourceSchema.InnerSchema == NativeSchema.Byte)
        {
            return (Read<T[]?>)(object)CreateByteArrayReader(targetIsNullable);
        }

        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema.InnerSchema, targetSchema.InnerSchema.IsNullable);

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
            {
                return targetIsNullable ? null : [];
            }

            var array = new T[count];

            for (var i = 0; i < count; i++)
            {
                array[i] = readElement(input);
            }

            return array;
        };
    }

    private static Read<byte[]?> CreateByteArrayReader(bool targetIsNullable)
    {
        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
            {
                return targetIsNullable ? null : [];
            }

            return input.Reader.ReadBytes(count);
        };
    }

    private Read<List<T>?> CreateListReaderFor<T>(ArraySchema sourceSchema, ArraySchema targetSchema)
    {
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema.InnerSchema, typeof(T).IsNullable(false));
        var targetIsNullable = targetSchema.IsNullable;

        return (BonInput input) =>
        {
            if ((int?)WholeNumberSerializer.ReadNullable(input.Reader) is not int count)
            {
                return targetIsNullable ? null : [];
            }

            var list = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                list.Add(readElement(input));
            }

            return list;
        };
    }

    private Delegate CreateElementToCollectionReaderFor<T>(Schema sourceSchema, ArraySchema targetSchema, CollectionKind collectionKind)
    {
        return collectionKind switch
        {
            CollectionKind.Array => CreateElementToArrayReaderFor<T>(sourceSchema, targetSchema),
            CollectionKind.List => CreateElementToListReaderFor<T>(sourceSchema, targetSchema),
            _ => throw new ArgumentOutOfRangeException(nameof(collectionKind), collectionKind, null),
        };
    }

    private Read<T[]?> CreateElementToArrayReaderFor<T>(Schema sourceSchema, ArraySchema targetSchema)
    {
        // Bookmark 943797192
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema, typeof(T).IsValueType ? null : true);
        var targetIsNullable = targetSchema.IsNullable;

        return (BonInput input) =>
        {
            var element = readElement(input);

            if (element is null)
            {
                return targetIsNullable ? null : [];
            }

            return [element];
        };
    }

    private Read<List<T>?> CreateElementToListReaderFor<T>(Schema sourceSchema, ArraySchema targetSchema)
    {
        // Bookmark 943797192
        var readElement = deserializerStore.GetDeserializer<T>(sourceSchema, typeof(T).IsValueType ? null : true);
        var targetIsNullable = targetSchema.IsNullable;

        return (BonInput input) =>
        {
            var element = readElement(input);

            if (element is null)
            {
                return targetIsNullable ? null : [];
            }

            return [element];
        };
    }

    public enum CollectionKind { Array, List }

}
