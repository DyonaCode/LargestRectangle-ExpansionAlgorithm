namespace LeastRectangles.Common;

/// <summary>
/// Common helper methods used across rectangle algorithms
/// </summary>
public static class RectangleHelper
{
    /// <summary>
    /// Standard algorithm pattern: repeatedly find largest rectangle and remove it
    /// </summary>
    public static List<Rectangle> FindRectanglesGreedy(int[,] inputGrid, 
        Func<int[,], Rectangle> findLargestRectangle)
    {
        var working = inputGrid.CreateWorkingCopy();
        var rectangles = new List<Rectangle>();

        while (true)
        {
            var rect = findLargestRectangle(working);
            if (rect.Area == 0)
                break;

            rectangles.Add(rect);
            ZeroOutRectangle(working, rect);
        }

        return rectangles;
    }

    /// <summary>
    /// Marks all cells in a rectangle as used (sets to 0)
    /// </summary>
    public static void ZeroOutRectangle(int[,] grid, Rectangle rect)
    {
        for (int r = rect.Row; r < rect.Row + rect.Height; r++)
        for (int c = rect.Col; c < rect.Col + rect.Width; c++)
            grid[r, c] = 0;
    }

    /// <summary>
    /// Legacy version using tuple format
    /// </summary>
    public static void ZeroOutRectangle(int[,] grid, (int row, int col, int height, int width) rect)
    {
        ZeroOutRectangle(grid, new Rectangle(rect.row, rect.col, rect.height, rect.width));
    }

    /// <summary>
    /// Converts legacy tuple rectangles to Rectangle structs
    /// </summary>
    public static List<Rectangle> ConvertToRectangles(
        IEnumerable<(int row, int col, int height, int width)> tuples)
    {
        return tuples.Select(t => new Rectangle(t.row, t.col, t.height, t.width)).ToList();
    }
}