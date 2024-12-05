using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public string teamName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Get the position of the ball relative to the goal
            Vector3 ballToGoal = other.transform.position - transform.position;

            // check if the ball is entering from the correct side
            float relativePosition = Vector3.Dot(ballToGoal, transform.forward);

            if (teamName == "Player" && relativePosition > 0)
            {
                GameManager.Instance.RegisterGoal("Opponent");
            }
            else if (teamName == "Opponent" && relativePosition < 0)
            {
                GameManager.Instance.RegisterGoal("Player");
            }
        }
    }
}
