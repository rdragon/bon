namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    /// <summary>
    /// Converts a double to a decimal.
    /// This method exists because we don't want to throw exceptions.
    /// An explicit cast (or using the Convert class) can throw overflow exceptions.
    /// </summary>
    private static decimal? DoubleToDecimal(double? x)
    {
        if (x is not { } y)
        {
            return null;
        }

        if (double.IsNaN(y))
        {
            return null;
        }

        const double limit = (double)decimal.MaxValue;

        if (y <= -limit)
        {
            return decimal.MinValue;
        }

        return y < limit ? (decimal)y : decimal.MaxValue;
    }

    /// <summary>
    /// Converts a decimal to a long.
    /// This method exists because we don't want to throw exceptions.
    /// An explicit cast (or using the Convert class) can throw overflow exceptions.
    /// </summary>
    private static long? DecimalToLong(decimal? x)
    {
        if (x is not { } y)
        {
            return null;
        }

        // Here we copy the native double to long conversion logic.

        if (y <= long.MinValue)
        {
            return long.MinValue;
        }

        return y < long.MaxValue ? (long)y : long.MaxValue;
    }

    /// <summary>
    /// Converts a decimal to a ulong.
    /// This method exists because we don't want to throw exceptions.
    /// An explicit cast (or using the Convert class) can throw overflow exceptions.
    /// </summary>
    private static ulong? DecimalToULong(decimal? x)
    {
        if (x is not { } y)
        {
            return null;
        }

        // Here we copy the native double to ulong conversion logic.

        if (y <= 0)
        {
            return 0;
        }

        return y < ulong.MaxValue ? (ulong)y : ulong.MaxValue;
    }

    private static long? StringToLong(string? x) => long.TryParse(x, Culture, out var result) ? result : null;
    private static ulong? StringToULong(string? x) => ulong.TryParse(x, Culture, out var result) ? result : null;
    private static double? StringToDouble(string? x) => double.TryParse(x, Culture, out var result) ? result : null;
    private static decimal? StringToDecimal(string? x) => decimal.TryParse(x, Culture, out var result) ? result : null;
}
