// Bookmark 413211217
namespace Bon.Serializer.Schemas;

public enum SchemaType
{
    /// <summary>
    /// A struct.
    /// </summary>
    Record = 1,

    RecordMaybe = 24,

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
    DoubleMaybe = 23,//1at => double? (maar kost soms maar 5 ipv 9 bytes)
    Decimal = 17,//1at altijd nullable, dus => decimal?
    Guid = 18,//1at altijd nullable, dus => Guid?

    /// <summary>
    /// A value tuple with two elements.
    /// </summary>
    Tuple2 = 19,
    Tuple2Maybe = 25,

    /// <summary>
    /// A value tuple with three elements.
    /// </summary>
    Tuple3 = 20,
    Tuple3Maybe = 26,

    WholeNumber = 21, //1at => ulong? als tussenliggende waarde (voor nu)
    SignedWholeNumber = 22,//1at => long? als tussenliggende waarde (voor nu)
}
