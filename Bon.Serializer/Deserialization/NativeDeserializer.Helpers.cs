namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
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
