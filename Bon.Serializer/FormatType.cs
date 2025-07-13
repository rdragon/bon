namespace Bon.Serializer;

internal enum FormatType : byte
{
    /// <summary>
    /// The header contains a format type, block ID and schema.
    /// </summary>
    Full = 254, // Why 254? See below.

    /// <summary>
    /// The header contains a format type and schema.
    /// </summary>
    WithoutBlockId = 253,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type byte.
    /// </summary>
    Byte = 252,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type sbyte.
    /// </summary>
    SByte = 251,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type short.
    /// </summary>
    Short = 250,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type ushort.
    /// </summary>
    UShort = 249,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type int.
    /// </summary>
    Int = 248,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type uint.
    /// </summary>
    UInt = 247,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type long.
    /// </summary>
    Long = 246,

    /// <summary>
    /// The header only contains a format type.
    /// The schema is implicit and of type ulong.
    /// </summary>
    ULong = 245,

    // We want to reserve the lower numbers for future use, e.g. for sending 1-byte messages.
    // Therefore, we start with the (almost) highest byte and go down.

    // We could have started with 255, but 255 might occur more often in binary data than the numbers directly below it.
    // We like to use values that don't occur often, so that we can abort early if we deserialize data that is not
    // in the expected format. Therefore we started with 254.
}
