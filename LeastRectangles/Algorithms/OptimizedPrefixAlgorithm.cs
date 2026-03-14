using System.Buffers;
using System.Runtime.CompilerServices;
using LeastRectangles.Common;

namespace LeastRectangles.Algorithms;

/// <summary>
/// Optimized prefix rectangle algorithm using safe managed code and compact data types.
/// 
/// This algorithm uses several safe performance optimizations:
/// 1. ArrayPool for memory management instead of unsafe stackalloc
/// 2. Compact byte data types for grids ≤ 255x255
/// 3. Flattened array indexing for better cache performance
/// 4. Early termination optimizations
/// 5. Aggressive method inlining
/// 
/// Performance Characteristics:
/// - Safe alternative to unsafe pointer arithmetic
/// - Excellent cache locality with flat array access
/// - Memory-efficient using ArrayPool for large grids
/// - Optimized for grids up to 255x255 dimensions
/// </summary>
public class OptimizedPrefixAlgorithm : IRectangleAlgorithm
{
    // Threshold for using array allocation vs ArrayPool
    private const int ArrayPoolThreshold = 1024;

    public string Name => "Prefix (Optimized)";

    public int[,] Solve(int[,] grid)
    {
        var rectangles = RectangleHelper
            .FindRectanglesGreedy(grid, (g) => LargestRectangle(g));
        return grid.VisualizeRectangles(rectangles);
    }

    /// <summary>
    /// Finds the largest rectangle using safe memory management and optimized algorithms.
    /// Uses ArrayPool for efficient memory allocation when dealing with larger grids.
    /// </summary>
    /// <param name="grid">Input 2D grid where 1 = valid cell, 0 = invalid cell</param>
    /// <returns>Largest rectangle found in the grid</returns>
    public static Rectangle LargestRectangle(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int totalSize = rows * cols;

        // Validate grid size constraints for byte dimensions
        if (rows > 255 || cols > 255)
        {
            throw new ArgumentException($"Grid dimensions ({rows}x{cols}) exceed maximum supported size of 255x255");
        }

        // Use direct array allocation for small grids, ArrayPool for large ones
        if (totalSize <= ArrayPoolThreshold)
        {
            byte[] prefix = new byte[totalSize];
            return FindLargestRectangleCore(grid, prefix, rows, cols);
        }
        else
        {
            byte[] rentedArray = ArrayPool<byte>.Shared.Rent(totalSize);
            try
            {
                // ArrayPool doesn't guarantee zeroed memory
                Array.Clear(rentedArray, 0, totalSize);
                return FindLargestRectangleCore(grid, rentedArray, rows, cols);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentedArray);
            }
        }
    }

    /// <summary>
    /// Core rectangle finding algorithm using safe array operations.
    /// 
    /// Algorithm Steps:
    /// 1. Build prefix sum array using flattened indexing for cache efficiency
    /// 2. For each potential top-left corner, expand rectangle downward
    /// 3. Use prefix sums to calculate maximum width efficiently
    /// 4. Apply early termination optimizations to skip impossible cases
    /// 5. Return rectangle with maximum area found
    /// 
    /// Optimizations:
    /// - Flattened array indexing: grid[r,c] -> grid[r*cols + c]
    /// - Early area comparison to skip impossible rectangles
    /// - Early termination when remaining area can't improve result
    /// - Aggressive method inlining for performance
    /// </summary>
    /// <param name="grid">Source 2D grid</param>
    /// <param name="prefix">Working array for prefix calculations</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <returns>Largest rectangle found</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Rectangle FindLargestRectangleCore(int[,] grid, byte[] prefix, int rows, int cols)
    {
        // Step 1: Build row prefix (consecutive 1s to the right) using flat indexing
        GeneratePrefixesFlat(grid, prefix, rows, cols);

        int maxArea = 0;
        int bestTop = 0, bestLeft = 0, bestHeight = 0, bestWidth = 0;

        // Step 2: Try each cell as top-left corner
        for (int top = 0; top < rows; top++)
        {
            int topOffset = top * cols;

            for (int left = 0; left < cols; left++)
            {
                // Skip invalid starting positions
                if (grid[top, left] == 0) continue;

                int minWidth = prefix[topOffset + left];
                if (minWidth == 0) continue;

                // Early exit optimization: check if this position can beat current best
                int maxPossibleHeight = rows - top;
                if (minWidth * maxPossibleHeight <= maxArea) continue;

                // Expand rectangle downward from this top-left corner
                for (int bottom = top; bottom < rows; bottom++)
                {
                    int bottomOffset = bottom * cols;
                    int currentPrefixValue = prefix[bottomOffset + left];

                    // Stop if we hit an invalid row
                    if (currentPrefixValue == 0) break;

                    // Update minimum width constraint
                    if (currentPrefixValue < minWidth)
                        minWidth = currentPrefixValue;

                    // Calculate current rectangle area
                    int height = bottom - top + 1;
                    int area = height * minWidth;
                    var candidate = new Rectangle(top, left, height, minWidth);

                    // Update best rectangle using the shared deterministic tie-break rules
                    if (area > maxArea ||
                        (area == maxArea &&
                         RectangleSelection.IsBetterCandidate(
                             candidate,
                             new Rectangle(bestTop, bestLeft, bestHeight, bestWidth))))
                    {
                        maxArea = area;
                        bestTop = top;
                        bestLeft = left;
                        bestHeight = height;
                        bestWidth = minWidth;
                    }

                    // Early termination: if remaining rows can't improve result
                    int remainingRows = rows - bottom - 1;
                    if ((height + remainingRows) * minWidth <= maxArea) break;
                }
            }
        }

        return new Rectangle(bestTop, bestLeft, bestHeight, bestWidth);
    }

    /// <summary>
    /// Generates prefix sum arrays using flattened indexing for optimal cache performance.
    /// 
    /// For each row, calculates consecutive count of 1s from right to left.
    /// This enables O(1) width calculation during rectangle expansion.
    /// 
    /// Example: Row [1,1,0,1,1,1] produces prefix [2,1,0,3,2,1]
    /// 
    /// Memory Layout:
    /// - Uses flattened array: prefix[r*cols + c] instead of prefix[r,c]
    /// - Better cache locality due to linear memory access pattern
    /// - Byte data type reduces memory usage by 75% vs int
    /// </summary>
    /// <param name="grid">Source 2D grid</param>
    /// <param name="prefix">Destination prefix array</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GeneratePrefixesFlat(int[,] grid, byte[] prefix, int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            int rowOffset = i * cols;
            byte count = 0;

            // Process row from right to left
            for (int j = cols - 1; j >= 0; j--)
            {
                int idx = rowOffset + j;
                if (grid[i, j] == 1)
                    count++;
                else
                    count = 0;

                prefix[idx] = count;
            }
        }
    }
}

/// <summary>
/// Alternative implementation optimized specifically for byte grids.
/// Useful when input data is already in byte format or memory is extremely constrained.
/// </summary>
public static class PrefixRectangleByteGrid
{
    private const int ArrayPoolThreshold = 2048;

    /// <summary>
    /// Finds largest rectangle in a byte grid using safe memory management.
    /// Assumes grid values are only 0 or 1, optimized for memory-constrained scenarios.
    /// </summary>
    /// <param name="grid">Byte grid data in row-major order</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <returns>Largest rectangle as (Top, Left, Height, Width) tuple</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static (int Top, int Left, int Height, int Width) LargestRectangle(
        ReadOnlySpan<byte> grid, int rows, int cols)
    {
        int totalSize = rows * cols;

        // Validate grid size
        if (rows > 255 || cols > 255)
        {
            throw new ArgumentException($"Grid dimensions ({rows}x{cols}) exceed maximum supported size of 255x255");
        }

        if (totalSize <= ArrayPoolThreshold)
        {
            byte[] prefix = new byte[totalSize];
            return FindLargestCore(grid, prefix, rows, cols);
        }
        else
        {
            byte[] rentedArray = ArrayPool<byte>.Shared.Rent(totalSize);
            try
            {
                Array.Clear(rentedArray, 0, totalSize);
                return FindLargestCore(grid, rentedArray, rows, cols);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentedArray);
            }
        }
    }

    /// <summary>
    /// Core implementation for byte grid processing with maximum performance optimizations.
    /// Uses flat array indexing and early termination for optimal cache performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int Top, int Left, int Height, int Width) FindLargestCore(
        ReadOnlySpan<byte> grid, byte[] prefix, int rows, int cols)
    {
        // Build prefix sum array
        for (int i = 0; i < rows; i++)
        {
            int rowOffset = i * cols;
            byte count = 0;

            for (int j = cols - 1; j >= 0; j--)
            {
                int idx = rowOffset + j;
                count = grid[idx] != 0 ? (byte)(count + 1) : (byte)0;
                prefix[idx] = count;
            }
        }

        int maxArea = 0;
        int bestTop = 0, bestLeft = 0, bestHeight = 0, bestWidth = 0;

        // Find largest rectangle
        for (int top = 0; top < rows; top++)
        {
            int topOffset = top * cols;

            for (int left = 0; left < cols; left++)
            {
                if (grid[topOffset + left] == 0) continue;

                int minWidth = prefix[topOffset + left];
                
                // Early termination check
                if (minWidth * (rows - top) <= maxArea) continue;

                for (int bottom = top; bottom < rows; bottom++)
                {
                    int bottomOffset = bottom * cols;
                    int currentPrefix = prefix[bottomOffset + left];

                    if (currentPrefix == 0) break;
                    if (currentPrefix < minWidth) minWidth = currentPrefix;

                    int height = bottom - top + 1;
                    int area = height * minWidth;
                    var candidate = new Rectangle(top, left, height, minWidth);

                    if (area > maxArea ||
                        (area == maxArea &&
                         RectangleSelection.IsBetterCandidate(
                             candidate,
                             new Rectangle(bestTop, bestLeft, bestHeight, bestWidth))))
                    {
                        maxArea = area;
                        bestTop = top;
                        bestLeft = left;
                        bestHeight = height;
                        bestWidth = minWidth;
                    }

                    // Early termination for remaining rows
                    if ((height + rows - bottom - 1) * minWidth <= maxArea) break;
                }
            }
        }

        return (bestTop, bestLeft, bestHeight, bestWidth);
    }
}
