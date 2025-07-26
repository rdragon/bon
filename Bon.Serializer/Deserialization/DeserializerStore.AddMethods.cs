namespace Bon.Serializer.Deserialization;

partial class DeserializerStore
{
    /// <param name="deserializer">
    /// A method that reads binary data with as schema the schema that is used when serializing values
    /// of the target type and that outputs a value of the target type.
    /// </param>
    public void AddDeserializer(Type targetType, Delegate deserializer)
    {
        var schema = schemaStore.GetOrAddSchema(targetType);
        _deserializers[(schema, targetType)] = deserializer;
    }

    public void AddReaderFactory(Type type, Delegate factory) => RecordDeserializer.ReaderFactories[type] = factory;

    public void AddNativeReaders()
    {
        // Bookmark 659516266 (native serialization)
        // For every native schema type there should be a reader defined here.
        // The output types should match the types at bookmark 683558879.
        AddNativeReader(Schema.String, static input => NativeSerializer.ReadString(input.Reader));
        AddNativeReader(Schema.Byte, static input => NativeSerializer.ReadByte(input.Reader));
        AddNativeReader(Schema.SByte, static input => NativeSerializer.ReadSByte(input.Reader));
        AddNativeReader(Schema.Short, static input => NativeSerializer.ReadShort(input.Reader));
        AddNativeReader(Schema.UShort, static input => NativeSerializer.ReadUShort(input.Reader));
        AddNativeReader(Schema.Int, static input => NativeSerializer.ReadInt(input.Reader));
        AddNativeReader(Schema.UInt, static input => NativeSerializer.ReadUInt(input.Reader));
        AddNativeReader(Schema.Long, static input => NativeSerializer.ReadLong(input.Reader));
        AddNativeReader(Schema.ULong, static input => NativeSerializer.ReadULong(input.Reader));
        AddNativeReader(Schema.Float, static input => NativeSerializer.ReadFloat(input.Reader));
        AddNativeReader(Schema.Double, static input => NativeSerializer.ReadDouble(input.Reader));
        AddNativeReader(Schema.NullableDecimal, static input => NativeSerializer.ReadNullableDecimal(input.Reader));
        AddNativeReader(Schema.WholeNumber, static input => WholeNumberSerializer.Read(input.Reader));
        AddNativeReader(Schema.SignedWholeNumber, static input => WholeNumberSerializer.ReadSigned(input.Reader));
        AddNativeReader(Schema.FractionalNumber, static input => FractionalNumberSerializer.Read(input.Reader));
    }

    private void AddNativeReader<T>(Schema sourceSchema, Read<T> reader)
    {
        _deserializers[(sourceSchema, typeof(T))] = reader;
        _nativeMethods[sourceSchema.SchemaType] = (reader, SkipValue);

        void SkipValue(BonInput input) => reader(input);
    }
}
