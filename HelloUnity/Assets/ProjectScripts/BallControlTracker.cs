using UnityEngine;

public class BallControlTracker : MonoBehaviour
{
    public string CurrentDribbler { get; private set; } = "None"; // current team controlling the ball

    private string lastKicker = "None"; // last team to kick the ball

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CurrentDribbler = "Player";
        }
        else if (collision.gameObject.CompareTag("Opponent"))
        {
            CurrentDribbler = "Opponent";
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Opponent"))
        {
            CurrentDribbler = "None";
        }
    }

    public void RegisterKick(string teamName)
    {
        lastKicker = teamName;
    }

    public string GetCurrentPossessor()
    {
        // if the ball is being dribbled, return the current controller
        if (CurrentDribbler != "None")
        {
            return CurrentDribbler;
        }

        // otherwise return the last kicker's team
        return lastKicker;
    }
}