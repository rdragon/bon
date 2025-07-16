namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    public static Delegate AddTransformation(Delegate method, OutputType outputType, TargetType targetType)
    {
        if (Enum.IsDefined((BridgeType)outputType))
        {
            return AddTransformation(method, (BridgeType)outputType, targetType);
        }

        return outputType switch
        {
            OutputType.Bool => Transform<bool, long?>(BridgeType.LongMaybe, f => x => f(x) ? 1 : 0),
        };

        Delegate Transform<T1, T2>(BridgeType type, Func<Read<T1>, Read<T2>> func)
        {
            return AddTransformation(func((Read<T1>)method), type, targetType);
        }

        //return sourceType switch
        //{
        //_ when sourceType == typeof(bool) => Transform<bool, long>(f => x => f(x) ? 1 : 0),
        //_ when sourceType == typeof(byte) => Transform<byte, ulong>(f => x => f(x)),
        //_ when sourceType == typeof(sbyte) => Transform<sbyte, long>(f => x => f(x)),
        //_ when sourceType == typeof(short) => Transform<short, long>(f => x => f(x)),
        //_ when sourceType == typeof(ushort) => Transform<ushort, ulong>(f => x => f(x)),
        //_ when sourceType == typeof(int) => Transform<int, long>(f => x => f(x)),
        //_ when sourceType == typeof(uint) => Transform<uint, ulong>(f => x => f(x)),
        //_ when sourceType == typeof(float) => Transform<float, double>(f => x => f(x)),
        //_ when sourceType == typeof(Guid) => Transform<Guid, string>(f => x => f(x).ToString()),

        //_ when sourceType == typeof(bool?) => Transform<bool?, long?>(f => x => BoolToLong(f(x))),
        //_ when sourceType == typeof(float?) => Transform<float?, double?>(f => x => f(x)),
        //_ when sourceType == typeof(Guid?) => Transform<Guid?, string?>(f => x => f(x)?.ToString()),
        //};
    }

    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    private static Delegate AddTransformation(Delegate method, BridgeType outputType, TargetType targetType)
    {
        if (Enum.IsDefined((BridgeType)targetType))
        {
            return AddTransformation(method, outputType, (BridgeType)targetType);
        }

        Read<T2> Transform<T1, T2>(BridgeType bridgeType, Func<Read<T1>, Read<T2>> func)
        {
            var reader = (Read<T1>)AddTransformation(method, outputType, bridgeType);

            return func(reader);
        }


        return targetType switch
        {
            TargetType.Bool => Transform<long?, bool>(BridgeType.LongMaybe, f => x => f(x) != 0),
        };

        //return true switch
        //{
        //    _ when targetType == typeof(bool) => Transform<long, bool>(f => x => f(x) != 0),
        //    _ when targetType == typeof(byte) => Transform<ulong, byte>(f => x => (byte)f(x)),
        //    _ when targetType == typeof(sbyte) => Transform<long, sbyte>(f => x => (sbyte)f(x)),
        //    _ when targetType == typeof(short) => Transform<long, short>(f => x => (short)f(x)),
        //    _ when targetType == typeof(ushort) => Transform<ulong, ushort>(f => x => (ushort)f(x)),
        //    _ when targetType == typeof(int) => Transform<long, int>(f => x => (int)f(x)),
        //    _ when targetType == typeof(uint) => Transform<ulong, uint>(f => x => (uint)f(x)),
        //    _ when targetType == typeof(float) => Transform<double, float>(f => x => (float)f(x)),
        //    _ when targetType == typeof(Guid) => Transform<string?, Guid>(f => x => Guid.TryParse(f(x), out var y) ? y : Guid.Empty),

        //    _ when targetType == typeof(bool?) => Transform<long?, bool?>(f => x => f(x) is long y ? y != 0 : null),
        //    _ when targetType == typeof(byte?) => Transform<ulong?, byte?>(f => x => (byte?)f(x)),
        //    _ when targetType == typeof(sbyte?) => Transform<long?, sbyte?>(f => x => (sbyte?)f(x)),
        //    _ when targetType == typeof(short?) => Transform<long?, short?>(f => x => (short?)f(x)),
        //    _ when targetType == typeof(ushort?) => Transform<ulong?, ushort?>(f => x => (ushort?)f(x)),
        //    _ when targetType == typeof(int?) => Transform<long?, int?>(f => x => (int?)f(x)),
        //    _ when targetType == typeof(uint?) => Transform<ulong?, uint?>(f => x => (uint?)f(x)),
        //    _ when targetType == typeof(float?) => Transform<double?, float?>(f => x => (float?)f(x)),
        //    _ when targetType == typeof(Guid?) => Transform<string?, Guid?>(f => x => Guid.TryParse(f(x), out var y) ? y : null),

        //    // Bookmark 659516266 (char serialization)

        //    _ when targetType == typeof(char) => Transform<ulong, char>(f => x => (char)f(x)),
        //    _ when targetType == typeof(DateTime) => Transform<long, DateTime>(f => x => f(x).ToDateTime()),
        //    _ when targetType == typeof(DateTimeOffset) => Transform<long, DateTimeOffset>(f => x => f(x).ToDateTimeOffset()),
        //    _ when targetType == typeof(TimeSpan) => Transform<long, TimeSpan>(f => x => f(x).ToTimeSpan()),
        //    _ when targetType == typeof(DateOnly) => Transform<long, DateOnly>(f => x => f(x).ToDateOnly()),
        //    _ when targetType == typeof(TimeOnly) => Transform<long, TimeOnly>(f => x => f(x).ToTimeOnly()),

        //    _ when targetType == typeof(char?) => Transform<ulong?, char?>(f => x => (char?)f(x)),
        //    _ when targetType == typeof(DateTime?) => Transform<long?, DateTime?>(f => x => f(x)?.ToDateTime()),
        //    _ when targetType == typeof(DateTimeOffset?) => Transform<long?, DateTimeOffset?>(f => x => f(x)?.ToDateTimeOffset()),
        //    _ when targetType == typeof(TimeSpan?) => Transform<long?, TimeSpan?>(f => x => f(x)?.ToTimeSpan()),
        //    _ when targetType == typeof(DateOnly?) => Transform<long?, DateOnly?>(f => x => f(x)?.ToDateOnly()),
        //    _ when targetType == typeof(TimeOnly?) => Transform<long?, TimeOnly?>(f => x => f(x)?.ToTimeOnly()),

        //    _ => null,
        //};
    }

    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    private static Delegate AddTransformation(Delegate method, BridgeType outputType, BridgeType targetType)
    {
        if (outputType == targetType)
        {
            return method;
        }

        return outputType switch
        {
            BridgeType.String => AddTransformation((Read<string?>)method, targetType),
        };
    }
}
