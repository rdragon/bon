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

    public static Type UnwrapNullable(this Type type) => type.TryGetInnerTypeOfNullable() ?? type;

    public static Type? TryGetInnerTypeOfNullable(this Type type)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
            return null;
        }

        return type.GetGenericArguments()[0];
    }

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

    public static (Type Item1Type, Type Item2Type)? TryGetInnerTypesOfTuple2(this Type type)
    {
        type = type.UnwrapNullable();

        if (!type.IsGenericType)
        {
            return null;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        if (genericTypeDefinition != typeof(ValueTuple<,>))
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        return (innerTypes[0], innerTypes[1]);
    }

    public static (Type Item1Type, Type Item2Type, Type Item3Type)? TryGetInnerTypesOfTuple3(this Type type)
    {
        type = type.UnwrapNullable();

        if (!type.IsGenericType)
        {
            return null;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        if (genericTypeDefinition != typeof(ValueTuple<,,>))
        {
            return null;
        }

        var innerTypes = type.GetGenericArguments();

        return (innerTypes[0], innerTypes[1], innerTypes[2]);
    }

    public static MethodInfo GetPrivateMethod(this IUseReflection instance, string methodName)
    {
        return
            instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new ArgumentException($"Method not found.", methodName);
    }

    public static MethodInfo GetPrivateStaticMethodInfo(this Type type, string methodName)
    {
        return
            type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static) ??
            throw new ArgumentException($"Method not found.", methodName);
    }

    public static bool IsNullable(this Type type, bool seeReferenceTypeAsNullable)
    {
        return type.IsValueType ? Nullable.GetUnderlyingType(type) is { } : seeReferenceTypeAsNullable;
    }

    public static JsonNode GetPropertyValue(this JsonObject jsonObject, string propertyName)
    {
        if (jsonObject.TryGetPropertyValue(propertyName, out var jsonNode) && jsonNode is { })
        {
            return jsonNode;
        }

        throw new ArgumentException($"Property '{propertyName}' not found or null.", nameof(propertyName));
    }

    public static Type ToNullable(this Type type)
    {
        if (type.IsNullable(true))
        {
            return type;
        }

        return typeof(Nullable<>).MakeGenericType(type);
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
    // Bookmark 659516266 (char serialization)

    public static ulong ToSchemaType(this char value) => value;
    public static long ToSchemaType(this DateTime value) => value.ToUniversalTime().Ticks;
    public static long ToSchemaType(this DateTimeOffset value) => value.UtcTicks;
    public static long ToSchemaType(this TimeSpan value) => value.Ticks;
    public static int ToSchemaType(this DateOnly value) => value.DayNumber;
    public static long ToSchemaType(this TimeOnly value) => value.Ticks;

    public static char ToChar(this ulong value) => (char)value;
    public static DateTime ToDateTime(this long value) => new(value, DateTimeKind.Utc);
    public static DateTimeOffset ToDateTimeOffset(this long value) => new(value, TimeSpan.Zero);
    public static TimeSpan ToTimeSpan(this long value) => new(value);
    public static DateOnly ToDateOnly(this int value) => DateOnly.FromDayNumber(value);
    public static DateOnly ToDateOnly(this long value) => DateOnly.FromDayNumber((int)value);
    public static TimeOnly ToTimeOnly(this long value) => new(value);
}
