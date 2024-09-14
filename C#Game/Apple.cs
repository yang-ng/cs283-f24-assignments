using System;
using System.Drawing;

public class Apple
{
    public Point Position { get; private set; }
    public int Speed { get; private set; }
    private int Size = 20;

    public Apple(int x, int speed)
    {
        Position = new Point(x, 0); // start from the top of the window
        Speed = speed;
    }

    public void Update(float dt)
    {
        Position = new Point(Position.X, Position.Y + (int)(Speed * dt));
    }

    public void Draw(Graphics g)
    {
        // colors of apple
        Brush redBrush = new SolidBrush(Color.Red); // apple
        Brush brownBrush = new SolidBrush(Color.FromArgb(139, 69, 19)); // stem
        Brush greenBrush = new SolidBrush(Color.Green); // leaf

        // draw apple
        int appleSize = 30;
        g.FillEllipse(redBrush, Position.X, Position.Y, appleSize, appleSize);

        // draw stem
        int stickWidth = 4;
        int stickHeight = 10;
        g.FillRectangle(brownBrush, Position.X + (appleSize / 2) - (stickWidth / 2), Position.Y - stickHeight, stickWidth, stickHeight);

        // draw leaf
        int triangleWidth = 20;
        int triangleHeight = 10;
        Point[] trianglePoints =
        {
        new Point(Position.X + (appleSize / 2) - stickWidth / 2, Position.Y),
        new Point(Position.X + (appleSize / 2) + triangleWidth - stickWidth / 2, Position.Y),
        new Point(Position.X + (appleSize / 2) + triangleWidth / 2 - stickWidth / 2, Position.Y - triangleHeight)
    };
        g.FillPolygon(greenBrush, trianglePoints);
    }

    public bool isCaught(Basket basket)
    {
        return (Position.Y >= Window.height - 45) &&
               (Position.X > basket.Position.X && Position.X < basket.Position.X + basket.Width);
    }

    public bool isOutOfBounds()
    {
        return Position.Y > Window.height;
    }

    public void Reset()
    {
        Position = new Point(Window.RandomRange(0, Window.width - Size), 0);
    }
}