using System.Runtime.CompilerServices;

namespace Benchmark;

public static class OptimizedBranchingAlgorithm
{
    public static int[,] Solve(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        byte[] workingArray = new byte[rows * cols];
        Span<byte> working = workingArray;

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            working[r * cols + c] = (byte)grid[r, c];

        var result = new int[rows, cols];
        int rectId = 1;

        byte[] priorityArray = new byte[rows * cols];
        Span<byte> priority = priorityArray;

        ComputePriority(working, priority, rows, cols);

        while (true)
        {
            int bestR = -1, bestC = -1, bestPriority = -1;
            for (int r = 0; r < rows; r++)
            {
                int offset = r * cols;
                for (int c = 0; c < cols; c++)
                {
                    if (working[offset + c] == 1 && priority[offset + c] > bestPriority)
                    {
                        bestPriority = priority[offset + c];
                        bestR = r;
                        bestC = c;
                    }
                }
            }

            if (bestR < 0) break;

            var rect = ExpandFromSeed(working, bestR, bestC, rows, cols);

            for (int r = rect.row; r < rect.row + rect.height; r++)
            {
                int offset = r * cols;
                for (int c = rect.col; c < rect.col + rect.width; c++)
                {
                    result[r, c] = rectId;
                    working[offset + c] = 0;
                }
            }
            rectId++;

            UpdatePriority(working, priority, rect, rows, cols);
        }

        return result;
    }

    private static void ComputePriority(Span<byte> grid, Span<byte> priority, int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
        {
            int offset = r * cols;
            for (int c = 0; c < cols; c++)
            {
                if (grid[offset + c] != 1)
                {
                    priority[offset + c] = 0;
                    continue;
                }

                int p = 0;
                if (r == 0 || grid[(r - 1) * cols + c] == 0) p++;
                if (r == rows - 1 || grid[(r + 1) * cols + c] == 0) p++;
                if (c == 0 || grid[offset + c - 1] == 0) p++;
                if (c == cols - 1 || grid[offset + c + 1] == 0) p++;

                priority[offset + c] = (byte)(p + 1);
            }
        }
    }

    private static (int row, int col, int height, int width) ExpandFromSeed(
        Span<byte> grid, int seedR, int seedC, int rows, int cols)
    {
        int top = seedR, bottom = seedR;
        int left = seedC, right = seedC;

        bool changed = true;
        while (changed)
        {
            changed = false;

            if (top > 0 && CanExpandHorizontal(grid, top - 1, left, right, cols))
            { top--; changed = true; }

            if (bottom < rows - 1 && CanExpandHorizontal(grid, bottom + 1, left, right, cols))
            { bottom++; changed = true; }

            if (left > 0 && CanExpandVertical(grid, left - 1, top, bottom, cols))
            { left--; changed = true; }

            if (right < cols - 1 && CanExpandVertical(grid, right + 1, top, bottom, cols))
            { right++; changed = true; }
        }

        return (top, left, bottom - top + 1, right - left + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanExpandHorizontal(Span<byte> grid, int row, int left, int right, int cols)
    {
        int offset = row * cols;
        for (int c = left; c <= right; c++)
            if (grid[offset + c] != 1) return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanExpandVertical(Span<byte> grid, int col, int top, int bottom, int cols)
    {
        for (int r = top; r <= bottom; r++)
            if (grid[r * cols + col] != 1) return false;
        return true;
    }

    private static void UpdatePriority(Span<byte> grid, Span<byte> priority,
        (int row, int col, int height, int width) rect, int rows, int cols)
    {
        int r1 = Math.Max(0, rect.row - 1);
        int r2 = Math.Min(rows - 1, rect.row + rect.height);
        int c1 = Math.Max(0, rect.col - 1);
        int c2 = Math.Min(cols - 1, rect.col + rect.width);

        for (int r = r1; r <= r2; r++)
        {
            int offset = r * cols;
            for (int c = c1; c <= c2; c++)
            {
                if (grid[offset + c] != 1) continue;

                int p = 0;
                if (r == 0 || grid[(r - 1) * cols + c] == 0) p++;
                if (r == rows - 1 || grid[(r + 1) * cols + c] == 0) p++;
                if (c == 0 || grid[offset + c - 1] == 0) p++;
                if (c == cols - 1 || grid[offset + c + 1] == 0) p++;
                priority[offset + c] = (byte)(p + 1);
            }
        }
    }
}