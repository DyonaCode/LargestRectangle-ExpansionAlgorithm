using System.Text;

namespace LeastRectangles.Common;

/// <summary>
/// Shared helpers for cloning, formatting, and visualizing grids.
/// </summary>
public static class GridExtensions
{
    /// <summary>
    /// Creates a visualization grid where each rectangle is assigned a unique ID.
    /// All cells belonging to the same rectangle receive the same integer identifier.
    /// </summary>
    public static int[,] VisualizeRectangles(this int[,] original,
        IEnumerable<Rectangle> rectangles)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);

        int[,] map = new int[rows, cols];
        int id = 1;

        foreach (var rect in rectangles)
        {
            for (int r = rect.Row; r < rect.Row + rect.Height; r++)
            for (int c = rect.Col; c < rect.Col + rect.Width; c++)
                map[r, c] = id;

            id++;
        }

        return map;
    }

    /// <summary>
    /// Overload for legacy tuple format
    /// </summary>
    public static int[,] VisualizeRectangles(this int[,] original,
        IEnumerable<(int row, int col, int height, int width)> rectangles)
    {
        return original.VisualizeRectangles(
            rectangles.Select(r => new Rectangle(r.row, r.col, r.height, r.width)));
    }

    /// <summary>
    /// Pretty prints a 2D grid to the console
    /// </summary>
    public static void PrintGrid(this int[,] grid, string title = "")
    {
        if (!string.IsNullOrEmpty(title))
        {
            Console.WriteLine($"\n=== {title} ===");
        }

        Console.WriteLine(grid.ToDisplayString());
    }

    /// <summary>
    /// Creates a working copy of the grid for algorithms that need to modify it
    /// </summary>
    public static int[,] CreateWorkingCopy(this int[,] grid)
    {
        return (int[,])grid.Clone();
    }

    /// <summary>
    /// Counts the number of distinct rectangle labels in a result grid.
    /// </summary>
    public static int CountRectangles(this int[,] grid)
    {
        var ids = new HashSet<int>();

        for (int row = 0; row < grid.GetLength(0); row++)
        for (int col = 0; col < grid.GetLength(1); col++)
            if (grid[row, col] > 0)
                ids.Add(grid[row, col]);

        return ids.Count;
    }

    /// <summary>
    /// Formats a grid into a stable multi-line string for diagnostics.
    /// </summary>
    public static string ToDisplayString(this int[,] grid)
    {
        var builder = new StringBuilder();
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
                builder.Append($"{grid[row, col],3}");

            if (row < rows - 1)
                builder.AppendLine();
        }

        return builder.ToString();
    }
}
