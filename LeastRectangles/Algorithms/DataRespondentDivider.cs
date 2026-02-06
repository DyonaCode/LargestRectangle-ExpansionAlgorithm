using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

public class DataRespondentDivider : IRectangleAlgorithm
{
    public string Name => "Data Respondent Divider";

    public int[,] Solve(int[,] grid)
    {
        var results = OptimalDivider(grid);
        return grid.VisualizeRectangles(results);
    }

    /// <summary>
    /// Gets the optimal least amount of rectangle accounting for datapoint variety. 
    /// cells should be aggregated by the statistically relevant groupings. 
    /// 1 & 78 should not be grouped but 1 & 2 could or should be. 
    ///
    /// Partitions a numeric grid into rectangles that are both spatially contiguous and
    /// statistically consistent in their values.
    /// 
    /// Unlike the data-agnostic version, this method prevents grouping cells whose values
    /// differ too widely. Rectangles are accepted only if their internal value variation
    /// is below a defined statistical threshold.
    /// 
    /// The algorithm greedily removes the "best" rectangle at each step, preferring:
    ///  1) Larger area rectangles
    ///  2) Rectangles with tighter statistical similarity
    /// 
    /// This is a heuristic solution to a spatial clustering problem with rectangular constraints.
    /// </summary>
    /// <param name="grid">A 2D grid where 0 = invalid cell and other values represent measurements.</param>
    /// <returns>A sequence of rectangles defined by (row, column, height, width).</returns>
    private static List<Rectangle> OptimalDivider(int[,] grid)
    {
        var results = new List<Rectangle>();
        var working = grid.CreateWorkingCopy();

        while (true)
        {
            var best = FindBestDataRectangle(working);
            if (best.Area == 0)
                break;

            results.Add(best);
            RectangleHelper.ZeroOutRectangle(working, best);
        }

        return results;
    }

    /// <summary>
    /// Searches the grid for the largest statistically valid rectangle of non-zero values.
    /// 
    /// Every possible rectangle starting at each cell is evaluated. A rectangle is considered
    /// valid only if all its cells are non-zero and its value distribution passes a statistical
    /// consistency test (see <see cref="IsRectangleValid"/>).
    /// 
    /// Among valid rectangles, the method prefers larger area first and lower statistical
    /// variation second.
    /// </summary>
    /// <param name="grid">The working grid where zero values represent already-used or invalid cells.</param>
    /// <returns>The best rectangle found, defined by (row, column, height, width). 
    /// Returns zeros if no valid rectangle exists.</returns>
    private static Rectangle FindBestDataRectangle(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int bestArea = 0;
        Rectangle best = new Rectangle(0, 0, 0, 0);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == 0) continue;

                for (int h = 1; r + h <= rows; h++)
                {
                    for (int w = 1; c + w <= cols; w++)
                    {
                        if (!IsRectangleValid(grid, r, c, h, w, out double score))
                            continue;

                        int area = h * w;

                        // Prefer larger rectangles, then tighter stats
                        if (area > bestArea || (area == bestArea && score < GetScore(best, grid)))
                        {
                            bestArea = area;
                            best = new Rectangle(r, c, h, w);
                        }
                    }
                }
            }
        }

        return best;
    }

    /// <summary>
    /// Computes the statistical variation score of a rectangle for comparison purposes.
    /// 
    /// This is a helper method used to prefer rectangles with tighter value groupings
    /// when multiple rectangles have the same area.
    /// </summary>
    /// <param name="rect">The rectangle to evaluate.</param>
    /// <param name="grid">The grid containing the data.</param>
    /// <returns>The coefficient of variation for the rectangle.</returns>
    private static double GetScore(Rectangle rect, int[,] grid)
    {
        if (rect.Area == 0) return double.MaxValue;
        IsRectangleValid(grid, rect.Row, rect.Col, rect.Height, rect.Width, out double cv);
        return cv;
    }

    /// <summary>
    /// Determines whether a rectangular region contains statistically similar values.
    /// 
    /// A rectangle is rejected if:
    ///  • Any cell contains a 0 (invalid or already assigned)
    ///  • The coefficient of variation (standard deviation / mean) exceeds a threshold
    /// 
    /// This prevents grouping outliers (e.g., 1 and 78) while allowing similar values
    /// (e.g., 1 and 2) to be clustered together.
    /// </summary>
    /// <param name="grid">The source grid.</param>
    /// <param name="r0">Top row of the rectangle.</param>
    /// <param name="c0">Left column of the rectangle.</param>
    /// <param name="h">Rectangle height.</param>
    /// <param name="w">Rectangle width.</param>
    /// <param name="cv">Outputs the coefficient of variation for the rectangle.</param>
    /// <returns>True if the rectangle is statistically consistent; otherwise false.</returns>
    private static bool IsRectangleValid(int[,] grid, int r0, int c0, int h, int w, out double cv)
    {
        var values = new List<int>();

        for (int r = r0; r < r0 + h; r++)
        for (int c = c0; c < c0 + w; c++)
        {
            int v = grid[r, c];
            if (v == 0)
            {
                cv = double.MaxValue;
                return false;
            }
            values.Add(v);
        }

        double mean = values.Average();
        double variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;
        double stdDev = Math.Sqrt(variance);

        cv = stdDev / mean;

        // threshold
        return cv < 1; 
    }
}