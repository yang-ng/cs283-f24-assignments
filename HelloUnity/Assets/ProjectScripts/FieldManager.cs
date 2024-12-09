using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public Transform playerGoal; // The left goal (Player's goal)
    public Transform OpponentGoal; // The right goal (Opponent's goal)
    public Transform ball; // Reference to the ball

    private float xMin; // Closest X position to the left goal (Player's goal)
    private float xMax; // Closest X position to the right goal (Opponent's goal)
    private float xMid1; // Boundary between Defensive and Midfield areas
    private float xMid2; // Boundary between Midfield and Offensive areas

    void Start()
    {
        // Assuming goals are at the edges of the field along the X-axis
        xMin = playerGoal.position.x; // Leftmost edge of the field
        xMax = OpponentGoal.position.x; // Rightmost edge of the field

        // Calculate boundaries for the three areas
        xMid1 = xMin + (xMax - xMin) / 3; // 1/3 of the field
        xMid2 = xMin + 2 * (xMax - xMin) / 3; // 2/3 of the field
    }


    public string GetBallArea()
    {
        // Check the ball's position along the X-axis and determine the area
        if (ball.position.x <= xMid1)
        {
            return "PlayerDefensiveZone"; // Ball is near the left goal
        }
        else if (ball.position.x > xMid1 && ball.position.x <= xMid2)
        {
            return "Midfield"; // Ball is in the middle of the pitch
        }
        else
        {
            return "OpponentDefensiveZone"; // Ball is near the right goal
        }
    }
}
