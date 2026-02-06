namespace LeastRectangles.Common;

public static class TestData
{
    /// <summary>
    /// Standard 7x7 binary test grid used across multiple algorithms
    /// 1 = valid cell, 0 = invalid/blocked cell
    /// </summary>
    public static int[,] StandardBinaryGrid => new int[,]
    {
        {1,1,1,1,1,1,1},
        {1,1,0,1,1,1,0},
        {0,1,1,1,1,1,1},
        {1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1},
        {1,0,1,1,1,1,1},
        {1,1,1,1,1,0,1}
    };

    /// <summary>
    /// Test grid with varying numeric values for data-aware algorithms
    /// 0 = invalid cell, other values represent measurements
    /// </summary>
    public static int[,] VariedDataGrid => new int[,]
    {
        {22,4,1,1,1,3,1},
        {1,1,0,2,1,1,0},
        {0,1,1,1,1,1,1},
        {1,2,1,50,1,1,1},
        {1,3,1,1,1,1,1},
        {1,0,1,1,1,1,1},
        {1,2,78,1,1,0,1}
    };
}