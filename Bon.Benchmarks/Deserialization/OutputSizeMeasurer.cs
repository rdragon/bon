using Bon.Benchmarks.Deserialization.Benchmarks;

namespace Bon.Benchmarks.Deserialization;

internal class OutputSizeMeasurer
{
    public static async Task Measure<TIn, TOut>(string benchmarkName, Func<BenchmarkBase<TIn, TOut>> createBenchmark)
    {
        Console.WriteLine(benchmarkName);
        Console.WriteLine("| Method      | Size (bytes) | Ratio |");
        Console.WriteLine("|------------ |-------------:|------:|");

        var bonBenchmark = createBenchmark();
        await bonBenchmark.SetupBon();
        WriteRow("Bon", bonBenchmark.GetStreamLength());
        var bonOutput = bonBenchmark.Bon();

        var messagePackBenchmark = createBenchmark();
        messagePackBenchmark.SetupMessagePack();
        WriteRow("MessagePack", messagePackBenchmark.GetStreamLength());
        var messagePackOutput = messagePackBenchmark.MessagePack();

        var protobufBenchmark = createBenchmark();
        protobufBenchmark.SetupProtobuf();
        WriteRow("Protobuf", protobufBenchmark.GetStreamLength());
        var protobufOutput = protobufBenchmark.Protobuf();

        var jsonBenchmark = createBenchmark();
        jsonBenchmark.SetupJson();
        WriteRow("Json", jsonBenchmark.GetStreamLength());
        var jsonOutput = jsonBenchmark.Json();

        RequireEqual(bonOutput, jsonOutput);
        RequireEqual(bonOutput, messagePackOutput);
        RequireEqual(bonOutput, protobufOutput);

        void WriteRow(string name, long length)
        {
            var size = $"{length,12:N0}";
            var ratio = $"{length * 1.0 / bonBenchmark.GetStreamLength(),5:0.00}";
            Console.WriteLine($"| {name,-11} | {size} | {ratio} |");
        }
    }

    private static void RequireEqual(object? left, object? right)
    {
        if (!IsEqual(left, right))
        {
            throw new Exception($"Unequal outputs found");
        }
    }

    private static bool IsEqual(object? left, object? right)
    {
        if (left is Array leftArray && right is Array rightArray)
        {
            for (int i = 0; i < leftArray.Length; i++)
            {
                if (!IsEqual(leftArray.GetValue(i), rightArray.GetValue(i)))
                {
                    return false;
                }
            }

            return true;
        }

        if (left is Product leftProduct && right is Product rightProduct)
        {
            return leftProduct.Int == rightProduct.Int
                && IsEqual(leftProduct.IntArray, rightProduct.IntArray)
                && IsEqual(leftProduct.Features, rightProduct.Features);
        }

        return left?.Equals(right) ?? (right is null);
    }
}
