using System.Text.Json.Nodes;

namespace Bon.Serializer.Deserialization;

internal static class BonToJsonDeserializer
{
    public static JsonNode? Deserialize(BinaryReader reader, Schema schema)
    {
        return schema switch
        {
            { IsNative: true } => DeserializeNative(reader, schema),
            { IsUnion: true } => DeserializeUnion(reader, schema),
            { IsArray: true } => DeserializeArray(reader, schema),
            _ => DeserializeRecordLike(reader, schema),
        };
    }

    private static JsonValue? DeserializeNative(BinaryReader reader, Schema schema)
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

    private static JsonArray? DeserializeRecordLike(BinaryReader reader, Schema schema)
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

        var valueSchemas = schema.IsTuple ? schema.SchemaArguments : schema.Members.Select(member => member.Schema).ToArray();

        return new JsonArray(valueSchemas.Select(valueSchema => Deserialize(reader, valueSchema)).ToArray());
    }

    private static JsonArray? DeserializeUnion(BinaryReader reader, Schema schema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        if (IntSerializer.Read(reader) is not int id)
        {
            return null;
        }

        var recordSchema = schema.Members.First(member => member.Id == id).Schema;
        var contents = DeserializeRecordLike(reader, recordSchema);

        return [id, contents];
    }

    private static JsonArray? DeserializeArray(BinaryReader reader, Schema schema)
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        if (IntSerializer.Read(reader) is not int count)
        {
            return null;
        }

        var array = new JsonArray();

        for (var i = 0; i < count; i++)
        {
            array.Add(Deserialize(reader, schema.SchemaArguments[0]));
        }

        return array;
    }
}
