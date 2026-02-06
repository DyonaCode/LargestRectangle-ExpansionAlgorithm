using LeastRectangles.Algorithms;
using LeastRectangles.Common;

namespace LeastRectangles;

internal static class Program
{
    public static void Main()
    {
        Console.WriteLine("Rectangle Algorithm Comparison");
        Console.WriteLine(new string('=', 50));
        
        // Test on standard binary grid
        Console.WriteLine("\nTesting on Standard Binary Grid (7x7):");
        // TestData.StandardBinaryGrid.PrintGrid("Input Grid");
        var grid = GridGenerator.CreateRandomGrid(8, 8);
        grid.PrintGrid();
        
        var binaryAlgorithms = new IRectangleAlgorithm[]
        {
            new PrefixRectangleAlgorithm(),
            new OptimizedExpansionAlgorithm(),
            new OptimizedPrefixAlgorithm(),
            new RandomizedRectangleAlgorithm(),
            new StackHistogramAlgorithm()
        };

        var results = new List<int[,]>();
        
        foreach (var algorithm in binaryAlgorithms)
        {
            Console.WriteLine($"\n--- {algorithm.Name} Algorithm ---");
            var result = algorithm.Solve(grid);
            results.Add(result);
            result.PrintGrid($"{algorithm.Name} Result");
            int rectangleCount = CountUniqueRectangles(result);
            Console.WriteLine($"Total rectangles found: {rectangleCount}");
        }

        foreach (var gri in results)
        {
            bool same = true;
            for (int row = 0; row < gri.GetLength(0); row++)
            for (int col = 0; col < gri.GetLength(1); col++)
                same = gri[row, col] == results[0][col, row];

            Console.WriteLine(same);
        }
        
        // // Test data-aware algorithm on varied data grid
        // Console.WriteLine("\n" + new string('=', 50));
        // Console.WriteLine("Testing Data-Aware Algorithm on Varied Data Grid:");
        // TestData.VariedDataGrid.PrintGrid("Input Grid (with varied values)");
        //
        // var dataAlgorithm = new DataRespondentDivider();
        // Console.WriteLine($"\n--- {dataAlgorithm.Name} Algorithm ---");
        // var dataResult = dataAlgorithm.Solve(TestData.VariedDataGrid);
        // dataResult.PrintGrid($"{dataAlgorithm.Name} Result");
        // int dataRectangleCount = CountUniqueRectangles(dataResult);
        // Console.WriteLine($"Total rectangles found: {dataRectangleCount}");
        
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("Algorithm comparison complete!");
    }
    
    private static int CountUniqueRectangles(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int maxId = 0;

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            maxId = Math.Max(maxId, grid[r, c]);

        return maxId;
    }
}

