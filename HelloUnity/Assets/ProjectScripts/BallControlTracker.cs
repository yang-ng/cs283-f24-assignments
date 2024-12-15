using UnityEngine;

public class BallControlTracker : MonoBehaviour
{
    public string CurrentDribbler { get; private set; } = "None"; // Current team controlling the ball ("Player" or "Opponent")

    private string lastKicker = "None"; // Last team to kick the ball ("Player" or "Opponent")

    void Update()
    {
        // When spacebar is pressed, print the current controller of the ball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string controllerToPrint = CurrentDribbler != "None" ? CurrentDribbler : $"Last kicker: {lastKicker}";
            Debug.Log($"{controllerToPrint}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the ball is colliding with a player or opponent
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
        // When the ball is no longer in collision, it is not being dribbled
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Opponent"))
        {
            CurrentDribbler = "None";
        }
    }

    public void RegisterKick(string teamName)
    {
        // Register the team that last kicked the ball
        lastKicker = teamName;
    }

    public string GetCurrentPossessor()
    {
        // If the ball is being dribbled, return the current controller
        if (CurrentDribbler != "None")
        {
            return CurrentDribbler;
        }

        // Otherwise, return the last kicker's team
        return lastKicker;
    }
}
