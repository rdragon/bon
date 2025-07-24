using System.Text.Json.Nodes;

namespace Bon.Serializer.Deserialization;

internal static class BonToJsonDeserializer
{
    public static JsonNode? Deserialize(BinaryReader reader, Schema1 schema)
    {
        return schema switch
        {
            NativeSchema nativeSchema => DeserializeNative(reader, nativeSchema),
            UnionSchema unionSchema => DeserializeUnion(reader, unionSchema),
            ArraySchema arraySchema => DeserializeArray(reader, arraySchema),
            DictionarySchema dictionarySchema => DeserializeDictionary(reader, dictionarySchema),
            _ => DeserializeRecordLike(reader, schema),
        };
    }

    private static JsonValue? DeserializeNative(BinaryReader reader, NativeSchema schema)
    {
        return schema.SchemaType switch
        {
            SchemaType.String => JsonValue.Create(NativeSerializer.ReadString(reader)),
            SchemaType.Byte => JsonValue.Create(NativeSerializer.ReadByte(reader)),
            SchemaType.SByte => JsonValue.Create(NativeSerializer.ReadSByte(reader)),
            SchemaType.Short => JsonValue.Create(NativeSerializer.ReadShort(reader)),
            SchemaType.UShort => JsonValue.Create(NativeSerializer.ReadUShort(reader)),
            SchemaType.Int => JsonValue.Create(NativeSerializer.ReadInt(reader)),
            SchemaType.UInt => JsonValue.Create(NativeSerializer.ReadUInt(reader)),
            SchemaType.Long => JsonValue.Create(NativeSerializer.ReadLong(reader)),
            SchemaType.ULong => JsonValue.Create(NativeSerializer.ReadULong(reader)),
            SchemaType.Float => JsonValue.Create(NativeSerializer.ReadFloat(reader)),
            SchemaType.Double => JsonValue.Create(NativeSerializer.ReadDouble(reader)),
            SchemaType.NullableDecimal => JsonValue.Create(NativeSerializer.ReadDecimal(reader)),

            SchemaType.WholeNumber => JsonValue.Create(WholeNumberSerializer.Read(reader)),
            SchemaType.SignedWholeNumber => JsonValue.Create(WholeNumberSerializer.ReadSigned(reader)),
            SchemaType.FractionalNumber => JsonValue.Create(FractionalNumberSerializer.Read(reader)),
        };
    }

    private static JsonArray? DeserializeRecordLike(BinaryReader reader, Schema1 schema)
    {
        // See bookmark 831853187 for all places where a record is serialized/deserialized.
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        if (schema.IsNullable)
        {
            if (reader.ReadByte() == NativeWriter.NULL)
            {
                return null;
            }
        }

        return new JsonArray(schema.GetInnerSchemas().Select(memberSchema => Deserialize(reader, memberSchema)).ToArray());
    }

    private static JsonArray? DeserializeUnion(BinaryReader reader, UnionSchema schema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        if (IntSerializer.Read(reader) is not int id)
        {
            return null;
        }

        var recordSchema = (RecordSchema)schema.Members.First(member => member.Id == id).Schema;
        var contents = DeserializeRecordLike(reader, recordSchema);

        return [id, contents];
    }

    private static JsonArray? DeserializeArray(BinaryReader reader, ArraySchema schema)
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        if (IntSerializer.Read(reader) is not int count)
        {
            return null;
        }

        var array = new JsonArray();

        for (var i = 0; i < count; i++)
        {
            array.Add(Deserialize(reader, schema.InnerSchema));
        }

        return array;
    }

    private static JsonArray? DeserializeDictionary(BinaryReader reader, DictionarySchema schema)
    {
        // See bookmark 662741575 for all places where a dictionary is serialized/deserialized.

        var tupleSchema = new Tuple2Schema(SchemaType.Tuple2)
        {
            InnerSchema1 = schema.InnerSchema1,
            InnerSchema2 = schema.InnerSchema2,
        };

        var arraySchema = new ArraySchema()
        {
            InnerSchema = tupleSchema,
        };

        return DeserializeArray(reader, arraySchema);
    }
}
