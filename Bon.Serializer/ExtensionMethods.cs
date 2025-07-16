using System.Text.Json.Nodes;

namespace Bon.Serializer;

internal static class ExtensionMethods
{
    public static Type? TryGetElementTypeOfArray(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (!type.IsGenericType || !type.IsAssignableTo(typeof(IEnumerable)))
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        if (innerTypes.Length != 1)
        {
            return null;
        }

        return innerTypes[0];
    }

    /// <summary>
    /// //2at
    /// </summary>
    public static Type UnwrapNullable(this Type type, out bool wasNullable)
    {
        wasNullable = type.IsNullableValueType();
        return wasNullable ? type.GetGenericArguments()[0] : type;
    }

    public static bool IsNullableValueType(this Type type) =>
        type.TryGetGenericTypeDefinition() == typeof(Nullable<>);

    public static bool IsNullable(this Type type) => !type.IsValueType || IsNullable(type);

    public static Type? TryGetGenericTypeDefinition(this Type type) =>
        type.IsGenericType ? type.GetGenericTypeDefinition() : null;

    public static (Type KeyType, Type ValueType)? TryGetInnerTypesOfDictionary(this Type type)
    {
        if (!type.IsGenericType)
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        if (innerTypes.Length != 2)
        {
            return null;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        if (!type.IsAssignableTo(typeof(IDictionary)) &&
            genericTypeDefinition != typeof(IReadOnlyDictionary<,>) &&
            genericTypeDefinition != typeof(IDictionary<,>))
        {
            return null;
        }

        return (innerTypes[0], innerTypes[1]);
    }

    public static Tuple2Type? TryGetTuple2Type(this Type type)
    {
        type = type.UnwrapNullable(out var isNullable);

        if (!type.IsGenericType)
        {
            return null;
        }

        if (type.TryGetGenericTypeDefinition() != typeof(ValueTuple<,>))
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        return new(innerTypes[0], innerTypes[1], isNullable);
    }

    public static Tuple3Type? TryGetTuple3Type(this Type type)
    {
        type = type.UnwrapNullable(out var isNullable);

        if (!type.IsGenericType)
        {
            return null;
        }

        if (type.TryGetGenericTypeDefinition() != typeof(ValueTuple<,,>))
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        return new(innerTypes[0], innerTypes[1], innerTypes[2], isNullable);
    }

    public static MethodInfo GetPrivateMethod(this IUseReflection instance, string methodName)
    {
        return
            instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new ArgumentException($"Method not found.", methodName);
    }

    public static MethodInfo GetPrivateStaticMethod(this Type type, string methodName)
    {
        return
            type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static) ??
            throw new ArgumentException($"Method not found.", methodName);
    }

    public static JsonNode GetPropertyValue(this JsonObject jsonObject, string propertyName)
    {
        if (jsonObject.TryGetPropertyValue(propertyName, out var jsonNode) && jsonNode is { })
        {
            return jsonNode;
        }

        throw new ArgumentException($"Property '{propertyName}' not found or null.", nameof(propertyName));
    }

    public static void AddMultiple<T>(this ref HashCode hashCode, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            hashCode.Add(value);
        }
    }

    public static void AddMultipleUnordered<T>(this ref HashCode hashCode, IEnumerable<T> values)
    {
        foreach (var hash in values.Select(x => x?.GetHashCode() ?? 0).Order())
        {
            hashCode.Add(hash);
        }
    }

    // The reason for using extension methods is that you can then use the null-conditional operator "?.".

    public static ulong? ToNullableULong(this char value) => value;
    public static long ToLong(this DateTime value) => value.ToUniversalTime().Ticks;
    public static long ToLong(this DateTimeOffset value) => value.UtcTicks;
    public static long ToLong(this TimeSpan value) => value.Ticks;
    public static int ToInt(this DateOnly value) => value.DayNumber;
    public static long ToLong(this TimeOnly value) => value.Ticks;
    public static byte[] ToByteArray(this Guid guid) => guid.ToByteArray();

    public static DateTime ToDateTime(this long value) => new(value, DateTimeKind.Utc);
    public static DateTimeOffset ToDateTimeOffset(this long value) => new(value, TimeSpan.Zero);
    public static TimeSpan ToTimeSpan(this long value) => new(value);
    public static DateOnly ToDateOnly(this int value) => DateOnly.FromDayNumber(value);
    public static DateOnly ToDateOnly(this long value) => DateOnly.FromDayNumber((int)value);
    public static TimeOnly ToTimeOnly(this long value) => new(value);
    public static Guid? ToGuid(this byte[]? value) => value?.Length == 16 ? new(value) : null;

    public static bool IsNullable(this SchemaType schemaType) => schemaType is
        SchemaType.WholeNumber or
        SchemaType.SignedWholeNumber or
        SchemaType.FractionalNumber or
        SchemaType.NullableRecord or
        SchemaType.NullableTuple2 or
        SchemaType.NullableTuple3 or
        SchemaType.Union or
        SchemaType.Array or
        SchemaType.Dictionary or
        SchemaType.String;
}
