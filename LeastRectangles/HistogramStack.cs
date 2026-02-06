namespace Benchmark;

public static class HistogramStackAlgorithm
{
    public static int[,] Solve(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int[] heights = new int[cols + 1]; // +1 for sentinel
        var result = new int[rows, cols];

        int[,] working = (int[,])grid.Clone();
        int rectId = 1;

        int[] stack = new int[cols + 1];
        int[] startPos = new int[cols + 1];

        while (HasUncovered(working, rows, cols))
        {
            Array.Clear(heights, 0, heights.Length);

            var best = FindLargestHistogram(working, heights, stack, startPos, rows, cols);

            if (best.height == 0) break;

            for (int r = best.row; r < best.row + best.height; r++)
            for (int c = best.col; c < best.col + best.width; c++)
            {
                result[r, c] = rectId;
                working[r, c] = 0;
            }
            rectId++;
        }

        return result;
    }

    private static bool HasUncovered(int[,] grid, int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            if (grid[r, c] == 1) return true;
        return false;
    }

    private static (int row, int col, int height, int width) FindLargestHistogram(
        int[,] grid, int[] heights, int[] stack, int[] startPos, int rows, int cols)
    {
        int bestArea = 0;
        var best = (row: 0, col: 0, height: 0, width: 0);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                heights[c] = grid[r, c] == 1 ? heights[c] + 1 : 0;
            }
            heights[cols] = 0; // Sentinel

            int stackTop = -1;

            for (int c = 0; c <= cols; c++)
            {
                int start = c;

                while (stackTop >= 0 && heights[stack[stackTop]] > heights[c])
                {
                    int idx = stack[stackTop];
                    int h = heights[idx];
                    start = startPos[stackTop];
                    stackTop--;

                    int width = c - start;
                    int area = h * width;

                    if (area > bestArea)
                    {
                        bestArea = area;
                        best = (r - h + 1, start, h, width);
                    }
                }

                stackTop++;
                stack[stackTop] = c;
                startPos[stackTop] = start;
            }
        }

        return best;
    }
}