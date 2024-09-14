using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

public class Game
{
    private Basket _basket;
    private List<Apple> _apples;
    private int _score;
    private float _timeLeft; // time of the game, will be set in Setup()
    private bool _showRules = true;  // initially true to show the rule before the game starts
    private float _ruleDisplayTime; // time that the rule is displayed, will be set in Setup()

    public void Setup()
    {
        _basket = new Basket(Window.width / 2 - 40, Window.height - 30, 80);
        _apples = new List<Apple>();

        // create apples
        for (int i = 0; i < 3; i++)
        {
            _apples.Add(new Apple(Window.RandomRange(0, Window.width - 20), Window.RandomRange(100, 300)));
        }

        _score = 0;
        _timeLeft = 120;
        _showRules = true;
        _ruleDisplayTime = 1.5f;
    }
    
    public void Update(float dt)
    {
        if (_showRules)
        {
            _ruleDisplayTime -= dt;
            if (_ruleDisplayTime <= 0)
            {
                _showRules = false;
            }
            return;
        }

        if (_timeLeft > 0) // when the game is still on
        {
            _timeLeft -= dt;

            // update apples
            foreach (var apple in _apples)
            {
                apple.Update(dt);

                if (apple.isCaught(_basket))
                {
                    _score++;
                    apple.Reset();
                }
                else if (apple.isOutOfBounds())
                {
                    apple.Reset();
                }
            }
        }
    }



    public void Draw(Graphics g)
    {
        // set a background color
        Color skyBlue = Color.FromArgb(135, 206, 235); // sky color
        Brush skyBrush = new SolidBrush(skyBlue);
        g.FillRectangle(skyBrush, 0, 0, Window.width, Window.height);

        if (_showRules)
        {
            // show the rule
            Font ruleFont = new Font("Arial", 24, FontStyle.Bold);
            Brush textBrush = Brushes.White;
            string ruleMessage = "Use the basket to catch the apples";
            SizeF ruleSize = g.MeasureString(ruleMessage, ruleFont);

            g.DrawString(
                ruleMessage,
                ruleFont,
                textBrush,
                (Window.width - ruleSize.Width) / 2,
                (Window.height - ruleSize.Height) / 2
            );
        }
        else if (_timeLeft > 0) // when the game is on, draw apples and basket
        {
            _basket.Draw(g);

            foreach (var apple in _apples)
            {
                apple.Draw(g);
            }

            // print score and remaining time on the screen
            g.DrawString($"Score: {_score}", new Font("Arial", 16), Brushes.White, 10, 10);
            g.DrawString($"Time left: {Math.Max(0, (int)_timeLeft)}", new Font("Arial", 16), Brushes.White, 10, 30);
        }
        else
        {
            // time's up, print score
            Font gameOverFont = new Font("Arial", 24, FontStyle.Bold);
            Brush textBrush = Brushes.White;
            string message = $"Your score: {_score}";
            SizeF textSize = g.MeasureString(message, gameOverFont);

            g.DrawString(
                message,
                gameOverFont,
                textBrush,
                (Window.width - textSize.Width) / 2,
                (Window.height - textSize.Height) / 2 - 20
            );

            // print instrution to play again
            Font instructionFont = new Font("Arial", 14);
            string instructionMessage = "Click the screen to play again";
            SizeF instructionSize = g.MeasureString(instructionMessage, instructionFont);

            g.DrawString(
                instructionMessage,
                instructionFont,
                textBrush,
                (Window.width - instructionSize.Width) / 2,
                (Window.height - textSize.Height) / 2 + 20
            );
        }
    }

    public void MouseClick(MouseEventArgs mouse)
    {
        if (_timeLeft <= 0) // when the game ends, click will start the game again
        {
            Setup();
        }
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode == Keys.Left || key.KeyCode == Keys.A)
        {
            _basket.MoveLeft(0.1f);
        }
        else if (key.KeyCode == Keys.Right || key.KeyCode == Keys.D)
        {
            _basket.MoveRight(0.1f);
        }
    }

}