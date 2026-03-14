using LeastRectangles.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeastRectangles.Tests;

/// <summary>
/// Verifies that all comparable algorithms remain deterministic and exactly equivalent.
/// </summary>
[TestClass]
public class ComparableAlgorithmsTests
{
    public static IEnumerable<object[]> ComparableCases()
    {
        yield return ["Empty-3x3", new int[3, 3]];
        yield return ["SingleCell", new[,] { { 1 } }];
        yield return ["Solid-2x3", new[,] { { 1, 1, 1 }, { 1, 1, 1 } }];
        yield return ["StandardBinaryGrid", TestData.StandardBinaryGrid];
        yield return ["Random-8x8-35", GridGenerator.CreateRandomGrid(8, 8, 0.35, 11)
        ];
        yield return ["Random-8x8-50", GridGenerator.CreateRandomGrid(8, 8, 0.50, 19)
        ];
        yield return ["Random-12x12-70", GridGenerator.CreateRandomGrid(12, 12, 0.70, 23)
        ];
        yield return ["Rooms-12x12", GridGenerator.CreateRoomsGrid(12)];
    }

    [TestMethod]
    [DynamicData(nameof(ComparableCases))]
    public void ComparableAlgorithmsProduceIdenticalMaps(string caseName, int[,] grid)
    {
        try
        {
            AlgorithmComparison.AssertDeterministicallyEquivalent(
                RectangleAlgorithmCatalog.CreateComparableAlgorithms(),
                grid);
        }
        catch (Exception ex)
        {
            Assert.Fail($"{caseName} failed deterministic equivalence: {ex.Message}");
        }
    }

    [TestMethod]
    [DynamicData(nameof(ComparableCases))]
    public void ComparableAlgorithmsAreRepeatable(string caseName, int[,] grid)
    {
        foreach (var algorithm in RectangleAlgorithmCatalog.CreateComparableAlgorithms())
        {
            int[,] first = algorithm.Solve(grid.CreateWorkingCopy());
            int[,] second = algorithm.Solve(grid.CreateWorkingCopy());

            RectangleResultInspector.ValidateAgainstInput(grid, first);
            RectangleResultInspector.ValidateAgainstInput(grid, second);

            try
            {
                RectangleResultInspector.EnsureEquivalent(
                    first,
                    second,
                    $"{algorithm.Name} first run",
                    $"{algorithm.Name} second run");
            }
            catch (Exception ex)
            {
                Assert.Fail($"{caseName} / {algorithm.Name} was not repeatable: {ex.Message}");
            }
        }
    }
}
