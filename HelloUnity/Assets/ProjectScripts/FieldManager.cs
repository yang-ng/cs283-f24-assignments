using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public Transform playerGoal;
    public Transform OpponentGoal; 
    public Transform ball;

    private float xMin;
    private float xMax;
    private float xMid1;
    private float xMid2;
    void Start()
    {
        xMin = playerGoal.position.x;
        xMax = OpponentGoal.position.x;

        // calculate boundaries for the three areas
        xMid1 = xMin + (xMax - xMin) / 3;
        xMid2 = xMin + 2 * (xMax - xMin) / 3;
    }


    public string GetBallArea()
    {
        if (ball.position.x <= xMid1)
        {
            return "PlayerDefensiveZone";
        }
        else if (ball.position.x > xMid1 && ball.position.x <= xMid2)
        {
            return "Midfield";
        }
        else
        {
            return "OpponentDefensiveZone";
        }
    }
}