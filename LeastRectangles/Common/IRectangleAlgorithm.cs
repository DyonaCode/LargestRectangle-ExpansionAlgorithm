namespace LeastRectangles.Common;

public interface IRectangleAlgorithm
{
    string Name { get; }
    int[,] Solve(int[,] inputGrid);
}

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