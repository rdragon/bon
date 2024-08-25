using System.Collections.ObjectModel;

namespace Bon.Serializer.Deserialization;

internal sealed class DefaultValueGetterFactory : IUseReflection
{
    /// <summary>
    /// Contains for each class, struct or interface with the BonObject attribute a method that can create a default instance of the type.
    /// These methods accept a single <see cref="BonInput"/> argument.
    /// However, that argument is not used.
    /// The reason for this argument is that for the code at bookmark 359877524 it is easier if these methods have the same signature
    /// as normal deserializer methods.
    /// This dictionary becomes read-only after the source generation context has run.
    /// </summary>
    private readonly Dictionary<Type, Delegate> _defaultValueGetters = [];

    private readonly ConcurrentDictionary<AnnotatedType, Delegate> _defaultValueGettersAnnotated = new();

    public void AddDefaultValueGetter(Type type, Delegate getDefaultValue) => _defaultValueGetters[type] = getDefaultValue;

    public Delegate GetDefaultValueGetter(Type type, bool isNullable)
    {
        var key = new AnnotatedType(type, isNullable);

        if (_defaultValueGettersAnnotated.TryGetValue(key, out var defaultValueGetter))
        {
            return defaultValueGetter;
        }

        var getter = LoadDefaultValueGetterNow(type, isNullable);

        return _defaultValueGettersAnnotated.TryAdd(key, getter) ? getter : _defaultValueGettersAnnotated[key];
    }

    public Read<T> GetDefaultValueGetter<T>(bool isNullable)
    {
        var key = new AnnotatedType(typeof(T), isNullable);

        if (_defaultValueGettersAnnotated.TryGetValue(key, out var defaultValueGetter))
        {
            return (Read<T>)defaultValueGetter;
        }

        var getter = LoadDefaultValueGetter<T>(isNullable);

        return _defaultValueGettersAnnotated.TryAdd(key, getter) ? getter : (Read<T>)_defaultValueGettersAnnotated[key];
    }

    private Delegate LoadDefaultValueGetterNow(Type type, bool isNullable)
    {
        return (Delegate)this.GetPrivateMethod(nameof(LoadDefaultValueGetter))
            .MakeGenericMethod(type)
            .Invoke(this, [isNullable])!;
    }

    private Read<T> LoadDefaultValueGetter<T>(bool isNullable)
    {
        if (isNullable)
        {
            return static _ => default!;
        }

        var type = typeof(T);

        if (type == typeof(string))
        {
            return static _ => (T)(object)"";
        }

        if (type.IsGenericType && TryLoadDefaultValueGetterForGenericType<T>(type) is Read<T> getter)
        {
            return getter;
        }

        if (type.TryGetElementTypeOfArray() is Type elementType)
        {
            var emptyArray = GetEmptyArray(elementType);

            return _ => (T)emptyArray;
        }

        if (_defaultValueGetters.TryGetValue(type, out var defaultValueGetter))
        {
            return (Read<T>)defaultValueGetter;
        }

        if (type.IsValueType)
        {
            return static _ => default!;
        }

        throw new InvalidOperationException($"Cannot handle '{type}'.");
    }

    private Read<T>? TryLoadDefaultValueGetterForGenericType<T>(Type type)
    {
        var genericTypeDefinition = type.GetGenericTypeDefinition();

        if (TryGetTypeToCreate(type, genericTypeDefinition) is Type typeToCreate)
        {
            return _ => (T)Activator.CreateInstance(typeToCreate)!;
        }

        if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
        {
            var typeArguments = type.GetGenericArguments();
            var emptyDictionary = GetEmptyReadOnlyDictionary(typeArguments[0], typeArguments[1]);

            return _ => (T)emptyDictionary;
        }

        if (genericTypeDefinition == typeof(ValueTuple<,>))
        {
            return LoadTuple2DefaultValueGetter<T>();
        }

        if (genericTypeDefinition == typeof(ValueTuple<,,>))
        {
            return LoadTuple3DefaultValueGetter<T>();
        }

        return null;
    }

    private static Type? TryGetTypeToCreate(Type type, Type genericTypeDefinition)
    {
        if (genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>))
        {
            return typeof(List<>).MakeGenericType(type.GetGenericArguments());
        }

        if (genericTypeDefinition == typeof(List<>))
        {
            return type;
        }

        if (genericTypeDefinition == typeof(Dictionary<,>))
        {
            return type;
        }

        if (genericTypeDefinition == typeof(IDictionary<,>))
        {
            return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
        }

        return null;
    }

    private Read<T> LoadTuple2DefaultValueGetter<T>()
    {
        var typeArguments = typeof(T).GetGenericArguments();

        return (Read<T>)this.GetPrivateMethod(nameof(LoadTuple2DefaultValueGetterNow))
            .MakeGenericMethod(typeArguments)
            .Invoke(this, [])!;
    }

    private Read<(T1, T2)> LoadTuple2DefaultValueGetterNow<T1, T2>()
    {
        var item1Getter = GetDefaultValueGetter<T1>(typeof(T1).IsNullable(false));
        var item2Getter = GetDefaultValueGetter<T2>(typeof(T2).IsNullable(false));

        return input => (item1Getter(input), item2Getter(input));
    }

    private Read<T> LoadTuple3DefaultValueGetter<T>()
    {
        var typeArguments = typeof(T).GetGenericArguments();

        return (Read<T>)this.GetPrivateMethod(nameof(LoadTuple3DefaultValueGetterNow))
            .MakeGenericMethod(typeArguments)
            .Invoke(this, [])!;
    }

    private Read<(T1, T2, T3)> LoadTuple3DefaultValueGetterNow<T1, T2, T3>()
    {
        var item1Getter = GetDefaultValueGetter<T1>(typeof(T1).IsNullable(false));
        var item2Getter = GetDefaultValueGetter<T2>(typeof(T2).IsNullable(false));
        var item3Getter = GetDefaultValueGetter<T3>(typeof(T3).IsNullable(false));

        return input => (item1Getter(input), item2Getter(input), item3Getter(input));
    }

    private static object GetEmptyArray(Type elementType) => typeof(DefaultValueGetterFactory)
        .GetPrivateStaticMethodInfo(nameof(GetEmptyArrayNow))
        .MakeGenericMethod(elementType)
        .Invoke(null, [])!;

    private static T[] GetEmptyArrayNow<T>() => [];

    private static object GetEmptyReadOnlyDictionary(Type keyType, Type valueType) => typeof(DefaultValueGetterFactory)
        .GetPrivateStaticMethodInfo(nameof(GetEmptyReadOnlyDictionaryNow))
        .MakeGenericMethod(keyType, valueType)
        .Invoke(null, [])!;

    private static ReadOnlyDictionary<TKey, TValue> GetEmptyReadOnlyDictionaryNow<TKey, TValue>() where TKey : notnull =>
        ReadOnlyDictionary<TKey, TValue>.Empty;

    public object LoadDefaultValue(Type type, bool isNullable)
    {
        return this.GetPrivateMethod(nameof(GetDefaultValue))
            .MakeGenericMethod(type)
            .Invoke(this, [isNullable])!;
    }

    private T GetDefaultValue<T>(bool isNullable) => GetDefaultValueGetter<T>(isNullable)(default);
}
