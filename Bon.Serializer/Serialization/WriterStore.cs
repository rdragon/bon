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
        Add<string>(NativeSerializer.WriteString, false);
        Add<bool>(NativeSerializer.WriteBool, false);
        Add<byte>(NativeSerializer.WriteByte, false, SimpleWriterType.Byte);
        Add<sbyte>(NativeSerializer.WriteSByte, false, SimpleWriterType.SByte);
        Add<short>(NativeSerializer.WriteShort, false, SimpleWriterType.Short);
        Add<ushort>(NativeSerializer.WriteUShort, false, SimpleWriterType.UShort);
        Add<int>(NativeSerializer.WriteInt, false, SimpleWriterType.Int);
        Add<uint>(NativeSerializer.WriteUInt, false, SimpleWriterType.UInt);
        Add<long>(NativeSerializer.WriteLong, false, SimpleWriterType.Long);
        Add<ulong>(NativeSerializer.WriteULong, false, SimpleWriterType.ULong);
        Add<float>(NativeSerializer.WriteFloat, false);
        Add<double>(NativeSerializer.WriteDouble, false);
        Add<decimal>(NativeSerializer.WriteDecimal, false);
        Add<Guid>(NativeSerializer.WriteGuid, false);
        Add<char>(NativeSerializer.WriteChar, false);
        Add<DateTime>(NativeSerializer.WriteDateTime, false);
        Add<DateTimeOffset>(NativeSerializer.WriteDateTimeOffset, false);
        Add<TimeSpan>(NativeSerializer.WriteTimeSpan, false);
        Add<DateOnly>(NativeSerializer.WriteDateOnly, false);
        Add<TimeOnly>(NativeSerializer.WriteTimeOnly, false);

        Add<bool?>(NativeSerializer.WriteNullableBool, false);
        Add<byte?>(NativeSerializer.WriteNullableByte, false);
        Add<sbyte?>(NativeSerializer.WriteNullableSByte, false);
        Add<short?>(NativeSerializer.WriteNullableShort, false);
        Add<ushort?>(NativeSerializer.WriteNullableUShort, false);
        Add<int?>(NativeSerializer.WriteNullableInt, false);
        Add<uint?>(NativeSerializer.WriteNullableUInt, false);
        Add<long?>(NativeSerializer.WriteNullableLong, false);
        Add<ulong?>(NativeSerializer.WriteNullableULong, false);
        Add<float?>(NativeSerializer.WriteNullableFloat, false);
        Add<double?>(NativeSerializer.WriteNullableDouble, false);
        Add<decimal?>(NativeSerializer.WriteNullableDecimal, false);
        Add<Guid?>(NativeSerializer.WriteNullableGuid, false);
        Add<char?>(NativeSerializer.WriteNullableChar, false);
        Add<DateTime?>(NativeSerializer.WriteNullableDateTime, false);
        Add<DateTimeOffset?>(NativeSerializer.WriteNullableDateTimeOffset, false);
        Add<TimeSpan?>(NativeSerializer.WriteNullableTimeSpan, false);
        Add<DateOnly?>(NativeSerializer.WriteNullableDateOnly, false);
        Add<TimeOnly?>(NativeSerializer.WriteNullableTimeOnly, false);
    }

    /// <param name="usesCustomSchemas">
    /// Whether the writer might use custom schemas.
    /// This value controls whether the header of the final binary message includes a block ID.
    /// </param>
    public void Add<T>(Action<BinaryWriter, T> writer, bool usesCustomSchemas, SimpleWriterType simpleWriterType = SimpleWriterType.None)
    {
        _writers[typeof(T)] = new(writer, usesCustomSchemas, simpleWriterType);
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

        var (writeElement, usesCustomSchemas, _) = GetWriter<TElement>();

        return new((BinaryWriter writer, IEnumerable<TElement> collection) =>
        {//0at: goed met nullable omgaan, 791351735
            var elements = collection as IReadOnlyList<TElement> ?? collection.ToArray();
            var count = elements.Count;
            IntSerializer.Write(writer, count);

            for (int i = 0; i < count; i++)
            {
                writeElement(writer, elements[i]);
            }
        }, usesCustomSchemas);
    }

    private static Writer CreateByteArrayWriter()
    {
        return new(WriteByteArray, false);
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

        var (writeKey, usesCustomSchemas1, _) = GetWriter<TKey>();
        var (writeValue, usesCustomSchemas2, _) = GetWriter<TValue>();

        var usesCustomSchemas = usesCustomSchemas1 | usesCustomSchemas2;

        return new((BinaryWriter writer, IDictionary<TKey, TValue> dictionary) =>
        {
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
        }, usesCustomSchemas);
    }

    private Writer CreateReadOnlyDictionaryWriterFor<TKey, TValue>()
    {
        // Almost identical to the method above.

        var (writeKey, usesCustomSchemas1, _) = GetWriter<TKey>();
        var (writeValue, usesCustomSchemas2, _) = GetWriter<TValue>();

        var usesCustomSchemas = usesCustomSchemas1 | usesCustomSchemas2;

        return new((BinaryWriter writer, IReadOnlyDictionary<TKey, TValue> dictionary) =>
        {
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
        }, usesCustomSchemas);
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

        var (writeItem1, usesCustomSchemas1, _) = GetWriter<T1>();
        var (writeItem2, usesCustomSchemas2, _) = GetWriter<T2>();

        var usesCustomSchemas = usesCustomSchemas1 | usesCustomSchemas2;

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
            }, usesCustomSchemas);
        }

        return new((BinaryWriter writer, (T1, T2) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
        }, usesCustomSchemas);
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

        var (writeItem1, usesCustomSchemas1, _) = GetWriter<T1>();
        var (writeItem2, usesCustomSchemas2, _) = GetWriter<T2>();
        var (writeItem3, usesCustomSchemas3, _) = GetWriter<T3>();

        var usesCustomSchemas = usesCustomSchemas1 | usesCustomSchemas2 | usesCustomSchemas3;

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
            }, usesCustomSchemas);
        }

        return new((BinaryWriter writer, (T1, T2, T3) tuple) =>
        {
            writeItem1(writer, tuple.Item1);
            writeItem2(writer, tuple.Item2);
            writeItem3(writer, tuple.Item3);
        }, usesCustomSchemas);
    }
}
