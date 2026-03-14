namespace LeastRectangles.Common;

/// <summary>
/// Validates labeled rectangle maps and extracts their geometry for diagnostics.
/// </summary>
public static class RectangleResultInspector
{
    public static IReadOnlyList<Rectangle> ExtractRectangles(int[,] labeledGrid)
    {
        var boundsById = new Dictionary<int, RectangleBounds>();

        for (int row = 0; row < labeledGrid.GetLength(0); row++)
        for (int col = 0; col < labeledGrid.GetLength(1); col++)
        {
            int label = labeledGrid[row, col];
            if (label <= 0)
                continue;

            boundsById[label] = boundsById.TryGetValue(label, out var current)
                ? current.Include(row, col)
                : RectangleBounds.Create(row, col);
        }

        var rectangles = new List<Rectangle>(boundsById.Count);

        foreach (var entry in boundsById.OrderBy(pair => pair.Key))
        {
            RectangleBounds bounds = entry.Value;
            int height = bounds.MaxRow - bounds.MinRow + 1;
            int width = bounds.MaxCol - bounds.MinCol + 1;

            if (bounds.CellCount != height * width)
            {
                throw new InvalidOperationException(
                    $"Label {entry.Key} does not form a solid rectangle. " +
                    $"Expected area {height * width}, observed {bounds.CellCount}.");
            }

            for (int row = bounds.MinRow; row <= bounds.MaxRow; row++)
            for (int col = bounds.MinCol; col <= bounds.MaxCol; col++)
                if (labeledGrid[row, col] != entry.Key)
                    throw new InvalidOperationException(
                        $"Label {entry.Key} is not rectangular; cell ({row}, {col}) breaks the bounding box.");

            rectangles.Add(new Rectangle(bounds.MinRow, bounds.MinCol, height, width));
        }

        return rectangles;
    }

    public static void ValidateAgainstInput(int[,] inputGrid, int[,] labeledGrid)
    {
        EnsureSameDimensions(inputGrid, labeledGrid);
        _ = ExtractRectangles(labeledGrid);

        for (int row = 0; row < inputGrid.GetLength(0); row++)
        for (int col = 0; col < inputGrid.GetLength(1); col++)
        {
            bool expectedFilled = inputGrid[row, col] != 0;
            bool actualFilled = labeledGrid[row, col] > 0;

            if (expectedFilled != actualFilled)
            {
                throw new InvalidOperationException(
                    $"Coverage mismatch at ({row}, {col}). " +
                    $"Input={inputGrid[row, col]}, Result={labeledGrid[row, col]}.");
            }
        }
    }

    public static void EnsureEquivalent(int[,] expected, int[,] actual, string expectedName, string actualName)
    {
        EnsureSameDimensions(expected, actual);

        for (int row = 0; row < expected.GetLength(0); row++)
        for (int col = 0; col < expected.GetLength(1); col++)
            if (expected[row, col] != actual[row, col])
                throw new InvalidOperationException(
                    $"{actualName} diverged from {expectedName} at ({row}, {col}). " +
                    $"{expectedName}={expected[row, col]}, {actualName}={actual[row, col]}\n" +
                    $"{expectedName}:\n{expected.ToDisplayString()}\n\n" +
                    $"{actualName}:\n{actual.ToDisplayString()}");
    }

    private static void EnsureSameDimensions(int[,] expected, int[,] actual)
    {
        if (expected.GetLength(0) != actual.GetLength(0) || expected.GetLength(1) != actual.GetLength(1))
        {
            throw new InvalidOperationException(
                $"Grid dimensions do not match. Expected {expected.GetLength(0)}x{expected.GetLength(1)}, " +
                $"actual {actual.GetLength(0)}x{actual.GetLength(1)}.");
        }
    }

    private readonly record struct RectangleBounds(
        int MinRow,
        int MinCol,
        int MaxRow,
        int MaxCol,
        int CellCount)
    {
        public static RectangleBounds Create(int row, int col) => new(row, col, row, col, 1);

        public RectangleBounds Include(int row, int col) => new(
            Math.Min(MinRow, row),
            Math.Min(MinCol, col),
            Math.Max(MaxRow, row),
            Math.Max(MaxCol, col),
            CellCount + 1);
    }
}
