using System.Text.Json.Nodes;

namespace Bon.Serializer.Deserialization;

internal static class BonToJsonDeserializer
{
    public static JsonNode? Deserialize(BinaryReader reader, Schema schema)
    {
        return schema switch
        {
            NativeSchema nativeSchema => DeserializeNative(reader, nativeSchema),
            UnionSchema unionSchema => DeserializeUnion(reader, unionSchema),
            ArraySchema arraySchema => DeserializeArray(reader, arraySchema),
            DictionarySchema dictionarySchema => DeserializeDictionary(reader, dictionarySchema),
            _ => DeserializeRecordLike(reader, schema.IsNullable, schema.GetInnerSchemas()),
        };
    }

    private static JsonValue? DeserializeNative(BinaryReader reader, NativeSchema schema)
    {
        return (schema.SchemaType, schema.IsNullable) switch
        {
            (SchemaType.String, false) => JsonValue.Create(NativeSerializer.ReadString(reader)),
            (SchemaType.Bool, false) => JsonValue.Create(NativeSerializer.ReadBool(reader)),
            (SchemaType.Byte, false) => JsonValue.Create(NativeSerializer.ReadByte(reader)),
            (SchemaType.SByte, false) => JsonValue.Create(NativeSerializer.ReadSByte(reader)),
            (SchemaType.Short, false) => JsonValue.Create(NativeSerializer.ReadShort(reader)),
            (SchemaType.UShort, false) => JsonValue.Create(NativeSerializer.ReadUShort(reader)),
            (SchemaType.Int, false) => JsonValue.Create(NativeSerializer.ReadInt(reader)),
            (SchemaType.UInt, false) => JsonValue.Create(NativeSerializer.ReadUInt(reader)),
            (SchemaType.Long, false) => JsonValue.Create(NativeSerializer.ReadLong(reader)),
            (SchemaType.ULong, false) => JsonValue.Create(NativeSerializer.ReadULong(reader)),
            (SchemaType.Float, false) => JsonValue.Create(NativeSerializer.ReadFloat(reader)),
            (SchemaType.Double, false) => JsonValue.Create(NativeSerializer.ReadDouble(reader)),
            (SchemaType.Decimal, false) => JsonValue.Create(NativeSerializer.ReadDecimal(reader)),
            (SchemaType.Guid, false) => JsonValue.Create(NativeSerializer.ReadGuid(reader)),

            (SchemaType.String, true) => JsonValue.Create(NativeSerializer.ReadNullableString(reader)),
            (SchemaType.Bool, true) => JsonValue.Create(NativeSerializer.ReadNullableBool(reader)),
            (SchemaType.WholeNumber, true) => JsonValue.Create(NativeSerializer.ReadNullableWholeNumber(reader)),
            (SchemaType.SignedWholeNumber, true) => JsonValue.Create(NativeSerializer.ReadNullableSignedWholeNumber(reader)),
            (SchemaType.Float, true) => JsonValue.Create(NativeSerializer.ReadNullableFloat(reader)),
            (SchemaType.Double, true) => JsonValue.Create(NativeSerializer.ReadNullableDouble(reader)),
            (SchemaType.Decimal, true) => JsonValue.Create(NativeSerializer.ReadNullableDecimal(reader)),
            (SchemaType.Guid, true) => JsonValue.Create(NativeSerializer.ReadNullableGuid(reader)),

            _ => throw new ArgumentOutOfRangeException(nameof(schema), schema, null)
        };
    }

    private static JsonArray? DeserializeRecordLike(BinaryReader reader, bool isNullable, IEnumerable<Schema> memberSchemas)
    {
        // See bookmark 831853187 for all places where a record is serialized/deserialized.
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        if (isNullable)
        {
            var firstByte = reader.ReadByte();

            if (firstByte == NativeSerializer.NULL)
            {
                return null;
            }
        }

        return new JsonArray(memberSchemas.Select(memberSchema => Deserialize(reader, memberSchema)).ToArray());
    }

    private static JsonArray? DeserializeUnion(BinaryReader reader, UnionSchema schema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        if ((int?)WholeNumberSerializer.ReadNullable(reader) is not int id)
        {
            return null;
        }

        var recordSchema = (RecordSchema)schema.Members.First(member => member.Id == id).Schema;
        var contents = DeserializeRecordLike(reader, recordSchema.IsNullable, recordSchema.GetInnerSchemas());

        return [id, contents];
    }

    private static JsonArray? DeserializeArray(BinaryReader reader, ArraySchema schema)
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        if ((int?)WholeNumberSerializer.ReadNullable(reader) is not int count)
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

        var tupleSchema = new Tuple2Schema(SchemaType.Tuple2, false)
        {
            InnerSchema1 = schema.InnerSchema1,
            InnerSchema2 = schema.InnerSchema2,
        };

        var arraySchema = new ArraySchema(SchemaType.Array, schema.IsNullable)
        {
            InnerSchema = tupleSchema,
        };

        return DeserializeArray(reader, arraySchema);
    }
}
