// Bookmark 413211217
namespace Bon.Serializer.Serialization;

// This class is also directly used by the source generated code.
public static class NativeSerializer
{
    public const byte NULL = 255;
    public const byte NOT_NULL = 254;

    public static void WriteString(BinaryWriter writer, string? value) => StringSerializer.WriteString(writer, value);

    public static string ReadString(BinaryReader reader) => StringDeserializer.ReadString(reader);

    public static string? ReadNullableString(BinaryReader reader) => StringDeserializer.ReadNullableString(reader);

    public static void WriteBool(BinaryWriter writer, bool value) => writer.Write(value);

    public static bool ReadBool(BinaryReader reader) => reader.ReadBoolean();

    public static void WriteNullableBool(BinaryWriter writer, bool? value)
    {
        writer.Write(value switch
        {
            false => (byte)0,
            true => (byte)1,
            null => NULL,
        });
    }

    public static bool? ReadNullableBool(BinaryReader reader)
    {
        return reader.ReadByte() switch
        {
            0 => false,
            1 => true,
            _ => null,
        };
    }

    public static void WriteByte(BinaryWriter writer, byte value) => writer.Write(value);
    public static void WriteSByte(BinaryWriter writer, sbyte value) => writer.Write(value);

    public static byte ReadByte(BinaryReader reader) => reader.ReadByte();
    public static sbyte ReadSByte(BinaryReader reader) => reader.ReadSByte();

    public static void WriteNullableByte(BinaryWriter writer, byte? value) => WriteNullableWholeNumber(writer, value);
    public static void WriteNullableSByte(BinaryWriter writer, sbyte? value) => WriteNullableSignedWholeNumber(writer, value);

    public static byte? ReadNullableByte(BinaryReader reader) => (byte?)ReadNullableWholeNumber(reader);
    public static sbyte? ReadNullableSByte(BinaryReader reader) => (sbyte?)ReadNullableSignedWholeNumber(reader);

    public static void WriteShort(BinaryWriter writer, short value) => writer.Write(value);
    public static void WriteUShort(BinaryWriter writer, ushort value) => writer.Write(value);

    public static short ReadShort(BinaryReader reader) => reader.ReadInt16();
    public static ushort ReadUShort(BinaryReader reader) => reader.ReadUInt16();

    public static void WriteNullableShort(BinaryWriter writer, short? value) => WriteNullableSignedWholeNumber(writer, value);
    public static void WriteNullableUShort(BinaryWriter writer, ushort? value) => WriteNullableWholeNumber(writer, value);

    public static short? ReadNullableShort(BinaryReader reader) => (short?)ReadNullableSignedWholeNumber(reader);
    public static ushort? ReadNullableUShort(BinaryReader reader) => (ushort?)ReadNullableWholeNumber(reader);

    public static void WriteInt(BinaryWriter writer, int value) => writer.Write(value);
    public static void WriteUInt(BinaryWriter writer, uint value) => writer.Write(value);

    public static int ReadInt(BinaryReader reader) => reader.ReadInt32();
    public static uint ReadUInt(BinaryReader reader) => reader.ReadUInt32();

    public static void WriteNullableInt(BinaryWriter writer, int? value) => WriteNullableSignedWholeNumber(writer, value);
    public static void WriteNullableUInt(BinaryWriter writer, uint? value) => WriteNullableWholeNumber(writer, value);

    public static int? ReadNullableInt(BinaryReader reader) => (int?)ReadNullableSignedWholeNumber(reader);
    public static uint? ReadNullableUInt(BinaryReader reader) => (uint?)ReadNullableWholeNumber(reader);

    public static void WriteLong(BinaryWriter writer, long value) => writer.Write(value);
    public static void WriteULong(BinaryWriter writer, ulong value) => writer.Write(value);

    public static long ReadLong(BinaryReader reader) => reader.ReadInt64();
    public static ulong ReadULong(BinaryReader reader) => reader.ReadUInt64();

    public static void WriteNullableLong(BinaryWriter writer, long? value) => WriteNullableSignedWholeNumber(writer, value);
    public static void WriteNullableULong(BinaryWriter writer, ulong? value) => WriteNullableWholeNumber(writer, value);

    public static long? ReadNullableLong(BinaryReader reader) => ReadNullableSignedWholeNumber(reader);
    public static ulong? ReadNullableULong(BinaryReader reader) => ReadNullableWholeNumber(reader);

    public static void WriteFloat(BinaryWriter writer, float value) => writer.Write(value);

    public static float ReadFloat(BinaryReader reader) => reader.ReadSingle();

    public static void WriteNullableFloat(BinaryWriter writer, float? value)
    {
        if (value is float f)
        {
            writer.Write(NOT_NULL);
            writer.Write(f);
        }
        else
        {
            writer.Write(NULL);
        }
    }

    public static float? ReadNullableFloat(BinaryReader reader) => reader.ReadByte() == NULL ? null : reader.ReadSingle();

    public static void WriteDouble(BinaryWriter writer, double value) => writer.Write(value);

    public static double ReadDouble(BinaryReader reader) => reader.ReadDouble();

    public static void WriteNullableDouble(BinaryWriter writer, double? value)
    {
        if (value is double d)
        {
            writer.Write(NOT_NULL);
            writer.Write(d);
        }
        else
        {
            writer.Write(NULL);
        }
    }

    public static double? ReadNullableDouble(BinaryReader reader) => reader.ReadByte() == NULL ? null : reader.ReadDouble();

    public static void WriteDecimal(BinaryWriter writer, decimal value) => writer.Write(value);

    public static decimal ReadDecimal(BinaryReader reader) => reader.ReadDecimal();

    public static void WriteNullableDecimal(BinaryWriter writer, decimal? value)
    {
        if (value is decimal d)
        {
            writer.Write(NOT_NULL);
            writer.Write(d);
        }
        else
        {
            writer.Write(NULL);
        }
    }

    public static decimal? ReadNullableDecimal(BinaryReader reader) => reader.ReadByte() == NULL ? null : reader.ReadDecimal();

    public static void WriteGuid(BinaryWriter writer, Guid value) => writer.Write(value.ToByteArray());

    public static Guid ReadGuid(BinaryReader reader) => new(reader.ReadBytes(16));

    public static void WriteNullableGuid(BinaryWriter writer, Guid? value)
    {
        if (value is Guid guid)
        {
            writer.Write(NOT_NULL);
            WriteGuid(writer, guid);
        }
        else
        {
            writer.Write(NULL);
        }
    }

    public static Guid? ReadNullableGuid(BinaryReader reader) => reader.ReadByte() == NULL ? null : ReadGuid(reader);

    public static void WriteWholeNumber(BinaryWriter writer, ulong value) => WholeNumberSerializer.Write(writer, value);
    public static ulong ReadWholeNumber(BinaryReader reader) => WholeNumberSerializer.Read(reader);
    public static void WriteNullableWholeNumber(BinaryWriter writer, ulong? value) => WholeNumberSerializer.WriteNullable(writer, value);
    public static ulong? ReadNullableWholeNumber(BinaryReader reader) => WholeNumberSerializer.ReadNullable(reader);

    public static void WriteSignedWholeNumber(BinaryWriter writer, long value) => WholeNumberSerializer.WriteSigned(writer, value);
    public static long ReadSignedWholeNumber(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader);
    public static void WriteNullableSignedWholeNumber(BinaryWriter writer, long? value) => WholeNumberSerializer.WriteNullableSigned(writer, value);
    public static long? ReadNullableSignedWholeNumber(BinaryReader reader) => WholeNumberSerializer.ReadNullableSigned(reader);

    // Bookmark 659516266 (char serialization)
    public static void WriteChar(BinaryWriter writer, char value) => WriteWholeNumber(writer, value.ToSchemaType());
    public static char ReadChar(BinaryReader reader) => ReadWholeNumber(reader).ToChar();
    public static void WriteNullableChar(BinaryWriter writer, char? value) => WriteNullableWholeNumber(writer, value?.ToSchemaType());
    public static char? ReadNullableChar(BinaryReader reader) => ReadNullableWholeNumber(reader)?.ToChar();

    public static void WriteDateTime(BinaryWriter writer, DateTime value) => WriteLong(writer, value.ToSchemaType());
    public static DateTime ReadDateTime(BinaryReader reader) => ReadLong(reader).ToDateTime();
    public static void WriteNullableDateTime(BinaryWriter writer, DateTime? value) => WriteNullableLong(writer, value?.ToSchemaType());
    public static DateTime? ReadNullableDateTime(BinaryReader reader) => ReadNullableLong(reader)?.ToDateTime();

    public static void WriteDateTimeOffset(BinaryWriter writer, DateTimeOffset value) => WriteLong(writer, value.ToSchemaType());
    public static DateTimeOffset ReadDateTimeOffset(BinaryReader reader) => ReadLong(reader).ToDateTimeOffset();
    public static void WriteNullableDateTimeOffset(BinaryWriter writer, DateTimeOffset? value) => WriteNullableLong(writer, value?.ToSchemaType());
    public static DateTimeOffset? ReadNullableDateTimeOffset(BinaryReader reader) => ReadNullableLong(reader)?.ToDateTimeOffset();

    public static void WriteTimeSpan(BinaryWriter writer, TimeSpan value) => WriteLong(writer, value.ToSchemaType());
    public static TimeSpan ReadTimeSpan(BinaryReader reader) => ReadLong(reader).ToTimeSpan();
    public static void WriteNullableTimeSpan(BinaryWriter writer, TimeSpan? value) => WriteNullableLong(writer, value?.ToSchemaType());
    public static TimeSpan? ReadNullableTimeSpan(BinaryReader reader) => ReadNullableLong(reader)?.ToTimeSpan();

    public static void WriteDateOnly(BinaryWriter writer, DateOnly value) => WriteInt(writer, value.ToSchemaType());
    public static DateOnly ReadDateOnly(BinaryReader reader) => ReadInt(reader).ToDateOnly();
    public static void WriteNullableDateOnly(BinaryWriter writer, DateOnly? value) => WriteNullableInt(writer, value?.ToSchemaType());
    public static DateOnly? ReadNullableDateOnly(BinaryReader reader) => ReadNullableInt(reader)?.ToDateOnly();

    public static void WriteTimeOnly(BinaryWriter writer, TimeOnly value) => WriteLong(writer, value.ToSchemaType());
    public static TimeOnly ReadTimeOnly(BinaryReader reader) => ReadLong(reader).ToTimeOnly();
    public static void WriteNullableTimeOnly(BinaryWriter writer, TimeOnly? value) => WriteNullableLong(writer, value?.ToSchemaType());
    public static TimeOnly? ReadNullableTimeOnly(BinaryReader reader) => ReadNullableLong(reader)?.ToTimeOnly();
}
