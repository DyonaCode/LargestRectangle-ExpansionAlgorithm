using LeastRectangles.Common;

namespace LeastRectangles;

/// <summary>
/// Console entry point that prints one deterministic comparison over the sample grid.
/// </summary>
internal static class Program
{
    public static void Main()
    {
        Console.WriteLine("LeastRectangles");
        Console.WriteLine(new string('=', 50));

        int[,] grid = TestData.StandardBinaryGrid;
        IReadOnlyList<AlgorithmRunResult> results = AlgorithmComparison.RunAll(
            RectangleAlgorithmCatalog.CreateComparableAlgorithms(),
            grid);

        grid.PrintGrid("Input");

        foreach (var result in results)
        {
            result.Result.PrintGrid($"{result.Name} ({result.Result.CountRectangles()} rectangles)");
        }

        var baseline = results[0];

        for (int index = 1; index < results.Count; index++)
        {
            RectangleResultInspector.EnsureEquivalent(
                baseline.Result,
                results[index].Result,
                baseline.Name,
                results[index].Name);
        }

        Console.WriteLine("\nComparable algorithms matched exactly: True");
        Console.WriteLine("Run `dotnet test` for the parity suite and `dotnet run --project Benchmark -c Release` for benchmarks.");
    }
}
