using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

/// <summary>
/// Readable expansion-based implementation that keeps prefix widths up to date incrementally.
/// </summary>
public class ExpansionRectangleAlgorithm : IRectangleAlgorithm
{
    public string Name => "Expansion";

    public int[,] Solve(int[,] grid)
    {
        int[,] working = grid.CreateWorkingCopy();
        int[,] prefixRight = BuildPrefixRight(working);
        var rectangles = new List<Rectangle>();

        while (true)
        {
            Rectangle best = FindLargestRectangle(working, prefixRight);
            if (best.Area == 0)
                break;

            rectangles.Add(best);
            RectangleHelper.ZeroOutRectangle(working, best);
            UpdatePrefixRows(working, prefixRight, best);
        }

        return grid.VisualizeRectangles(rectangles);
    }

    private static Rectangle FindLargestRectangle(int[,] working, int[,] prefixRight)
    {
        Rectangle best = default;

        for (int top = 0; top < working.GetLength(0); top++)
        {
            for (int left = 0; left < working.GetLength(1); left++)
            {
                if (working[top, left] == 0)
                    continue;

                int minWidth = prefixRight[top, left];
                for (int bottom = top; bottom < working.GetLength(0); bottom++)
                {
                    if (working[bottom, left] == 0)
                        break;

                    minWidth = Math.Min(minWidth, prefixRight[bottom, left]);
                    if (minWidth == 0)
                        break;

                    var candidate = new Rectangle(top, left, bottom - top + 1, minWidth);
                    if (RectangleSelection.IsBetterCandidate(candidate, best))
                        best = candidate;
                }
            }
        }

        return best;
    }

    private static int[,] BuildPrefixRight(int[,] working)
    {
        int rows = working.GetLength(0);
        int cols = working.GetLength(1);
        int[,] prefix = new int[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            int runLength = 0;
            for (int col = cols - 1; col >= 0; col--)
            {
                runLength = working[row, col] == 0 ? 0 : runLength + 1;
                prefix[row, col] = runLength;
            }
        }

        return prefix;
    }

    private static void UpdatePrefixRows(int[,] working, int[,] prefixRight, Rectangle rectangle)
    {
        for (int row = rectangle.Row; row < rectangle.Row + rectangle.Height; row++)
        {
            int runLength = 0;
            for (int col = working.GetLength(1) - 1; col >= 0; col--)
            {
                runLength = working[row, col] == 0 ? 0 : runLength + 1;
                prefixRight[row, col] = runLength;
            }
        }
    }
}
