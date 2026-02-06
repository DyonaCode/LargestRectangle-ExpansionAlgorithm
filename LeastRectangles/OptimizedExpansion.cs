using System.Runtime.CompilerServices;

namespace Benchmark;

public static class OptimizedExpansionAlgorithm
{
    public readonly struct Rect
    {
        public readonly short Row, Col, Height, Width;
        public int Area => Height * Width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(int row, int col, int height, int width)
        {
            Row = (short)row;
            Col = (short)col;
            Height = (short)height;
            Width = (short)width;
        }
    }

    public static int[,] Solve(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Use heap allocation for larger grids to avoid stack overflow
        byte[] workingArray = new byte[rows * cols];
        Span<byte> working = workingArray;
        InitializeWorking(grid, working, rows, cols);

        short[] prefixArray = new short[rows * cols];
        Span<short> prefixRight = prefixArray;
        BuildPrefixRight(working, prefixRight, rows, cols);

        var result = new int[rows, cols];
        int rectId = 1;

        while (true)
        {
            var best = FindBestRectangle(working, prefixRight, rows, cols);
            if (best.Area == 0) break;

            MarkRectangle(result, working, best, rectId++, rows, cols);
            UpdatePrefixRows(working, prefixRight, best, rows, cols);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeWorking(int[,] grid, Span<byte> working, int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            working[r * cols + c] = (byte)grid[r, c];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BuildPrefixRight(Span<byte> grid, Span<short> prefix, int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
        {
            short count = 0;
            int rowOffset = r * cols;
            for (int c = cols - 1; c >= 0; c--)
            {
                count = grid[rowOffset + c] == 1 ? (short)(count + 1) : (short)0;
                prefix[rowOffset + c] = count;
            }
        }
    }

    private static Rect FindBestRectangle(Span<byte> grid, Span<short> prefix, int rows, int cols)
    {
        Rect best = default;
        int bestArea = 0;

        for (int r = 0; r < rows; r++)
        {
            int rowOffset = r * cols;
            for (int c = 0; c < cols; c++)
            {
                if (grid[rowOffset + c] != 1) continue;

                int minWidth = prefix[rowOffset + c];

                for (int bottom = r; bottom < rows; bottom++)
                {
                    int bottomOffset = bottom * cols;
                    if (grid[bottomOffset + c] != 1) break;

                    minWidth = Math.Min(minWidth, prefix[bottomOffset + c]);
                    if (minWidth == 0) break;

                    int height = bottom - r + 1;
                    int area = height * minWidth;

                    if (area > bestArea)
                    {
                        bestArea = area;
                        best = new Rect(r, c, height, minWidth);
                    }
                }
            }
        }

        return best;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MarkRectangle(int[,] result, Span<byte> working, Rect rect, int id, int rows, int cols)
    {
        for (int r = rect.Row; r < rect.Row + rect.Height; r++)
        {
            int rowOffset = r * cols;
            for (int c = rect.Col; c < rect.Col + rect.Width; c++)
            {
                result[r, c] = id;
                working[rowOffset + c] = 0;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdatePrefixRows(Span<byte> grid, Span<short> prefix, Rect rect, int rows, int cols)
    {
        for (int r = rect.Row; r < rect.Row + rect.Height; r++)
        {
            short count = 0;
            int rowOffset = r * cols;
            for (int c = cols - 1; c >= 0; c--)
            {
                count = grid[rowOffset + c] == 1 ? (short)(count + 1) : (short)0;
                prefix[rowOffset + c] = count;
            }
        }
    }
}