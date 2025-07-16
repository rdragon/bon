namespace Bon.Serializer.Deserialization;
//1at
partial class NativeDeserializer
{
    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    private static Delegate AddTransformation(Read<string?> read, BridgeType targetType)
    {
        return targetType switch
        {
            BridgeType.LongMaybe => (Read<long?>)(x => TryParseLong(read(x))),
        };

        //return true switch
        //{
        //    _ when targetType == typeof(long) => (Read<long>)(x => ParseLong(read(x))),
        //    _ when targetType == typeof(ulong) => (Read<ulong>)(x => ParseULong(read(x))),
        //    _ when targetType == typeof(double) => (Read<double>)(x => ParseDouble(read(x))),
        //    _ when targetType == typeof(decimal) => (Read<decimal>)(x => ParseDecimal(read(x))),

        //    _ when targetType == typeof(long?) => (Read<long?>)(x => TryParseLong(read(x))),
        //    _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => TryParseULong(read(x))),
        //    _ when targetType == typeof(double?) => (Read<double?>)(x => TryParseDouble(read(x))),
        //    _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => TryParseDecimal(read(x))),

        //    _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        //};
    }

    private static Delegate HandleLong(Read<long> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => (Read<string>)(x => read(x).ToString(Culture)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)read(x)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x)),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x)),

            _ when targetType == typeof(long?) => (Read<long?>)(x => read(x)),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => (ulong)read(x)),
            _ when targetType == typeof(double?) => (Read<double?>)(x => read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => read(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleULong(Read<ulong> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => (Read<string>)(x => read(x).ToString(Culture)),
            _ when targetType == typeof(long) => (Read<long>)(x => (long)read(x)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x)),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x)),

            _ when targetType == typeof(long?) => (Read<long?>)(x => (long)read(x)),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => read(x)),
            _ when targetType == typeof(double?) => (Read<double?>)(x => read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => read(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleDouble(Read<double> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => (Read<string>)(x => read(x).ToString(Culture)),
            _ when targetType == typeof(long) => (Read<long>)(x => (long)read(x)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)read(x)),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => DoubleToDecimal(read(x))),

            _ when targetType == typeof(long?) => (Read<long?>)(x => (long)read(x)),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => (ulong)read(x)),
            _ when targetType == typeof(double?) => (Read<double?>)(x => read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => DoubleToDecimal(read(x))),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleDecimal(Read<decimal> f, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => (Read<string>)(x => f(x).ToString(Culture)),
            _ when targetType == typeof(long) => (Read<long>)(x => DecimalToLong(f(x))),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => DecimalToULong(f(x))),
            _ when targetType == typeof(double) => (Read<double>)(x => (double)f(x)),

            _ when targetType == typeof(long?) => (Read<long?>)(x => DecimalToLong(f(x))),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => DecimalToULong(f(x))),
            _ when targetType == typeof(double?) => (Read<double?>)(x => (double?)f(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => f(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleNullableLong(Read<long?> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(long) => (Read<long>)(x => read(x) ?? 0),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)(read(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x) ?? 0),

            _ when targetType == typeof(string) => (Read<string?>)(x => read(x)?.ToString(Culture)),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => (ulong?)read(x)),
            _ when targetType == typeof(double?) => (Read<double?>)(x => read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => read(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleNullableULong(Read<ulong?> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(long) => (Read<long>)(x => (long)(read(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => read(x) ?? 0),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x) ?? 0),

            _ when targetType == typeof(string) => (Read<string?>)(x => read(x)?.ToString(Culture)),
            _ when targetType == typeof(long?) => (Read<long?>)(x => (long?)read(x)),
            _ when targetType == typeof(double?) => (Read<double?>)(x => read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => read(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleNullableDouble(Read<double?> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(long) => (Read<long>)(x => (long)(read(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)(read(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => DoubleToDecimal(read(x) ?? double.NaN)),

            _ when targetType == typeof(string) => (Read<string?>)(x => read(x)?.ToString(Culture)),
            _ when targetType == typeof(long?) => (Read<long?>)(x => (long?)read(x)),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => (ulong?)read(x)),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => DoubleToDecimal(read(x))),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate HandleNullableDecimal(Read<decimal?> f, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(long) => (Read<long>)(x => DecimalToLong(f(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => DecimalToULong(f(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => (double)(f(x) ?? 0)),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => f(x) ?? 0),

            _ when targetType == typeof(string) => (Read<string?>)(x => f(x)?.ToString(Culture)),
            _ when targetType == typeof(long?) => (Read<long?>)(x => DecimalToLong(f(x))),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => DecimalToULong(f(x))),
            _ when targetType == typeof(double?) => (Read<double?>)(x => (double?)f(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }
}
