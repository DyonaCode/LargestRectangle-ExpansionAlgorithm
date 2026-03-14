namespace LeastRectangles.Common;

/// <summary>
/// Defines the canonical tie-break rules used by all comparable algorithms.
/// </summary>
public static class RectangleSelection
{
    public static bool IsBetterCandidate(Rectangle candidate, Rectangle currentBest)
    {
        if (candidate.Area != currentBest.Area)
            return candidate.Area > currentBest.Area;

        if (candidate.Row != currentBest.Row)
            return candidate.Row < currentBest.Row;

        if (candidate.Col != currentBest.Col)
            return candidate.Col < currentBest.Col;

        if (candidate.Height != currentBest.Height)
            return candidate.Height < currentBest.Height;

        return candidate.Width < currentBest.Width;
    }
}
