namespace Bon.Serializer.Serialization;

internal sealed class WriterStore : IUseReflection
{
    /// <summary>
    /// Contains for every type that can be serialized a writer.
    /// There are a couple of ways in which this dictionary is filled:
    /// 1. by the method <see cref="AddNativeWriters"/>,
    /// 2. by the source generation context, see WriterGenerator,
    /// 3. by the method <see cref="CreateWriter"/>.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Writer> _writers = new();

    private Func<Type, Writer>? _createWriter;

    public void AddNativeWriters()
    {
        // Bookmark 659516266 (native serialization)
        // All non-generic types for which the source generation context does not provide a writer should be added here.
        // This is the same set of types as can be found at bookmark 293228595.
        Add<string>(NativeSerializer.WriteString);
        Add<bool>(NativeSerializer.WriteBool);
        Add<byte>(NativeSerializer.WriteByte, SimpleWriterType.Byte);
        Add<sbyte>(NativeSerializer.WriteSByte, SimpleWriterType.SByte);
        Add<short>(NativeSerializer.WriteShort, SimpleWriterType.Short);
        Add<ushort>(NativeSerializer.WriteUShort, SimpleWriterType.UShort);
        Add<int>(NativeSerializer.WriteInt, SimpleWriterType.Int);
        Add<uint>(NativeSerializer.WriteUInt, SimpleWriterType.UInt);
        Add<long>(NativeSerializer.WriteLong, SimpleWriterType.Long);
        Add<ulong>(NativeSerializer.WriteULong, SimpleWriterType.ULong);
        Add<float>(NativeSerializer.WriteFloat);
        Add<double>(NativeSerializer.WriteDouble);
        Add<decimal>(NativeSerializer.WriteDecimal);
        Add<Guid>(NativeSerializer.WriteGuid);
        Add<char>(NativeSerializer.WriteChar);
        Add<DateTime>(NativeSerializer.WriteDateTime);
        Add<DateTimeOffset>(NativeSerializer.WriteDateTimeOffset);
        Add<TimeSpan>(NativeSerializer.WriteTimeSpan);
        Add<DateOnly>(NativeSerializer.WriteDateOnly);
        Add<TimeOnly>(NativeSerializer.WriteTimeOnly);

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
        Add<char?>(NativeSerializer.WriteNullableChar);
        Add<DateTime?>(NativeSerializer.WriteNullableDateTime);
        Add<DateTimeOffset?>(NativeSerializer.WriteNullableDateTimeOffset);
        Add<TimeSpan?>(NativeSerializer.WriteNullableTimeSpan);
        Add<DateOnly?>(NativeSerializer.WriteNullableDateOnly);
        Add<TimeOnly?>(NativeSerializer.WriteNullableTimeOnly);
    }

    public void Add<T>(Action<BinaryWriter, T> writer, SimpleWriterType simpleWriterType = SimpleWriterType.None)
    {
        _writers[typeof(T)] = new(writer, simpleWriterType);
    }

    public void Add(Type type, Delegate writer)
    {
        _writers[type] = new(writer);
    }

    /// <summary>
    /// Returns a serializer that writes a value of type <typeparamref name="T"/> using the schema corresponding to
    /// <typeparamref name="T"/> if <typeparamref name="T"/> is a value type.
    /// If <typeparamref name="T"/> is a reference type, the value is serialized using the non-nullable version of the
    /// schema corresponding to <typeparamref name="T"/>.
    /// </summary>
    public Writer<T> GetWriter<T>()
    {
        _createWriter ??= CreateWriter;

        var writer = _writers.GetOrAdd(typeof(T), _createWriter);

        return writer.Convert<T>();
    }

    /// <summary>
    /// Creates a writer for an array, tuple, list or dictionary.
    /// </summary>
    private Writer CreateWriter(Type type)
    {
        if (type.IsArray && type.GetElementType() is Type elementType)
        {
            if (elementType == typeof(byte))
            {
                return CreateByteArrayWriter();
            }

            return CreateArrayWriter(elementType);
        }

        if (type.TryGetTuple2Type() is { } tuple2)
        {
            return CreateTuple2Writer(tuple2);
        }

        if (type.TryGetTuple3Type() is { } tuple3)
        {
            return CreateTuple3Writer(tuple3);
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

    private Writer CreateArrayWriter(Type elementType)
    {
        return (Writer)this.GetPrivateMethod(nameof(CreateArrayWriterFor))
            .MakeGenericMethod(elementType)
            .Invoke(this, null)!;
    }

    private Writer CreateArrayWriterFor<TElement>()
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        var (writeElement, _) = GetWriter<TElement>();

        return new((BinaryWriter writer, IEnumerable<TElement>? collection) =>
        {
            if (collection is null)
            {
                IntSerializer.WriteNull(writer);
                return;
            }

            var elements = collection as IReadOnlyList<TElement> ?? collection.ToArray();
            var count = elements.Count;
            IntSerializer.Write(writer, count);

            for (int i = 0; i < count; i++)
            {
                writeElement(writer, elements[i]);
            }
        });
    }

    private static Writer CreateByteArrayWriter()
    {
        return new(WriteByteArray);
    }

    public static void WriteByteArray(BinaryWriter writer, byte[]? array)
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        if (array is null)
        {
            IntSerializer.WriteNull(writer);
            return;
        }

        var count = array.Length;
        IntSerializer.Write(writer, count);
        writer.Write(array);
    }

    private Writer CreateDictionaryWriter(Type keyType, Type valueType, Type genericTypeDefinition)
    {
        if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
        {
            return (Writer)this.GetPrivateMethod(nameof(CreateReadOnlyDictionaryWriterFor))
                .MakeGenericMethod(keyType, valueType)
                .Invoke(this, null)!;
        }
        else
        {
            return (Writer)this.GetPrivateMethod(nameof(CreateDictionaryWriterFor))
                .MakeGenericMethod(keyType, valueType)
                .Invoke(this, null)!;
        }
    }

    private Writer CreateDictionaryWriterFor<TKey, TValue>()
    {
        // Almost identical to the method below.

        var (writeKey, _) = GetWriter<TKey>();
        var (writeValue, _) = GetWriter<TValue>();

        return new((BinaryWriter writer, IDictionary<TKey, TValue>? dictionary) =>
        {
            if (dictionary is null)
            {
                IntSerializer.WriteNull(writer);
                return;
            }

            var count = dictionary.Count;
            IntSerializer.Write(writer, count);
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
        });
    }

    private Writer CreateReadOnlyDictionaryWriterFor<TKey, TValue>()
    {
        // Almost identical to the method above.

        var (writeKey, _) = GetWriter<TKey>();
        var (writeValue, _) = GetWriter<TValue>();

        return new((BinaryWriter writer, IReadOnlyDictionary<TKey, TValue>? dictionary) =>
        {
            if (dictionary is null)
            {
                IntSerializer.WriteNull(writer);
                return;
            }

            var count = dictionary.Count;
            IntSerializer.Write(writer, count);
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
        });
    }

    private Writer CreateTuple2Writer(Tuple2Type tuple2)
    {
        return (Writer)this.GetPrivateMethod(nameof(CreateTuple2WriterFor))
            .MakeGenericMethod(tuple2.Item1Type, tuple2.Item2Type)
            .Invoke(this, [tuple2.IsNullable])!;
    }

    private Writer CreateTuple2WriterFor<T1, T2>(bool isNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var (writeItem1, _) = GetWriter<T1>();
        var (writeItem2, _) = GetWriter<T2>();

        if (isNullable)
        {
            return new((BinaryWriter writer, (T1, T2)? tuple) =>
            {
                if (tuple is null)
                {
                    writer.Write(NativeWriter.NULL);

                    return;
                }

                writer.Write(NativeWriter.NOT_NULL);
                writeItem1(writer, tuple.Value.Item1);
                writeItem2(writer, tuple.Value.Item2);
            });
        }

        return new((BinaryWriter writer, (T1, T2) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
        });
    }

    private Writer CreateTuple3Writer(Tuple3Type tuple3)
    {
        return (Writer)this.GetPrivateMethod(nameof(CreateTuple3WriterFor))
            .MakeGenericMethod(tuple3.Item1Type, tuple3.Item2Type, tuple3.Item3Type)
            .Invoke(this, [tuple3.IsNullable])!;
    }

    private Writer CreateTuple3WriterFor<T1, T2, T3>(bool isNullable)
    {
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        var (writeItem1, _) = GetWriter<T1>();
        var (writeItem2, _) = GetWriter<T2>();
        var (writeItem3, _) = GetWriter<T3>();

        if (isNullable)
        {
            return new((BinaryWriter writer, (T1, T2, T3)? tuple) =>
            {
                if (tuple is null)
                {
                    writer.Write(NativeWriter.NULL);

                    return;
                }

                writer.Write(NativeWriter.NOT_NULL);
                writeItem1(writer, tuple.Value.Item1);
                writeItem2(writer, tuple.Value.Item2);
                writeItem3(writer, tuple.Value.Item3);
            });
        }

        return new((BinaryWriter writer, (T1, T2, T3) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
            writeItem3(writer, tuple.Item3);
        });
    }
}
