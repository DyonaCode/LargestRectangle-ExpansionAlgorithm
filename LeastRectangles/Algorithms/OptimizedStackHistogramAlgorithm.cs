using System.Buffers;
using System.Runtime.CompilerServices;
using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

/// <summary>
/// Optimized histogram-stack implementation that reuses pooled arrays and a manual stack.
/// </summary>
public class OptimizedStackHistogramAlgorithm : IRectangleAlgorithm
{
    private const int ArrayPoolThreshold = 1024;

    public string Name => "Stack Histogram (Optimized)";

    public int[,] Solve(int[,] grid)
    {
        var rectangles = RectangleHelper.FindRectanglesGreedy(grid, LargestRectangle);
        return grid.VisualizeRectangles(rectangles);
    }

    /// <summary>
    /// Finds the largest rectangle using an in-place histogram and an array-backed monotonic stack.
    /// </summary>
    public static Rectangle LargestRectangle(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        if (cols == 0 || rows == 0)
            return default;

        bool poolHeights = cols > ArrayPoolThreshold;
        bool poolStack = cols + 1 > ArrayPoolThreshold;

        int[] heightsArray = poolHeights ? ArrayPool<int>.Shared.Rent(cols) : new int[cols];
        int[] stackArray = poolStack ? ArrayPool<int>.Shared.Rent(cols + 1) : new int[cols + 1];
        Span<int> heights = heightsArray.AsSpan(0, cols);
        Span<int> stack = stackArray.AsSpan(0, cols + 1);

        try
        {
            heights.Clear();
            Rectangle best = default;

            for (int row = 0; row < rows; row++)
            {
                UpdateHeights(grid, row, heights);
                int stackCount = 0;

                for (int index = 0; index <= cols; index++)
                {
                    int currentHeight = index == cols ? 0 : heights[index];

                    while (stackCount > 0 && currentHeight < heights[stack[stackCount - 1]])
                    {
                        int topIndex = stack[--stackCount];
                        int height = heights[topIndex];
                        int left = stackCount == 0 ? 0 : stack[stackCount - 1] + 1;
                        int width = stackCount == 0 ? index : index - stack[stackCount - 1] - 1;
                        var candidate = new Rectangle(row - height + 1, left, height, width);

                        if (RectangleSelection.IsBetterCandidate(candidate, best))
                            best = candidate;
                    }

                    stack[stackCount++] = index;
                }
            }

            return best;
        }
        finally
        {
            if (poolHeights)
                ArrayPool<int>.Shared.Return(heightsArray);

            if (poolStack)
                ArrayPool<int>.Shared.Return(stackArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateHeights(int[,] grid, int row, Span<int> heights)
    {
        for (int col = 0; col < heights.Length; col++)
            heights[col] = grid[row, col] == 0 ? 0 : heights[col] + 1;
    }
}
