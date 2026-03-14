using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

/// <summary>
/// Readable histogram-stack baseline that finds the next rectangle via row histograms.
/// </summary>
public class StackHistogramAlgorithm : IRectangleAlgorithm
{
    public string Name => "Stack Histogram";

    public int[,] Solve(int[,] grid)
    {
        var rectangles = OptimalDivider(grid);
        return grid.VisualizeRectangles(rectangles);
    }

    /// <summary>
    /// Partitions a binary grid into a set of non-overlapping rectangles 
    /// that cover all non-zero cells. This method ignores the actual 
    /// data values and treats every non-zero cell as identical.
    /// 
    /// The algorithm repeatedly finds the largest possible rectangle 
    /// consisting only of non-zero cells, records it, 
    /// removes it from consideration, 
    /// and continues until no valid cells remain.
    /// 
    /// This is a greedy approximation of the minimum-rectangle cover problem 
    /// and does not guarantee a mathematically minimal number of rectangles, 
    /// but performs well in practice.
    /// </summary>
    private static List<Rectangle> OptimalDivider(int[,] grid)
    {
        return RectangleHelper.FindRectanglesGreedy(grid, (g) => FindLargestRectangle(g));
    }

    /// <summary>
    /// Finds the largest-area rectangle consisting only of non-zero cells within the grid.
    /// 
    /// This uses a row-by-row histogram approach where each row is treated as the base of a
    /// histogram representing consecutive vertical runs of non-zero cells. For each row,
    /// the largest rectangle in the histogram is computed using a monotonic stack.
    /// 
    /// Time complexity is O(rows × cols).
    /// </summary>
    /// <param name="grid">A 2D grid where 0 = invalid cell and non-zero = usable cell.</param>
    /// <returns>The largest rectangle found, defined by (row, column, height, width). 
    /// Returns zeros if no valid rectangle exists.</returns>
    public static Rectangle FindLargestRectangle(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int[] heights = new int[cols];

        Rectangle best = default;

        for (int r = 0; r < rows; r++)
        {
            // Build histogram of consecutive 1s
            for (int c = 0; c < cols; c++)
            {
                heights[c] = grid[r, c] == 0 ? 0 : heights[c] + 1;
            }

            // Largest rectangle in histogram for this row
            var stack = new Stack<int>();
            for (int i = 0; i <= cols; i++)
            {
                int h = (i == cols) ? 0 : heights[i];

                while (stack.Count > 0 && h < heights[stack.Peek()])
                {
                    int top = stack.Pop();
                    int height = heights[top];
                    int width = stack.Count == 0 ? i : i - stack.Peek() - 1;
                    int bestCol = stack.Count == 0 ? 0 : stack.Peek() + 1;
                    var candidate = new Rectangle(r - height + 1, bestCol, height, width);

                    if (RectangleSelection.IsBetterCandidate(candidate, best))
                        best = candidate;
                }

                stack.Push(i);
            }
        }

        return best;
    }
}
