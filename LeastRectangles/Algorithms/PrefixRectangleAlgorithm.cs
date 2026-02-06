using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

public class PrefixRectangleAlgorithm : IRectangleAlgorithm
{
    public string Name => "Prefix Rectangle";

    public int[,] Solve(int[,] grid)
    {
        var rectangles = RectangleHelper
            .FindRectanglesGreedy(grid, (g) => LargestRectangle(g));
        return grid.VisualizeRectangles(rectangles);
    }

    public static Rectangle LargestRectangle(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Step 1: Build row prefix (consecutive 1s to the right)
        int[,] prefix = GeneratePrefixes(grid, rows, cols);

        int maxArea = 0;
        var rect = new Rectangle(0, 0, 0, 0);

        // Step 2: Try each cell as top-left
        for (int top = 0; top < rows; top++)
        {
            for (int left = 0; left < cols; left++)
            {
                if (grid[top, left] == 0) continue;

                int minWidth = int.MaxValue;

                for (int bottom = top; bottom < rows; bottom++)
                {
                    if (prefix[bottom, left] == 0) break;

                    minWidth = Math.Min(minWidth, prefix[bottom, left]);
                    int height = bottom - top + 1;
                    int area = height * minWidth;

                    if (area > maxArea)
                    {
                        maxArea = area;
                        rect = new Rectangle(top, left, height, minWidth);
                    }
                }
            }
        }
        return rect;
    }

    private static int[,] GeneratePrefixes(int[,] grid, int rows, int cols)
    {
        int[,] prefix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            int count = 0;
            for (int j = cols - 1; j >= 0; j--)
            {
                if (grid[i, j] == 1)
                    count++;
                else
                    count = 0;

                prefix[i, j] = count;
            }
        }

        return prefix;
    }
}