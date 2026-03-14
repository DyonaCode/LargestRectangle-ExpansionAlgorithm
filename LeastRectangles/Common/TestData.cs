namespace LeastRectangles.Common;

/// <summary>
/// Fixed grids used by demos and deterministic tests.
/// </summary>
public static class TestData
{
    /// <summary>
    /// Standard 7x7 binary test grid used across multiple algorithms
    /// 1 = valid cell, 0 = invalid/blocked cell
    /// </summary>
    public static int[,] StandardBinaryGrid => new[,]
    {
        {1,1,1,1,1,1,1},
        {1,1,0,1,1,1,0},
        {0,1,1,1,1,1,1},
        {1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1},
        {1,0,1,1,1,1,1},
        {1,1,1,1,1,0,1}
    };
}
