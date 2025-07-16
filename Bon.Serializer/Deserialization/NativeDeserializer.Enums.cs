namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    // Bookmark 212882721
    private enum NativeType
    {
        // For the values that exist in this enum and the enum below, keep their assigned integers the same.

        String = 1,
        NullableLong = 2,
        NullableULong = 3,
        NullableDouble = 4,
        NullableDecimal = 5,

        Byte = 6,
        SByte = 7,
        Short = 8,
        UShort = 9,
        Int = 10,
        UInt = 11,
        Long = 12,
        ULong = 13,
        Float = 14,
        Double = 15,
    }

    private enum BridgeType
    {
        String = 1,
        NullableLong = 2,
        NullableULong = 3,
        NullableDouble = 4,
        NullableDecimal = 5,
    }
}
