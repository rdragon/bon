namespace Bon.Serializer;

/// <summary>
/// Indicates that the type can be serialized and deserialized by the Bon serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum)]
public sealed class BonObjectAttribute : Attribute { }

/// <summary>
/// Indicates that the member should be serialized and deserialized by the Bon serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class BonMemberAttribute : Attribute
{
    /// <param name="id">
    /// The ID of the member.
    /// This ID is used during serialization and deserialization instead of the name.
    /// All members should have a unique ID within their class or struct.
    /// </param>
    public BonMemberAttribute(int id)
    {
        BonHelper.Ignore(id);
    }
}

/// <summary>
/// Indicates that the member should be ignored by the Bon serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BonIgnoreAttribute : Attribute { }

/// <summary>
/// Indicates that a certain class or struct implements the interface or abstract class.
/// Values that have this class or struct as run-time type and the interface or abstract class as compile-time type can be serialized and
/// deserialized by the Bon serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
public sealed class BonIncludeAttribute : Attribute
{
    /// <param name="id">
    /// The ID of the implementation.
    /// This ID is used during deserialization to determine which run-time type to create.
    /// It is included as variable-width integer in the serializer output every time an object of this type is serialized.
    /// All members should have a unique ID.
    /// The IDs do not have to be unique across different types.
    /// </param>
    /// <param name="type">
    /// A type implementing the interface or abstract class.
    /// </param>
    public BonIncludeAttribute(int id, Type type)
    {
        BonHelper.Ignore((id, type));
    }
}

/// <summary>
/// Indicates that the Bon source generator will add the generated code to this partial class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class BonSerializerContextAttribute : Attribute { }

/// <summary>
/// Indicates that some IDs are reserved and cannot be used as member IDs.
/// For example, these are IDs of deleted members.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class BonReservedMembersAttribute : Attribute
{
    /// <param name="ids">
    /// One or more IDs that are reserved and cannot be used as member IDs.
    /// For example, these are IDs of deleted members.
    /// </param>
    public BonReservedMembersAttribute(params int[] ids)
    {
        BonHelper.Ignore(ids);
    }
}
