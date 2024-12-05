using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    public GameObject ball;
    public float ballOffset = 1.5f;
    public float dribbleForce = 10f;
    public float kickForce = 10f;
    public float shootForce = 20f;

    private bool isDribbling = false;
    private Rigidbody ballRb;

    void Start()
    {
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (isDribbling)
        {
            DribbleBall();
        }

        // Kick action with key J
        if (isDribbling && Input.GetKeyDown(KeyCode.J))
        {
            KickBall(kickForce);
        }

        // Shoot action with key K
        if (isDribbling && Input.GetKeyDown(KeyCode.K))
        {
            KickBall(shootForce);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == ball)
        {
            StartDribbling();
        }
    }

    void StartDribbling()
    {
        isDribbling = true;
    }

    void DribbleBall()
    {
        if (ball != null)
        {
            // Calculate the target position slightly in front of the ghost
            Vector3 targetPosition = transform.position + transform.forward * ballOffset;

            // Keep the ball on the ground
            targetPosition.y = ball.transform.position.y;

            // Calculate the direction to the target position
            Vector3 direction = (targetPosition - ball.transform.position).normalized;

            // Apply force to move the ball toward the target position
            float distanceToTarget = Vector3.Distance(ball.transform.position, targetPosition);
            float dynamicDribbleForce = dribbleForce * Mathf.Clamp(distanceToTarget, 1f, 5f);

            ballRb.AddForce(direction * dynamicDribbleForce, ForceMode.Force);

            // Actively correct the ball's velocity to prevent outward drift during rotation
            Vector3 correctedVelocity = (targetPosition - ball.transform.position).normalized * dribbleForce;
            ballRb.velocity = Vector3.Lerp(ballRb.velocity, correctedVelocity, Time.deltaTime * 10f);

            // Limit the ball's overall velocity
            if (ballRb.velocity.magnitude > dribbleForce)
            {
                ballRb.velocity = ballRb.velocity.normalized * dribbleForce;
            }
        }
    }

    void KickBall(float force)
    {
        isDribbling = false;

        // Apply the specified force to the ball in the ghost's forward direction
        Vector3 direction = transform.forward;
        ballRb.AddForce(direction * force, ForceMode.Impulse);
    }
}
