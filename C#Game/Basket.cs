// creates a basket that is controlled by player

using System;
using System.Drawing;

public class Basket
{
    public Point Position { get; private set; }
    public int Width { get; private set; }
    private int Speed = 300;

    public Basket(int x, int y, int width)
    {
        Position = new Point(x, y);
        Width = width;
    }

    public void MoveLeft(float dt)
    {
        Position = new Point(Math.Max(0, Position.X - (int)(Speed * dt)), Position.Y);
    }

    public void MoveRight(float dt)
    {
        Position = new Point(Math.Min(Window.width - Width, Position.X + (int)(Speed * dt)), Position.Y);
    }

    public void Draw(Graphics g)
    {
        Brush brownBrush = new SolidBrush(Color.FromArgb(139, 69, 19)); // brown color
        g.FillRectangle(brownBrush, Position.X, Position.Y, Width, 30); // use a rectangle to represent basket
    }
}