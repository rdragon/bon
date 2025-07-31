namespace Bon.Serializer;

internal sealed record class Tuple2Type(Type Item1Type, Type Item2Type, bool IsNullable);

internal sealed record class Tuple3Type(Type Item1Type, Type Item2Type, Type Item3Type, bool IsNullable);

/// <summary>
/// Wraps the type of a class to denote the non-nullable version of the type.
/// This is needed because non-nullable reference types are not supported during runtime.
/// </summary>
public readonly record struct NotNull<T>(T Value);
