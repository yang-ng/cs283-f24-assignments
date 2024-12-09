using UnityEngine;

public class OpponentGK : MonoBehaviour
{
    public Transform pathStart; // Start position of the sliding path
    public Transform pathEnd;   // End position of the sliding path
    public float slideSpeed = 2f; // Speed of sliding along the path
    public float kickForce = 15f; // Force applied to the ball when kicked

    private Vector3 targetPosition; // The next target position on the sliding path
    private bool movingToEnd = true; // Sliding direction along the path

    void Start()
    {
        targetPosition = pathStart.position;
    }

    void Update()
    {
        SlideAlongPath();
    }

    private void SlideAlongPath()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);

        // Check if the GK reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Switch direction
            if (movingToEnd)
            {
                targetPosition = pathEnd.position;
                movingToEnd = false;
            }
            else
            {
                targetPosition = pathStart.position;
                movingToEnd = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) // Ensure the collided object is the ball
        {
            KickBall(collision.gameObject);
        }
    }

    private void KickBall(GameObject ball)
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            BallControlTracker tracker = FindObjectOfType<BallControlTracker>();
            tracker.RegisterKick("Opponent");

            // Calculate direction to kick the ball away
            Vector3 kickDirection = (ball.transform.position - transform.position).normalized;

            // Apply force to the ball
            ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
        }
    }
}
