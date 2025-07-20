namespace Bon.SourceGeneration
{
    // Keeps this enum identical to the enum at bookmark 954139432.
    public enum SchemaType
    {
        /// <summary>
        /// A non-nullable struct.
        /// </summary>
        Record = 1,

        /// <summary>
        /// A class or nullable struct.
        /// </summary>
        NullableRecord = 2,

        /// <summary>
        /// An interface or abstract class.
        /// </summary>
        Union = 3,

        /// <summary>
        /// A <c>string</c>.
        /// </summary>
        String = 4,

        /// <summary>
        /// A <c>byte</c> representation that is 1 byte long.
        /// </summary>
        Byte = 5,

        /// <summary>
        /// An <c>sbyte</c> representation that is 1 byte long.
        /// </summary>
        SByte = 6,

        /// <summary>
        /// A <c>short</c> representation that is 2 bytes long.
        /// </summary>
        Short = 7,

        /// <summary>
        /// A <c>ushort</c> representation that is 2 bytes long.
        /// </summary>
        UShort = 8,

        /// <summary>
        /// An <c>int</c> representation that is 4 bytes long.
        /// </summary>
        Int = 9,

        /// <summary>
        /// A <c>uint</c> representation that is 4 bytes long.
        /// </summary>
        UInt = 10,

        /// <summary>
        /// An <c>long</c> representation that is 8 bytes long.
        /// </summary>
        Long = 11,

        /// <summary>
        /// A <c>ulong</c> representation that is 8 bytes long.
        /// </summary>
        ULong = 12,

        /// <summary>
        /// A <c>float</c> representation that is 4 bytes long.
        /// </summary>
        Float = 13,

        /// <summary>
        /// A <c>double</c> representation that is 8 bytes long.
        /// </summary>
        Double = 14,

        /// <summary>
        /// A <c>decimal?</c> representation that is 1 or 17 bytes long.
        /// </summary>
        NullableDecimal = 15,

        /// <summary>
        /// A <c>ulong?</c> representation that is 1, 2, 3, 5, or 9 bytes long.
        /// </summary>
        WholeNumber = 16,

        /// <summary>
        /// A <c>long?</c> representation that is 1, 2, 3, 5, or 9 bytes long.
        /// </summary>
        SignedWholeNumber = 17,

        /// <summary>
        /// A <c>double?</c> representation that is 1, 5, or 9 bytes long.
        /// </summary>
        FractionalNumber = 18,

        /// <summary>
        /// An array, list or enumerable.
        /// </summary>
        Array = 19,

        /// <summary>
        /// A dictionary.
        /// </summary>
        Dictionary = 24,

        /// <summary>
        /// A non-nullable value tuple with two elements.
        /// </summary>
        Tuple2 = 20,

        /// <summary>
        /// A nullable value tuple with two elements.
        /// </summary>
        NullableTuple2 = 21,

        /// <summary>
        /// A non-nullable value tuple with three elements.
        /// </summary>
        Tuple3 = 22,

        /// <summary>
        /// A nullable value tuple with three elements.
        /// </summary>
        NullableTuple3 = 23,
    }
}
