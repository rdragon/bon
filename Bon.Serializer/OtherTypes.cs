namespace Bon.Serializer;

internal sealed record class Tuple2Type(Type Item1Type, Type Item2Type, bool IsNullable);
internal sealed record class Tuple3Type(Type Item1Type, Type Item2Type, Type Item3Type, bool IsNullable);

public readonly record struct NotNull<T>(T Value);//2at