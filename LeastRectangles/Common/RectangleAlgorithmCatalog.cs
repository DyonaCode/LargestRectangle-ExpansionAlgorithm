using LeastRectangles.Algorithms;

namespace LeastRectangles.Common;

/// <summary>
/// Creates the algorithm sets used by demos, tests, and benchmarks.
/// </summary>
public static class RectangleAlgorithmCatalog
{
    /// <summary>
    /// Creates the deterministic algorithms that should all produce the exact same labeled output.
    /// </summary>
    public static IReadOnlyList<IRectangleAlgorithm> CreateComparableAlgorithms()
    {
        return
        [
            new PrefixRectangleAlgorithm(),
            new StackHistogramAlgorithm(),
            new ExpansionRectangleAlgorithm(),
            new OptimizedPrefixAlgorithm(),
            new OptimizedExpansionAlgorithm()
        ];
    }
}
