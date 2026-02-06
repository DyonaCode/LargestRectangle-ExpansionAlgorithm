namespace LeastRectangles.Common;

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

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
                Console.Write($"{grid[r, c],3}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Creates a working copy of the grid for algorithms that need to modify it
    /// </summary>
    public static int[,] CreateWorkingCopy(this int[,] grid)
    {
        return (int[,])grid.Clone();
    }
}