namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    internal enum OutputType
    {
        String,
        Bool,
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,

        ULongMaybe,
        LongMaybe,
        DoubleMaybe,
        DecimalMaybe,
        GuidMaybe,
    }

    internal enum BridgeType
    {
        String,
        LongMaybe,
        ULongMaybe,
        DoubleMaybe,
        DecimalMaybe,
    }

    internal enum TargetType
    {
        String,
        Bool,
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Guid,

        StringMaybe,
        BoolMaybe,
        ByteMaybe,
        SByteMaybe,
        ShortMaybe,
        UShortMaybe,
        IntMaybe,
        UIntMaybe,
        LongMaybe,
        ULongMaybe,
        FloatMaybe,
        DoubleMaybe,
        GuidMaybe,

        Char,
        DateTime,
        DateTimeOffset,
        TimeSpan,
        DateOnly,
        TimeOnly,

        CharMaybe,
        DateTimeMaybe,
        DateTimeOffsetMaybe,
        TimeSpanMaybe,
        DateOnlyMaybe,
        TimeOnlyMaybe,
    }
}
