namespace LeastRectangles.Common;

/// <summary>
/// Creates deterministic benchmark and test grids.
/// </summary>
public static class GridGenerator
{
    /// <summary>
    /// Creates a seeded random binary grid with a configurable fill probability.
    /// </summary>
    public static int[,] CreateRandomGrid(
        int rows = 100,
        int cols = 100,
        double fillProbability = 0.5,
        int seed = 42)
    {
        if (fillProbability < 0 || fillProbability > 1)
            throw new ArgumentOutOfRangeException(nameof(fillProbability), "Fill probability must be between 0 and 1.");

        var random = new Random(seed);
        int[,] grid = new int[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[r, c] = random.NextDouble() < fillProbability ? 1 : 0;
            }
        }

        return grid;
    }

    /// <summary>
    /// Creates a structured grid of large rooms separated by walls with deterministic doors.
    /// </summary>
    public static int[,] CreateRoomsGrid(int size)
    {
        if (size < 6)
            throw new ArgumentOutOfRangeException(nameof(size), "Room grids require size >= 6.");

        int[,] grid = new int[size, size];

        for (int row = 0; row < size; row++)
        for (int col = 0; col < size; col++)
            grid[row, col] = 1;

        int spacing = Math.Max(4, size / 4);

        for (int wall = spacing; wall < size - 1; wall += spacing)
        {
            for (int col = 0; col < size; col++)
                grid[wall, col] = 0;

            int leftDoor = Math.Max(1, wall / 2);
            int rightDoor = Math.Min(size - 2, size - leftDoor - 1);
            grid[wall, leftDoor] = 1;
            grid[wall, rightDoor] = 1;
        }

        for (int wall = spacing; wall < size - 1; wall += spacing)
        {
            for (int row = 0; row < size; row++)
                grid[row, wall] = 0;

            int topDoor = Math.Max(1, wall / 2);
            int bottomDoor = Math.Min(size - 2, size - topDoor - 1);
            grid[topDoor, wall] = 1;
            grid[bottomDoor, wall] = 1;
        }

        return grid;
    }
}
