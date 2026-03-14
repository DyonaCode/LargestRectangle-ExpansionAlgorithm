using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using LeastRectangles.Algorithms;
using LeastRectangles.Common;

namespace Benchmark;

/// <summary>
/// Benchmark entry point.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}

/// <summary>
/// Shared setup for deterministic rectangle benchmark suites.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public abstract class RectangleBenchmarkBase
{
    [Params(32, 64, 128)]
    public int Size;

    [Params(ComparisonScenario.Random35, 
        ComparisonScenario.Random50, 
        ComparisonScenario.Random70, 
        ComparisonScenario.Rooms)]
    public ComparisonScenario Scenario;

    protected int[,] Grid = default!;

    [GlobalSetup]
    public void Setup()
    {
        Grid = CreateGrid(Scenario, Size);
        AlgorithmComparison.AssertDeterministicallyEquivalent(ComparisonAlgorithms, Grid);
    }

    protected abstract IReadOnlyList<IRectangleAlgorithm> ComparisonAlgorithms { get; }

    private static int[,] CreateGrid(ComparisonScenario scenario, int size)
    {
        return scenario switch
        {
            ComparisonScenario.Random35 => GridGenerator.CreateRandomGrid(size, size, 0.35, 1000 + size),
            ComparisonScenario.Random50 => GridGenerator.CreateRandomGrid(size, size, 0.50, 2000 + size),
            ComparisonScenario.Random70 => GridGenerator.CreateRandomGrid(size, size, 0.70, 3000 + size),
            ComparisonScenario.Rooms => GridGenerator.CreateRoomsGrid(size),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
        };
    }
}

/// <summary>
/// Named benchmark input scenarios.
/// </summary>
public enum ComparisonScenario
{
    Random35,
    Random50,
    Random70,
    Rooms
}

/// <summary>
/// Public apples-to-apples benchmark across the readable algorithm implementations.
/// </summary>
public class ReferenceRectangleBenchmarks : RectangleBenchmarkBase
{
    private readonly PrefixRectangleAlgorithm _prefix = new();
    private readonly StackHistogramAlgorithm _stack = new();
    private readonly ExpansionRectangleAlgorithm _expansion = new();

    protected override IReadOnlyList<IRectangleAlgorithm> ComparisonAlgorithms =>
        [_prefix, _stack, _expansion];

    [Benchmark(Baseline = true)]
    public int[,] Prefix() => _prefix.Solve(Grid);

    [Benchmark]
    public int[,] StackHistogram() => _stack.Solve(Grid);

    [Benchmark]
    public int[,] Expansion() => _expansion.Solve(Grid);
}

/// <summary>
/// Benchmark suite for the readable and optimized prefix implementations.
/// </summary>
public class PrefixImplementationBenchmarks : RectangleBenchmarkBase
{
    private readonly PrefixRectangleAlgorithm _prefix = new();
    private readonly OptimizedPrefixAlgorithm _optimizedPrefix = new();

    protected override IReadOnlyList<IRectangleAlgorithm> ComparisonAlgorithms =>
        [_prefix, _optimizedPrefix];

    [Benchmark(Baseline = true)]
    public int[,] Prefix() => _prefix.Solve(Grid);

    [Benchmark]
    public int[,] PrefixOptimized() => _optimizedPrefix.Solve(Grid);
}

/// <summary>
/// Benchmark suite for the readable and optimized expansion implementations.
/// </summary>
public class ExpansionImplementationBenchmarks : RectangleBenchmarkBase
{
    private readonly ExpansionRectangleAlgorithm _expansion = new();
    private readonly OptimizedExpansionAlgorithm _optimizedExpansion = new();

    protected override IReadOnlyList<IRectangleAlgorithm> ComparisonAlgorithms =>
        [_expansion, _optimizedExpansion];

    [Benchmark(Baseline = true)]
    public int[,] Expansion() => _expansion.Solve(Grid);

    [Benchmark]
    public int[,] ExpansionOptimized() => _optimizedExpansion.Solve(Grid);
}
