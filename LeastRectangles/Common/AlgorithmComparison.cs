namespace LeastRectangles.Common;

/// <summary>
/// Captures one algorithm execution and its labeled result grid.
/// </summary>
public sealed record AlgorithmRunResult(string Name, int[,] Result);

/// <summary>
/// Runs groups of algorithms against the same input and verifies exact parity.
/// </summary>
public static class AlgorithmComparison
{
    /// <summary>
    /// Executes each algorithm on a cloned copy of the same grid and validates the output shape.
    /// </summary>
    public static IReadOnlyList<AlgorithmRunResult> RunAll(
        IEnumerable<IRectangleAlgorithm> algorithms,
        int[,] inputGrid)
    {
        var results = new List<AlgorithmRunResult>();

        foreach (var algorithm in algorithms)
        {
            int[,] result = algorithm.Solve(inputGrid.CreateWorkingCopy());
            RectangleResultInspector.ValidateAgainstInput(inputGrid, result);
            results.Add(new AlgorithmRunResult(algorithm.Name, result));
        }

        return results;
    }

    /// <summary>
    /// Throws if any algorithm in the set produces a result that differs from the first one.
    /// </summary>
    public static void AssertDeterministicallyEquivalent(
        IEnumerable<IRectangleAlgorithm> algorithms,
        int[,] inputGrid)
    {
        IReadOnlyList<AlgorithmRunResult> results = RunAll(algorithms, inputGrid);
        if (results.Count == 0)
            return;

        var baseline = results[0];

        for (int index = 1; index < results.Count; index++)
        {
            RectangleResultInspector.EnsureEquivalent(
                baseline.Result,
                results[index].Result,
                baseline.Name,
                results[index].Name);
        }
    }
}
