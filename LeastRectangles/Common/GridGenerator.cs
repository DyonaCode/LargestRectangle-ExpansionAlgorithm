namespace LeastRectangles.Common;

public static class GridGenerator
{
    private static readonly Random _random = new Random(42);

    public static int[,] CreateRandomGrid(int rows = 100, int cols = 100)
    {
        int[,] grid = new int[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = _random.Next(0, 2);
            }
        }

        return grid;
    }
}