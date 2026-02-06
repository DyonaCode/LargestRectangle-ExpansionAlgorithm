using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

public class RandomizedRectangleAlgorithm : IRectangleAlgorithm
{
    private static readonly Random rand = new(42); // Fixed seed for reproducibility

    public string Name => "Randomized Rectangle";

    public int[,] Solve(int[,] grid)
    {
        var rectangles = RectangleHelper.FindRectanglesGreedy(grid, (g) => LargestRectangle(g));
        return grid.VisualizeRectangles(rectangles);
    }

    public static Rectangle LargestRectangle(int[,] grid, int attemptsPerSize = 200)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int[,] ps = BuildPrefixSum(grid);
        int maxArea = rows * cols;

        // Try rectangle areas from large → small
        for (int area = maxArea; area >= 1; area--)
        {
            // Try factor pairs (height × width)
            for (int h = 1; h <= Math.Min(rows, area); h++)
            {
                if (area % h != 0) continue;
                int w = area / h;
                if (w > cols) continue;

                // Try random placements for this size
                for (int attempt = 0; attempt < attemptsPerSize; attempt++)
                {
                    int r1 = rand.Next(0, rows - h + 1);
                    int c1 = rand.Next(0, cols - w + 1);
                    int r2 = r1 + h - 1;
                    int c2 = c1 + w - 1;

                    int sum = GetRectSum(ps, r1, c1, r2, c2);
                    if (sum == area)
                    {
                        return new Rectangle(r1, c1, h, w);
                    }
                }
            }
        }

        return new Rectangle(0, 0, 0, 0); // No rectangle found
    }

    private static int[,] BuildPrefixSum(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int[,] ps = new int[rows + 1, cols + 1];

        for (int r = 1; r <= rows; r++)
        {
            for (int c = 1; c <= cols; c++)
            {
                ps[r, c] = grid[r - 1, c - 1]
                           + ps[r - 1, c]
                           + ps[r, c - 1]
                           - ps[r - 1, c - 1];
            }
        }

        return ps;
    }

    private static int GetRectSum(int[,] ps, int r1, int c1, int r2, int c2)
    {
        return ps[r2 + 1, c2 + 1] - ps[r1, c2 + 1] - ps[r2 + 1, c1] + ps[r1, c1];
    }
}