namespace Bon.Serializer.Serialization;

internal sealed class WriterStore : IUseReflection
{
    private readonly ConcurrentDictionary<Type, Delegate> _writers = new();
    private Func<Type, Delegate>? _createWriter;

    public void AddBuiltInWriters()
    {
        AddNativeWriters();
        AddWeakWriters();
    }

    private void AddNativeWriters()
    {
        Add<string>(NativeSerializer.WriteString);
        Add<bool>(NativeSerializer.WriteBool);
        Add<byte>(NativeSerializer.WriteByte);
        Add<sbyte>(NativeSerializer.WriteSByte);
        Add<short>(NativeSerializer.WriteShort);
        Add<ushort>(NativeSerializer.WriteUShort);
        Add<int>(NativeSerializer.WriteInt);
        Add<uint>(NativeSerializer.WriteUInt);
        Add<long>(NativeSerializer.WriteLong);
        Add<ulong>(NativeSerializer.WriteULong);
        Add<float>(NativeSerializer.WriteFloat);
        Add<double>(NativeSerializer.WriteDouble);
        Add<decimal>(NativeSerializer.WriteDecimal);
        Add<Guid>(NativeSerializer.WriteGuid);

        Add<bool?>(NativeSerializer.WriteNullableBool);
        Add<byte?>(NativeSerializer.WriteNullableByte);
        Add<sbyte?>(NativeSerializer.WriteNullableSByte);
        Add<short?>(NativeSerializer.WriteNullableShort);
        Add<ushort?>(NativeSerializer.WriteNullableUShort);
        Add<int?>(NativeSerializer.WriteNullableInt);
        Add<uint?>(NativeSerializer.WriteNullableUInt);
        Add<long?>(NativeSerializer.WriteNullableLong);
        Add<ulong?>(NativeSerializer.WriteNullableULong);
        Add<float?>(NativeSerializer.WriteNullableFloat);
        Add<double?>(NativeSerializer.WriteNullableDouble);
        Add<decimal?>(NativeSerializer.WriteNullableDecimal);
        Add<Guid?>(NativeSerializer.WriteNullableGuid);
    }

    private void AddWeakWriters()
    {
        // Bookmark 659516266 (char serialization)
        Add<char>(NativeSerializer.WriteChar);
        Add<char?>(NativeSerializer.WriteNullableChar);
        Add<DateTime>(NativeSerializer.WriteDateTime);
        Add<DateTime?>(NativeSerializer.WriteNullableDateTime);
        Add<DateTimeOffset>(NativeSerializer.WriteDateTimeOffset);
        Add<DateTimeOffset?>(NativeSerializer.WriteNullableDateTimeOffset);
        Add<TimeSpan>(NativeSerializer.WriteTimeSpan);
        Add<TimeSpan?>(NativeSerializer.WriteNullableTimeSpan);
        Add<DateOnly>(NativeSerializer.WriteDateOnly);
        Add<DateOnly?>(NativeSerializer.WriteNullableDateOnly);
        Add<TimeOnly>(NativeSerializer.WriteTimeOnly);
        Add<TimeOnly?>(NativeSerializer.WriteNullableTimeOnly);
    }

    public void Add<T>(Action<BinaryWriter, T> writer)
    {
        _writers[typeof(T)] = writer;
    }

    /// <summary>
    /// Returns a serializer that writes a value of type <typeparamref name="T"/> using the schema corresponding to
    /// <typeparamref name="T"/> if <typeparamref name="T"/> is a value type.
    /// If <typeparamref name="T"/> is a reference type, the value is serialized using the non-nullable version of the
    /// schema corresponding to <typeparamref name="T"/>.
    /// </summary>
    public Action<BinaryWriter, T> GetWriter<T>()
    {
        _createWriter ??= CreateWriter;

        return (Action<BinaryWriter, T>)_writers.GetOrAdd(typeof(T), _createWriter);
    }

    private Delegate CreateWriter(Type type)
    {
        if (type.IsArray && type.GetElementType() is Type elementType)
        {
            if (elementType == typeof(byte))
            {
                return CreateByteArrayWriter();
            }

            return CreateArrayWriter(elementType);
        }

        if (type.TryGetInnerTypesOfTuple2() is (Type item1Type, Type item2Type))
        {
            return CreateTuple2Writer(item1Type, item2Type, type.IsNullable(false));
        }

        if (type.TryGetInnerTypesOfTuple3() is (Type item1, Type item2, Type item3))
        {
            return CreateTuple3Writer(item1, item2, item3, type.IsNullable(false));
        }

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var genericArguments = type.GetGenericArguments();

            if (genericTypeDefinition == typeof(IList<>) ||
                genericTypeDefinition == typeof(IEnumerable<>) ||
                genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IReadOnlyList<>) ||
                genericTypeDefinition == typeof(IReadOnlyCollection<>) ||
                genericTypeDefinition == typeof(List<>))
            {
                elementType = genericArguments[0];

                return CreateArrayWriter(elementType);
            }

            if (genericTypeDefinition == typeof(Dictionary<,>) ||
                genericTypeDefinition == typeof(IDictionary<,>) ||
                genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
            {
                var keyType = genericArguments[0];
                var valueType = genericArguments[1];

                return CreateDictionaryWriter(keyType, valueType, genericTypeDefinition);
            }
        }

        throw new SchemaException($"No schema for type '{type}' found. Perhaps this type is missing a [BonObject] attribute?");
    }

    private Delegate CreateArrayWriter(Type elementType)
    {
        return (Delegate)this.GetPrivateMethod(nameof(CreateArrayWriterFor))
            .MakeGenericMethod(elementType)
            .Invoke(this, null)!;
    }

    private Delegate CreateArrayWriterFor<TElement>()
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        var writeElement = GetWriter<TElement>();

        return (BinaryWriter writer, IEnumerable<TElement> collection) =>
        {
            var elements = collection as IReadOnlyList<TElement> ?? collection.ToArray();
            var count = elements.Count;
            WholeNumberSerializer.Write(writer, count);

            for (int i = 0; i < count; i++)
            {
                writeElement(writer, elements[i]);
            }
        };
    }

    private static Delegate CreateByteArrayWriter()
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        return (BinaryWriter writer, byte[] array) =>
        {
            var count = array.Length;
            WholeNumberSerializer.Write(writer, count);
            writer.Write(array);
        };
    }

    private Delegate CreateDictionaryWriter(Type keyType, Type valueType, Type genericTypeDefinition)
    {
        if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
        {
            return (Delegate)this.GetPrivateMethod(nameof(CreateReadOnlyDictionaryWriterFor))
                .MakeGenericMethod(keyType, valueType)
                .Invoke(this, null)!;
        }
        else
        {
            return (Delegate)this.GetPrivateMethod(nameof(CreateDictionaryWriterFor))
                .MakeGenericMethod(keyType, valueType)
                .Invoke(this, null)!;
        }
    }

    private Delegate CreateDictionaryWriterFor<TKey, TValue>()
    {
        // Almost identical to the method below.

        var writeKey = GetWriter<TKey>();
        var writeValue = GetWriter<TValue>();

        return (BinaryWriter writer, IDictionary<TKey, TValue> dictionary) =>
        {
            var count = dictionary.Count;
            WholeNumberSerializer.Write(writer, count);
            var actualCount = 0;

            foreach (var (key, value) in dictionary)
            {
                writeKey(writer, key);
                writeValue(writer, value);
                actualCount++;
            }

            if (actualCount != count)
            {
                throw new InvalidOperationException("Dictionary was modified.");
            }
        };
    }

    private Delegate CreateReadOnlyDictionaryWriterFor<TKey, TValue>()
    {
        // Almost identical to the method above.

        var writeKey = GetWriter<TKey>();
        var writeValue = GetWriter<TValue>();

        return (BinaryWriter writer, IReadOnlyDictionary<TKey, TValue> dictionary) =>
        {
            var count = dictionary.Count;
            WholeNumberSerializer.Write(writer, count);
            var actualCount = 0;

            foreach (var (key, value) in dictionary)
            {
                writeKey(writer, key);
                writeValue(writer, value);
                actualCount++;
            }

            if (actualCount != count)
            {
                throw new InvalidOperationException("Dictionary was modified.");
            }
        };
    }

    private Delegate CreateTuple2Writer(Type item1Type, Type item2Type, bool isNullable)
    {
        return (Delegate)this.GetPrivateMethod(nameof(CreateTuple2WriterFor))
            .MakeGenericMethod(item1Type, item2Type)
            .Invoke(this, [isNullable])!;
    }

    private Delegate CreateTuple2WriterFor<T1, T2>(bool isNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var writeItem1 = GetWriter<T1>();
        var writeItem2 = GetWriter<T2>();

        if (isNullable)
        {
            return (BinaryWriter writer, (T1, T2)? tuple) =>
            {
                if (tuple is null)
                {
                    writer.Write(NativeSerializer.NULL);

                    return;
                }

                writer.Write(NativeSerializer.NOT_NULL);
                writeItem1(writer, tuple.Value.Item1);
                writeItem2(writer, tuple.Value.Item2);
            };
        }

        return (BinaryWriter writer, (T1, T2) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
        };
    }

    private Delegate CreateTuple3Writer(Type item1Type, Type item2Type, Type item3Type, bool isNullable)
    {
        return (Delegate)this.GetPrivateMethod(nameof(CreateTuple3WriterFor))
            .MakeGenericMethod(item1Type, item2Type, item3Type)
            .Invoke(this, [isNullable])!;
    }

    private Delegate CreateTuple3WriterFor<T1, T2, T3>(bool isNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var writeItem1 = GetWriter<T1>();
        var writeItem2 = GetWriter<T2>();
        var writeItem3 = GetWriter<T3>();

        if (isNullable)
        {
            return (BinaryWriter writer, (T1, T2, T3)? tuple) =>
            {
                if (tuple is null)
                {
                    writer.Write(NativeSerializer.NULL);

                    return;
                }

                writer.Write(NativeSerializer.NOT_NULL);
                writeItem1(writer, tuple.Value.Item1);
                writeItem2(writer, tuple.Value.Item2);
                writeItem3(writer, tuple.Value.Item3);
            };
        }

        return (BinaryWriter writer, (T1, T2, T3) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
            writeItem3(writer, tuple.Item3);
        };
    }
}
