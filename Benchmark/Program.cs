using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LeastRectangles.Algorithms;
using LeastRectangles.Common;

namespace Benchmark;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RectangleAlgorithmsBenchmark>();
    }
}

[MemoryDiagnoser]
public class RectangleAlgorithmsBenchmark
{
    [Params(20, 50, 100)]
    public int GridSize;

    private int[,] grid;

    [GlobalSetup]
    public void Setup()
    {
        grid = GridGenerator.CreateRandomGrid(GridSize, GridSize);
    }

  //  [Benchmark]
    public int PrefixRectangleAlgorithmTest()
    {
        var algorithm = new PrefixRectangleAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
    }
    
   // [Benchmark]
    public int OptiPrefixRectangleAlgorithmTest()
    {
        var algorithm = new OptimizedPrefixAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
    }

    [Benchmark]
    public int OptimizedExpansionTest()
    {
        var algorithm = new OptimizedExpansionAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
    }

   public int HistogramStackTest()
    {
        var algorithm = new StackHistogramAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
    }

    [Benchmark]
    public int OptimizedBranchingTest()
    {
        var algorithm = new OptimizedBranchingAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
    }

 //   [Benchmark]
    public int RandomizedRectangleTest()
    {
        var algorithm = new RandomizedRectangleAlgorithm();
        var result = algorithm.Solve(grid);
        return CountUniqueRectangles(result);
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