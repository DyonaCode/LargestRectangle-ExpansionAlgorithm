namespace LeastRectangles.Common;

/// <summary>
/// Defines the contract for algorithms that partition a grid into labeled rectangles.
/// </summary>
public interface IRectangleAlgorithm
{
    string Name { get; }
    int[,] Solve(int[,] inputGrid);
}

/// <summary>
/// Represents a rectangle using top-left origin plus height and width.
/// </summary>
public readonly struct Rectangle
{
    public readonly int Row, Col, Height, Width;
    public int Area => Height * Width;

    public Rectangle(int row, int col, int height, int width)
    {
        Row = row;
        Col = col;
        Height = height;
        Width = width;
    }

    public override string ToString() => $"Rectangle({Row}, {Col}, {Height}x{Width}, Area={Area})";
}
