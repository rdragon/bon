using System.Globalization;

namespace Bon.Serializer.Deserialization.Native;

internal static class NativeDeserializer
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Returns a deserializer that reads a value of type <paramref name="sourceType"/> and returns a value of
    /// type <paramref name="annotatedTargetType"/>.
    /// </summary>
    /// <param name="readSourceType">A method that reads a value of type <paramref name="sourceType"/>.</param>
    public static Delegate CreateNativeDeserializer(Delegate readSourceType, Type sourceType, AnnotatedType annotatedTargetType)
    {
        var (targetType, targetIsNullable) = annotatedTargetType;

        if (sourceType == targetType && !(targetType == typeof(string) && !targetIsNullable))
        {
            return readSourceType;
        }

        if ((TryChangeSourceType(readSourceType, sourceType, annotatedTargetType) ??
            TryChangeTargetType(readSourceType, sourceType, annotatedTargetType)) is Delegate result)
        {
            return result;
        }

        return CreateNativeDeserializerNow(readSourceType, sourceType, annotatedTargetType);
    }

    private static Delegate CreateNativeDeserializerNow(Delegate readSourceType, Type sourceType, AnnotatedType annotatedTargetType)
    {
        var (targetType, _) = annotatedTargetType;
        var virtualType = annotatedTargetType.ToVirtualType();

        return true switch
        {
            _ when sourceType == typeof(string) => HandleString((Read<string?>)readSourceType, targetType),
            _ when sourceType == typeof(long) => HandleLong((Read<long>)readSourceType, targetType),
            _ when sourceType == typeof(ulong) => HandleULong((Read<ulong>)readSourceType, targetType),
            _ when sourceType == typeof(double) => HandleDouble((Read<double>)readSourceType, targetType),
            _ when sourceType == typeof(decimal) => HandleDecimal((Read<decimal>)readSourceType, targetType),
            _ when sourceType == typeof(long?) => HandleNullableLong((Read<long?>)readSourceType, virtualType),
            _ when sourceType == typeof(ulong?) => HandleNullableULong((Read<ulong?>)readSourceType, virtualType),
            _ when sourceType == typeof(double?) => HandleNullableDouble((Read<double?>)readSourceType, virtualType),
            _ when sourceType == typeof(decimal?) => HandleNullableDecimal((Read<decimal?>)readSourceType, virtualType),
            _ => throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null),
        };
    }

    private static Delegate HandleString(Read<string?> read, Type targetType)
    {
        return true switch
        {
            _ when targetType == typeof(string) => (Read<string>)(x => read(x) ?? ""),
            _ when targetType == typeof(long) => (Read<long>)(x => ParseLong(read(x))),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => ParseULong(read(x))),
            _ when targetType == typeof(double) => (Read<double>)(x => ParseDouble(read(x))),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => ParseDecimal(read(x))),

            _ when targetType == typeof(long?) => (Read<long?>)(x => TryParseLong(read(x))),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => TryParseULong(read(x))),
            _ when targetType == typeof(double?) => (Read<double?>)(x => TryParseDouble(read(x))),
            _ when targetType == typeof(decimal?) => (Read<decimal?>)(x => TryParseDecimal(read(x))),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
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
            _ when targetType == typeof(StrinG) => (Read<string>)(x => read(x)?.ToString(Culture) ?? ""),
            _ when targetType == typeof(long) => (Read<long>)(x => read(x) ?? 0),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)(read(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x) ?? 0),

            _ when targetType == typeof(StrinG?) => (Read<string?>)(x => read(x)?.ToString(Culture)),
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
            _ when targetType == typeof(StrinG) => (Read<string>)(x => read(x)?.ToString(Culture) ?? ""),
            _ when targetType == typeof(long) => (Read<long>)(x => (long)(read(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => read(x) ?? 0),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => read(x) ?? 0),

            _ when targetType == typeof(StrinG?) => (Read<string?>)(x => read(x)?.ToString(Culture)),
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
            _ when targetType == typeof(StrinG) => (Read<string>)(x => read(x)?.ToString(Culture) ?? ""),
            _ when targetType == typeof(long) => (Read<long>)(x => (long)(read(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => (ulong)(read(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => read(x) ?? double.NaN),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => DoubleToDecimal(read(x) ?? double.NaN)),

            _ when targetType == typeof(StrinG?) => (Read<string?>)(x => read(x)?.ToString(Culture)),
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
            _ when targetType == typeof(StrinG) => (Read<string>)(x => f(x)?.ToString(Culture) ?? ""),
            _ when targetType == typeof(long) => (Read<long>)(x => DecimalToLong(f(x) ?? 0)),
            _ when targetType == typeof(ulong) => (Read<ulong>)(x => DecimalToULong(f(x) ?? 0)),
            _ when targetType == typeof(double) => (Read<double>)(x => (double)(f(x) ?? 0)),
            _ when targetType == typeof(decimal) => (Read<decimal>)(x => f(x) ?? 0),

            _ when targetType == typeof(StrinG?) => (Read<string?>)(x => f(x)?.ToString(Culture)),
            _ when targetType == typeof(long?) => (Read<long?>)(x => DecimalToLong(f(x))),
            _ when targetType == typeof(ulong?) => (Read<ulong?>)(x => DecimalToULong(f(x))),
            _ when targetType == typeof(double?) => (Read<double?>)(x => (double?)f(x)),

            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null),
        };
    }

    private static Delegate? TryChangeSourceType(Delegate readSourceType, Type sourceType, AnnotatedType annotatedTargetType)
    {
        Delegate? Transform<T1, T2>(Func<Read<T1>, Read<T2>> func)
        {
            var read = (Read<T1>)readSourceType;

            return CreateNativeDeserializer(func(read), typeof(T2), annotatedTargetType);
        }

        return true switch
        {
            _ when sourceType == typeof(bool) => Transform<bool, long>(f => x => f(x) ? 1 : 0),
            _ when sourceType == typeof(byte) => Transform<byte, ulong>(f => x => f(x)),
            _ when sourceType == typeof(sbyte) => Transform<sbyte, long>(f => x => f(x)),
            _ when sourceType == typeof(short) => Transform<short, long>(f => x => f(x)),
            _ when sourceType == typeof(ushort) => Transform<ushort, ulong>(f => x => f(x)),
            _ when sourceType == typeof(int) => Transform<int, long>(f => x => f(x)),
            _ when sourceType == typeof(uint) => Transform<uint, ulong>(f => x => f(x)),
            _ when sourceType == typeof(float) => Transform<float, double>(f => x => f(x)),
            _ when sourceType == typeof(Guid) => Transform<Guid, string>(f => x => f(x).ToString()),

            _ when sourceType == typeof(bool?) => Transform<bool?, long?>(f => x => BoolToLong(f(x))),
            _ when sourceType == typeof(float?) => Transform<float?, double?>(f => x => f(x)),
            _ when sourceType == typeof(Guid?) => Transform<Guid?, string?>(f => x => f(x)?.ToString()),

            _ => null,
        };
    }

    private static Delegate? TryChangeTargetType(Delegate readSourceType, Type sourceType, AnnotatedType annotatedTargetType)
    {
        var (targetType, _) = annotatedTargetType;

        Read<T2> Transform<T1, T2>(Func<Read<T1>, Read<T2>> func)
        {
            var read = (Read<T1>)CreateNativeDeserializer(readSourceType, sourceType, new(typeof(T1), typeof(T1).IsNullable(false)));

            return func(read);
        }

        return true switch
        {
            _ when targetType == typeof(bool) => Transform<long, bool>(f => x => f(x) != 0),
            _ when targetType == typeof(byte) => Transform<ulong, byte>(f => x => (byte)f(x)),
            _ when targetType == typeof(sbyte) => Transform<long, sbyte>(f => x => (sbyte)f(x)),
            _ when targetType == typeof(short) => Transform<long, short>(f => x => (short)f(x)),
            _ when targetType == typeof(ushort) => Transform<ulong, ushort>(f => x => (ushort)f(x)),
            _ when targetType == typeof(int) => Transform<long, int>(f => x => (int)f(x)),
            _ when targetType == typeof(uint) => Transform<ulong, uint>(f => x => (uint)f(x)),
            _ when targetType == typeof(float) => Transform<double, float>(f => x => (float)f(x)),
            _ when targetType == typeof(Guid) => Transform<string?, Guid>(f => x => Guid.TryParse(f(x), out var y) ? y : Guid.Empty),

            _ when targetType == typeof(bool?) => Transform<long?, bool?>(f => x => f(x) is long y ? y != 0 : null),
            _ when targetType == typeof(byte?) => Transform<ulong?, byte?>(f => x => (byte?)f(x)),
            _ when targetType == typeof(sbyte?) => Transform<long?, sbyte?>(f => x => (sbyte?)f(x)),
            _ when targetType == typeof(short?) => Transform<long?, short?>(f => x => (short?)f(x)),
            _ when targetType == typeof(ushort?) => Transform<ulong?, ushort?>(f => x => (ushort?)f(x)),
            _ when targetType == typeof(int?) => Transform<long?, int?>(f => x => (int?)f(x)),
            _ when targetType == typeof(uint?) => Transform<ulong?, uint?>(f => x => (uint?)f(x)),
            _ when targetType == typeof(float?) => Transform<double?, float?>(f => x => (float?)f(x)),
            _ when targetType == typeof(Guid?) => Transform<string?, Guid?>(f => x => Guid.TryParse(f(x), out var y) ? y : null),

            // Bookmark 659516266 (char serialization)

            _ when targetType == typeof(char) => Transform<ulong, char>(f => x => (char)f(x)),
            _ when targetType == typeof(DateTime) => Transform<long, DateTime>(f => x => f(x).ToDateTime()),
            _ when targetType == typeof(DateTimeOffset) => Transform<long, DateTimeOffset>(f => x => f(x).ToDateTimeOffset()),
            _ when targetType == typeof(TimeSpan) => Transform<long, TimeSpan>(f => x => f(x).ToTimeSpan()),
            _ when targetType == typeof(DateOnly) => Transform<long, DateOnly>(f => x => f(x).ToDateOnly()),
            _ when targetType == typeof(TimeOnly) => Transform<long, TimeOnly>(f => x => f(x).ToTimeOnly()),

            _ when targetType == typeof(char?) => Transform<ulong?, char?>(f => x => (char?)f(x)),
            _ when targetType == typeof(DateTime?) => Transform<long?, DateTime?>(f => x => f(x)?.ToDateTime()),
            _ when targetType == typeof(DateTimeOffset?) => Transform<long?, DateTimeOffset?>(f => x => f(x)?.ToDateTimeOffset()),
            _ when targetType == typeof(TimeSpan?) => Transform<long?, TimeSpan?>(f => x => f(x)?.ToTimeSpan()),
            _ when targetType == typeof(DateOnly?) => Transform<long?, DateOnly?>(f => x => f(x)?.ToDateOnly()),
            _ when targetType == typeof(TimeOnly?) => Transform<long?, TimeOnly?>(f => x => f(x)?.ToTimeOnly()),

            _ => null,
        };
    }

    private static decimal DoubleToDecimal(double y)
    {
        if (double.IsNaN(y))
        {
            return 0;
        }

        const double limit = (double)decimal.MaxValue;

        if (y <= -limit)
        {
            return decimal.MinValue;
        }

        return y < limit ? (decimal)y : decimal.MaxValue;
    }

    private static decimal? DoubleToDecimal(double? x) => x is double y ? DoubleToDecimal(y) : null;

    private static long DecimalToLong(decimal x)
    {
        if (x <= long.MinValue)
        {
            return long.MinValue;
        }

        return x < long.MaxValue ? (long)x : long.MaxValue;
    }

    private static long? DecimalToLong(decimal? x) => x is decimal y ? DecimalToLong(y) : null;

    private static ulong DecimalToULong(decimal x)
    {
        if (x <= 0)
        {
            return 0;
        }

        return x < ulong.MaxValue ? (ulong)x : ulong.MaxValue;
    }

    private static ulong? DecimalToULong(decimal? x) => x is decimal y ? DecimalToULong(y) : null;

    private static long? BoolToLong(bool? x)
    {
        if (x is bool y)
        {
            return y ? 1 : 0;
        }

        return null;
    }

    private static long ParseLong(string? x) => long.TryParse(x, Culture, out var result) ? result : 0;
    private static ulong ParseULong(string? x) => ulong.TryParse(x, Culture, out var result) ? result : 0;
    private static double ParseDouble(string? x) => double.TryParse(x, Culture, out var result) ? result : double.NaN;
    private static decimal ParseDecimal(string? x) => decimal.TryParse(x, Culture, out var result) ? result : 0;

    private static long? TryParseLong(string? x) => long.TryParse(x, Culture, out var result) ? result : null;
    private static ulong? TryParseULong(string? x) => ulong.TryParse(x, Culture, out var result) ? result : null;
    private static double? TryParseDouble(string? x) => double.TryParse(x, Culture, out var result) ? result : null;
    private static decimal? TryParseDecimal(string? x) => decimal.TryParse(x, Culture, out var result) ? result : null;
}
