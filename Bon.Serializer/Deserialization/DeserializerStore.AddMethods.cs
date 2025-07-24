namespace Bon.Serializer.Deserialization;

partial class DeserializerStore
{
    /// <param name="deserializer">
    /// A method that reads binary data with as schema the schema that is used when serializing values
    /// of the target type and that outputs a value of the target type.
    /// </param>
    public void AddDeserializer(Type targetType, Delegate deserializer)
    {
        var schema = schemaStore.GetOrAdd(targetType);
        _deserializers[(schema, targetType)] = deserializer;
    }

    public void AddReaderFactory(Type type, Delegate factory) => RecordDeserializer.ReaderFactories[type] = factory;

    public void AddNativeReaders()
    {
        // Bookmark 659516266 (native serialization)
        // For every native schema type there should be a reader defined here.
        // The output types should match the types at bookmark 683558879.
        AddNativeReader(NativeSchema.String, static input => NativeSerializer.ReadString(input.Reader));
        AddNativeReader(NativeSchema.Byte, static input => NativeSerializer.ReadByte(input.Reader));
        AddNativeReader(NativeSchema.SByte, static input => NativeSerializer.ReadSByte(input.Reader));
        AddNativeReader(NativeSchema.Short, static input => NativeSerializer.ReadShort(input.Reader));
        AddNativeReader(NativeSchema.UShort, static input => NativeSerializer.ReadUShort(input.Reader));
        AddNativeReader(NativeSchema.Int, static input => NativeSerializer.ReadInt(input.Reader));
        AddNativeReader(NativeSchema.UInt, static input => NativeSerializer.ReadUInt(input.Reader));
        AddNativeReader(NativeSchema.Long, static input => NativeSerializer.ReadLong(input.Reader));
        AddNativeReader(NativeSchema.ULong, static input => NativeSerializer.ReadULong(input.Reader));
        AddNativeReader(NativeSchema.Float, static input => NativeSerializer.ReadFloat(input.Reader));
        AddNativeReader(NativeSchema.Double, static input => NativeSerializer.ReadDouble(input.Reader));
        AddNativeReader(NativeSchema.NullableDecimal, static input => NativeSerializer.ReadNullableDecimal(input.Reader));
        AddNativeReader(NativeSchema.WholeNumber, static input => WholeNumberSerializer.Read(input.Reader));
        AddNativeReader(NativeSchema.SignedWholeNumber, static input => WholeNumberSerializer.ReadSigned(input.Reader));
        AddNativeReader(NativeSchema.FractionalNumber, static input => FractionalNumberSerializer.Read(input.Reader));
    }

    private void AddNativeReader<T>(NativeSchema sourceSchema, Read<T> reader)
    {
        _deserializers[(sourceSchema, typeof(T))] = reader;
        _nativeMethods[sourceSchema.SchemaType] = (reader, SkipValue);

        void SkipValue(BonInput input) => reader(input);
    }
}
