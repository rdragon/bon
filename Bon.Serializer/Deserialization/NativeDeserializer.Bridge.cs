namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    private Dictionary<(BridgeType, BridgeType), Func<Delegate, Delegate>>? _transformations;

    private Dictionary<(BridgeType, BridgeType), Func<Delegate, Delegate>> Transformations => _transformations ??= CreateTransformations();

    private Dictionary<(BridgeType, BridgeType), Func<Delegate, Delegate>> CreateTransformations()
    {
        var transformations = new Dictionary<(BridgeType, BridgeType), Func<Delegate, Delegate>>();

        Add<string?, long?>(StringToLong);
        Add<string?, ulong?>(StringToULong);
        Add<string?, double?>(StringToDouble);
        Add<string?, decimal?>(StringToDecimal);

        Add<long?, string?>(x => x?.ToString(Culture));
        Add<ulong?, string?>(x => x?.ToString(Culture));
        Add<double?, string?>(x => x?.ToString(Culture));
        Add<decimal?, string?>(x => x?.ToString(Culture));

        Add<long?, ulong?>(x => (ulong?)x);
        Add<long?, double?>(x => x);
        Add<long?, decimal?>(x => x);

        Add<ulong?, long?>(x => (long?)x);
        Add<ulong?, double?>(x => x);
        Add<ulong?, decimal?>(x => x);

        Add<double?, long?>(x => (long?)x);
        Add<double?, ulong?>(x => (ulong?)x);
        Add<double?, decimal?>(DoubleToDecimal);

        Add<decimal?, long?>(DecimalToLong);
        Add<decimal?, ulong?>(DecimalToULong);
        Add<decimal?, double?>(x => (double?)x);

        return transformations;

        void Add<T1, T2>(Func<T1, T2> func)
        {
            var outputType = GetBridgeType(typeof(T1));
            var targetType = GetBridgeType(typeof(T2));

            transformations[(outputType, targetType)] = (method) =>
            {
                var read = (Read<T1>)method;
                return (Read<T2>)(input => func(read(input)));
            };
        }
    }

    private static BridgeType GetBridgeType(Type type)
    {
        return type switch
        {
            _ when type == typeof(string) => BridgeType.String,
            _ when type == typeof(long?) => BridgeType.NullableLong,
            _ when type == typeof(ulong?) => BridgeType.NullableULong,
            _ when type == typeof(double?) => BridgeType.NullableDouble,
            _ when type == typeof(decimal?) => BridgeType.NullableDecimal,
        };
    }

    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    private Delegate CreateDeserializer_WalkBridge(Delegate method, BridgeType outputType, BridgeType targetType)
    {
        if (outputType == targetType)
        {
            return method;
        }

        return Transformations[(outputType, targetType)](method);
    }
}
