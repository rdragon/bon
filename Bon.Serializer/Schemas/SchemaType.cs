namespace Bon.Serializer.Schemas;

// Keeps this enum identical to the enum at bookmark 954139432.
public enum SchemaType
{
    /// <summary>
    /// A struct.
    /// </summary>
    Record = 1,

    NullableRecord = 24,

    /// <summary>
    /// An interface or abstract class.
    /// </summary>
    Union = 2,

    /// <summary>
    /// An array, list or enumerable.
    /// </summary>
    Array = 3,

    /// <summary>
    /// A dictionary.
    /// </summary>
    Dictionary = 4,

    String = 5,
    Byte = 7,
    SByte = 8,
    Short = 9,
    UShort = 10,
    Int = 11,
    UInt = 12,
    Long = 13,
    ULong = 14,
    Float = 15,
    Double = 16,
    NullableDecimal = 17,
    WholeNumber = 21,
    SignedWholeNumber = 22,
    FractionalNumber = 23,

    /// <summary>
    /// A value tuple with two elements.
    /// </summary>
    Tuple2 = 19,
    NullableTuple2 = 25,

    /// <summary>
    /// A value tuple with three elements.
    /// </summary>
    Tuple3 = 20,
    NullableTuple3 = 26,
}
