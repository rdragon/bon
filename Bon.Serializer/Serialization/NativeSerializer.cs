namespace Bon.Serializer.Serialization;

// This class is directly used by the source generated code.
public static class NativeSerializer
{
    // It's good to have methods for both the nullable and non-nullable case (except for reference types).
    // The main reason is that you can't cast an Action<int?> to an Action<int>, for example.

    // Bookmark 659516266 (native serialization)
    // For every type that can be found at bookmark 293228595 there should be two methods defined here (one reader and one writer).
    // The naming of these methods is important, it should follow the type name.
    public static void WriteString(BinaryWriter writer, string? value) => StringSerializer.WriteString(writer, value);
    public static void WriteBool(BinaryWriter writer, bool value) => writer.Write(value);
    public static void WriteByte(BinaryWriter writer, byte value) => writer.Write(value);
    public static void WriteSByte(BinaryWriter writer, sbyte value) => writer.Write(value);
    public static void WriteShort(BinaryWriter writer, short value) => writer.Write(value);
    public static void WriteUShort(BinaryWriter writer, ushort value) => writer.Write(value);
    public static void WriteInt(BinaryWriter writer, int value) => writer.Write(value);
    public static void WriteUInt(BinaryWriter writer, uint value) => writer.Write(value);
    public static void WriteLong(BinaryWriter writer, long value) => writer.Write(value);
    public static void WriteULong(BinaryWriter writer, ulong value) => writer.Write(value);
    public static void WriteFloat(BinaryWriter writer, float value) => writer.Write(value);
    public static void WriteDouble(BinaryWriter writer, double value) => writer.Write(value);
    public static void WriteDecimal(BinaryWriter writer, decimal value) => NativeWriter.WriteDecimal(writer, value);
    public static void WriteGuid(BinaryWriter writer, Guid value) => NativeWriter.WriteGuid(writer, value);
    public static void WriteChar(BinaryWriter writer, char value) => WholeNumberSerializer.Write(writer, value.ToNullableULong());
    public static void WriteDateTime(BinaryWriter writer, DateTime value) => writer.Write(value.ToLong());
    public static void WriteDateTimeOffset(BinaryWriter writer, DateTimeOffset value) => writer.Write(value.ToLong());
    public static void WriteTimeSpan(BinaryWriter writer, TimeSpan value) => writer.Write(value.ToLong());
    public static void WriteDateOnly(BinaryWriter writer, DateOnly value) => writer.Write(value.ToInt());
    public static void WriteTimeOnly(BinaryWriter writer, TimeOnly value) => writer.Write(value.ToLong());

    public static void WriteNullableBool(BinaryWriter writer, bool? value) => NativeWriter.WriteBool(writer, value);
    public static void WriteNullableByte(BinaryWriter writer, byte? value) => WholeNumberSerializer.Write(writer, value);
    public static void WriteNullableSByte(BinaryWriter writer, sbyte? value) => WholeNumberSerializer.WriteSigned(writer, value);
    public static void WriteNullableShort(BinaryWriter writer, short? value) => WholeNumberSerializer.WriteSigned(writer, value);
    public static void WriteNullableUShort(BinaryWriter writer, ushort? value) => WholeNumberSerializer.Write(writer, value);
    public static void WriteNullableInt(BinaryWriter writer, int? value) => WholeNumberSerializer.WriteSigned(writer, value);
    public static void WriteNullableUInt(BinaryWriter writer, uint? value) => WholeNumberSerializer.Write(writer, value);
    public static void WriteNullableLong(BinaryWriter writer, long? value) => WholeNumberSerializer.WriteSigned(writer, value);
    public static void WriteNullableULong(BinaryWriter writer, ulong? value) => WholeNumberSerializer.Write(writer, value);
    public static void WriteNullableFloat(BinaryWriter writer, float? value) => FractionalNumberSerializer.Write(writer, value);
    public static void WriteNullableDouble(BinaryWriter writer, double? value) => FractionalNumberSerializer.Write(writer, value);
    public static void WriteNullableDecimal(BinaryWriter writer, decimal? value) => NativeWriter.WriteDecimal(writer, value);
    public static void WriteNullableGuid(BinaryWriter writer, Guid? value) => NativeWriter.WriteGuid(writer, value);
    public static void WriteNullableChar(BinaryWriter writer, char? value) => WholeNumberSerializer.Write(writer, value?.ToNullableULong());
    public static void WriteNullableDateTime(BinaryWriter writer, DateTime? value) => WholeNumberSerializer.WriteSigned(writer, value?.ToLong());
    public static void WriteNullableDateTimeOffset(BinaryWriter writer, DateTimeOffset? value) => WholeNumberSerializer.WriteSigned(writer, value?.ToLong());
    public static void WriteNullableTimeSpan(BinaryWriter writer, TimeSpan? value) => WholeNumberSerializer.WriteSigned(writer, value?.ToLong());
    public static void WriteNullableDateOnly(BinaryWriter writer, DateOnly? value) => WholeNumberSerializer.WriteSigned(writer, value?.ToInt());
    public static void WriteNullableTimeOnly(BinaryWriter writer, TimeOnly? value) => WholeNumberSerializer.WriteSigned(writer, value?.ToLong());

    public static string? ReadString(BinaryReader reader) => StringDeserializer.ReadString(reader);
    public static bool ReadBool(BinaryReader reader) => NativeReader.ReadBool(reader) ?? default;
    public static byte ReadByte(BinaryReader reader) => reader.ReadByte();
    public static sbyte ReadSByte(BinaryReader reader) => reader.ReadSByte();
    public static short ReadShort(BinaryReader reader) => reader.ReadInt16();
    public static ushort ReadUShort(BinaryReader reader) => reader.ReadUInt16();
    public static int ReadInt(BinaryReader reader) => reader.ReadInt32();
    public static uint ReadUInt(BinaryReader reader) => reader.ReadUInt32();
    public static long ReadLong(BinaryReader reader) => reader.ReadInt64();
    public static ulong ReadULong(BinaryReader reader) => reader.ReadUInt64();
    public static float ReadFloat(BinaryReader reader) => reader.ReadSingle();
    public static double ReadDouble(BinaryReader reader) => reader.ReadDouble();
    public static decimal ReadDecimal(BinaryReader reader) => NativeReader.ReadDecimal(reader) ?? default;
    public static Guid ReadGuid(BinaryReader reader) => NativeReader.ReadGuid(reader) ?? default;
    public static char ReadChar(BinaryReader reader) => (char?)WholeNumberSerializer.Read(reader) ?? default;
    public static DateTime ReadDateTime(BinaryReader reader) => reader.ReadInt64().ToDateTime();
    public static DateTimeOffset ReadDateTimeOffset(BinaryReader reader) => reader.ReadInt64().ToDateTimeOffset();
    public static TimeSpan ReadTimeSpan(BinaryReader reader) => reader.ReadInt64().ToTimeSpan();
    public static DateOnly ReadDateOnly(BinaryReader reader) => ReadInt(reader).ToDateOnly();
    public static TimeOnly ReadTimeOnly(BinaryReader reader) => reader.ReadInt64().ToTimeOnly();

    public static bool? ReadNullableBool(BinaryReader reader) => NativeReader.ReadBool(reader);
    public static byte? ReadNullableByte(BinaryReader reader) => (byte?)WholeNumberSerializer.Read(reader);
    public static sbyte? ReadNullableSByte(BinaryReader reader) => (sbyte?)WholeNumberSerializer.ReadSigned(reader);
    public static short? ReadNullableShort(BinaryReader reader) => (short?)WholeNumberSerializer.ReadSigned(reader);
    public static ushort? ReadNullableUShort(BinaryReader reader) => (ushort?)WholeNumberSerializer.Read(reader);
    public static int? ReadNullableInt(BinaryReader reader) => (int?)WholeNumberSerializer.ReadSigned(reader);
    public static uint? ReadNullableUInt(BinaryReader reader) => (uint?)WholeNumberSerializer.Read(reader);
    public static long? ReadNullableLong(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader);
    public static ulong? ReadNullableULong(BinaryReader reader) => WholeNumberSerializer.Read(reader);
    public static float? ReadNullableFloat(BinaryReader reader) => (float?)FractionalNumberSerializer.Read(reader);
    public static double? ReadNullableDouble(BinaryReader reader) => FractionalNumberSerializer.Read(reader);
    public static decimal? ReadNullableDecimal(BinaryReader reader) => NativeReader.ReadDecimal(reader);
    public static Guid? ReadNullableGuid(BinaryReader reader) => NativeReader.ReadGuid(reader);
    public static char? ReadNullableChar(BinaryReader reader) => (char?)WholeNumberSerializer.Read(reader);
    public static DateTime? ReadNullableDateTime(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader)?.ToDateTime();
    public static DateTimeOffset? ReadNullableDateTimeOffset(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader)?.ToDateTimeOffset();
    public static TimeSpan? ReadNullableTimeSpan(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader)?.ToTimeSpan();
    public static DateOnly? ReadNullableDateOnly(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader)?.ToDateOnly();
    public static TimeOnly? ReadNullableTimeOnly(BinaryReader reader) => WholeNumberSerializer.ReadSigned(reader)?.ToTimeOnly();
}
