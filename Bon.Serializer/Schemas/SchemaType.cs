// Bookmark 413211217
namespace Bon.Serializer.Schemas;

public enum SchemaType
{
    /// <summary>
    /// A class or struct.
    /// </summary>
    Record = 1,

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
    Bool = 6,
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
    Decimal = 17,
    Guid = 18,

    /// <summary>
    /// A value tuple with two elements.
    /// </summary>
    Tuple2 = 19,

    /// <summary>
    /// A value tuple with three elements.
    /// </summary>
    Tuple3 = 20,

    WholeNumber = 21,
    SignedWholeNumber = 22,
}
