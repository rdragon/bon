using System.Text.Json.Nodes;

namespace Bon.Serializer.Deserialization;

internal static class JsonToBonSerializer
{
    public static void Serialize(BinaryWriter writer, JsonNode? jsonNode, Schema schema)
    {
        switch (schema)
        {
            case NativeSchema nativeSchema: SerializeNative(writer, jsonNode, nativeSchema); break;
            case UnionSchema unionSchema: SerializeUnion(writer, jsonNode, unionSchema); break;
            case ArraySchema arraySchema: SerializeArray(writer, jsonNode, arraySchema); break;
            case DictionarySchema dictionarySchema: SerializeDictionary(writer, jsonNode, dictionarySchema); break;
            default: SerializeRecordLike(writer, jsonNode, schema.IsNullable, schema.GetInnerSchemas().ToArray()); break;
        }
    }

    private static void SerializeNative(BinaryWriter writer, JsonNode? jsonNode, NativeSchema schema)
    {
        switch (schema.SchemaType)
        {
            case SchemaType.String: NativeSerializer.WriteString(writer, jsonNode?.GetValue<string>()); break;
            case SchemaType.Byte: NativeSerializer.WriteByte(writer, ExpectNotNull(jsonNode).GetValue<byte>()); break;
            case SchemaType.SByte: NativeSerializer.WriteSByte(writer, ExpectNotNull(jsonNode).GetValue<sbyte>()); break;
            case SchemaType.Short: NativeSerializer.WriteShort(writer, ExpectNotNull(jsonNode).GetValue<short>()); break;
            case SchemaType.UShort: NativeSerializer.WriteUShort(writer, ExpectNotNull(jsonNode).GetValue<ushort>()); break;
            case SchemaType.Int: NativeSerializer.WriteInt(writer, ExpectNotNull(jsonNode).GetValue<int>()); break;
            case SchemaType.UInt: NativeSerializer.WriteUInt(writer, ExpectNotNull(jsonNode).GetValue<uint>()); break;
            case SchemaType.Long: NativeSerializer.WriteLong(writer, ExpectNotNull(jsonNode).GetValue<long>()); break;
            case SchemaType.ULong: NativeSerializer.WriteULong(writer, ExpectNotNull(jsonNode).GetValue<ulong>()); break;
            case SchemaType.Float: NativeSerializer.WriteFloat(writer, ExpectNotNull(jsonNode).GetValue<float>()); break;
            case SchemaType.Double: NativeSerializer.WriteDouble(writer, ExpectNotNull(jsonNode).GetValue<double>()); break;
            case SchemaType.FractionalNumber: NativeSerializer.WriteNullableDouble(writer, jsonNode?.GetValue<double>()); break;
            case SchemaType.NullableDecimal: NativeSerializer.WriteNullableDecimal(writer, jsonNode?.GetValue<decimal>()); break;
            case SchemaType.WholeNumber: WholeNumberSerializer.Write(writer, jsonNode?.GetValue<ulong>()); break;
            case SchemaType.SignedWholeNumber: WholeNumberSerializer.WriteSigned(writer, jsonNode?.GetValue<long>()); break;
        }
    }

    private static void SerializeRecordLike(BinaryWriter writer, JsonNode? jsonNode, bool isNullable, IReadOnlyList<Schema> memberSchemas)
    {
        // See bookmark 831853187 for all places where a record is serialized/deserialized.
        // See bookmark 747115664 for all places where a tuple is serialized/deserialized.

        if (isNullable)
        {
            if (jsonNode is null)
            {
                writer.Write(NativeWriter.NULL);

                return;
            }

            writer.Write(NativeWriter.NOT_NULL);
        }

        foreach (var (member, memberSchema) in ExpectArray(jsonNode, memberSchemas.Count).Zip(memberSchemas))
        {
            Serialize(writer, member, memberSchema);
        }
    }

    private static void SerializeUnion(BinaryWriter writer, JsonNode? jsonNode, UnionSchema unionSchema)
    {
        // See bookmark 628227999 for all places where a union is serialized/deserialized.

        if (unionSchema.IsNullable && jsonNode is null)
        {
            IntSerializer.WriteNull(writer);

            return;
        }

        var parts = ExpectArray(jsonNode, 2);
        var unionId = ExpectNotNull(parts[0]).GetValue<int>();
        var unionNode = parts[1];
        IntSerializer.Write(writer, unionId);
        var recordSchema = unionSchema.Members.First(member => member.Id == unionId).Schema;
        Serialize(writer, unionNode, recordSchema);
    }

    private static void SerializeArray(BinaryWriter writer, JsonNode? jsonNode, ArraySchema arraySchema)
    {
        // See bookmark 791351735 for all places where an array is serialized/deserialized.

        if (arraySchema.IsNullable && jsonNode is null)
        {
            IntSerializer.WriteNull(writer);

            return;
        }

        var array = ExpectNotNull(jsonNode).AsArray();
        IntSerializer.Write(writer, array.Count);

        foreach (var element in array)
        {
            Serialize(writer, element, arraySchema.InnerSchema);
        }
    }

    private static void SerializeDictionary(BinaryWriter writer, JsonNode? jsonNode, DictionarySchema dictionarySchema)
    {
        // See bookmark 662741575 for all places where a dictionary is serialized/deserialized.

        var tupleSchema = new Tuple2Schema(SchemaType.Tuple2)
        {
            InnerSchema1 = dictionarySchema.InnerSchema1,
            InnerSchema2 = dictionarySchema.InnerSchema2,
        };

        var arraySchema = new ArraySchema()
        {
            InnerSchema = tupleSchema,
        };

        SerializeArray(writer, jsonNode, arraySchema);
    }

    private static JsonArray ExpectArray(JsonNode? jsonNode, int count) => ExpectCount(ExpectNotNull(jsonNode).AsArray(), count);

    private static JsonNode ExpectNotNull(JsonNode? jsonNode) =>
        jsonNode ?? throw new InvalidOperationException("Expected a non-null value but found null.");

    private static JsonArray ExpectCount(JsonArray jsonArray, int count)
    {
        if (jsonArray.Count != count)
        {
            throw new InvalidOperationException($"Expected an array of length {count} but found an array of length {jsonArray.Count}.");
        }

        return jsonArray;
    }
}
