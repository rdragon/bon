using Bon.Benchmarks.Deserialization;
using Bon.Benchmarks.Deserialization.Benchmarks;
using Bon.Benchmarks.NumberBenchmarks;

//new WholeNumberBenchmark().SanityCheck();
//new WholeNumberNullableBenchmark().SanityCheck();
//new WholeNumberSignedBenchmark().SanityCheck();
//new Read7BitEncodedIntBenchmark().SanityCheck();
//new Read7BitEncodedInt64Benchmark().SanityCheck();
//await OutputSizeMeasurer.Measure("ArrayOfNumberStruct", () => new DeserializeArrayOfNumberStruct { N = 100_000 });
//await OutputSizeMeasurer.Measure("ArrayOfProduct", () => new DeserializeArrayOfProduct { N = 100_000 });
//await OutputSizeMeasurer.Measure("ArrayOfPerson", () => new DeserializeArrayOfPerson { N = 100_000 });

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
