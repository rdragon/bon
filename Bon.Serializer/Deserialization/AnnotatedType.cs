namespace Bon.Serializer.Deserialization;

/// <summary>
/// A type that is annotated with whether it is nullable.
/// For a value type the <see cref="IsNullable"/> property is determined by the type itself.
/// For a reference type the <see cref="IsNullable"/> property can be either <c>true</c> or <c>false</c>.
/// </summary>
internal readonly record struct AnnotatedType(Type Type, bool IsNullable)
{
    public bool IsNullableReferenceType => IsNullable && !Type.IsValueType;

    /// <summary>
    /// Replaces the type of <see cref="string"/> by the type of <see cref="StrinG"/> which is a struct type and therefore can
    /// be wrapped in a <see cref="Nullable{T}"/>.
    /// </summary>
    public Type ToVirtualType()
    {
        if (Type == typeof(string))
        {
            return IsNullable ? typeof(StrinG?) : typeof(StrinG);
        }

        return Type;
    }
}

internal readonly struct StrinG;