namespace Bon.Serializer.Deserialization;

partial class NativeDeserializer
{
    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    private Delegate CreateDeserializer_ChangeOutputType(Delegate method, NativeType outputType, NativeType targetType)
    {
        if (outputType == targetType)
        {
            return method;
        }

        return outputType switch
        {
            NativeType.Byte => ChangeOutputType<byte, ulong?>(f => x => f(x)),
            NativeType.SByte => ChangeOutputType<sbyte, long?>(f => x => f(x)),
            NativeType.Short => ChangeOutputType<short, long?>(f => x => f(x)),
            NativeType.UShort => ChangeOutputType<ushort, ulong?>(f => x => f(x)),
            NativeType.Int => ChangeOutputType<int, long?>(f => x => f(x)),
            NativeType.UInt => ChangeOutputType<uint, ulong?>(f => x => f(x)),
            NativeType.Long => ChangeOutputType<long, long?>(f => x => f(x)),
            NativeType.ULong => ChangeOutputType<ulong, ulong?>(f => x => f(x)),
            NativeType.Float => ChangeOutputType<float, double?>(f => x => f(x)),
            NativeType.Double => ChangeOutputType<double, double?>(f => x => f(x)),
            _ => CreateDeserializer_ChangeTargetType(method, (BridgeType)outputType, targetType),
        };

        Delegate ChangeOutputType<T1, T2>(Func<Read<T1>, Read<T2>> func)
        {
            return CreateDeserializer_ChangeTargetType(func((Read<T1>)method), GetBridgeType(typeof(T2)), targetType);
        }
    }

    /// <summary>
    /// Modifies the given method by transforming its output to the target type.
    /// </summary>
    /// <param name="method">A method that outputs a value of the output type.</param>
    private Delegate CreateDeserializer_ChangeTargetType(Delegate method, BridgeType outputType, NativeType targetType)
    {
        return targetType switch
        {
            NativeType.Byte => ChangeTargetType<byte, ulong?>(f => x => (byte)(f(x) ?? 0)),
            NativeType.SByte => ChangeTargetType<sbyte, long?>(f => x => (sbyte)(f(x) ?? 0)),
            NativeType.Short => ChangeTargetType<short, long?>(f => x => (short)(f(x) ?? 0)),
            NativeType.UShort => ChangeTargetType<ushort, ulong?>(f => x => (ushort)(f(x) ?? 0)),
            NativeType.Int => ChangeTargetType<int, long?>(f => x => (int)(f(x) ?? 0)),
            NativeType.UInt => ChangeTargetType<uint, ulong?>(f => x => (uint)(f(x) ?? 0)),
            NativeType.Long => ChangeTargetType<long, long?>(f => x => f(x) ?? 0),
            NativeType.ULong => ChangeTargetType<ulong, ulong?>(f => x => f(x) ?? 0),
            NativeType.Float => ChangeTargetType<float, double?>(f => x => (float)(f(x) ?? 0)),
            NativeType.Double => ChangeTargetType<double, double?>(f => x => (double)(f(x) ?? 0)),
            _ => CreateDeserializer_WalkBridge(method, outputType, (BridgeType)targetType),
        };

        Read<T1> ChangeTargetType<T1, T2>(Func<Read<T2>, Read<T1>> func)
        {
            var reader = (Read<T2>)CreateDeserializer_WalkBridge(method, outputType, GetBridgeType(typeof(T2)));

            return func(reader);
        }
    }
}
