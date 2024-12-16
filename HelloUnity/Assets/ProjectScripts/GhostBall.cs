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
        if (!GameManager.Instance.IsGameRunning())
        {
            return;
        }
        
        if (isDribbling)
        {
            DribbleBall();
        }

        if (isDribbling && Input.GetKeyDown(KeyCode.J))
        {
            KickBall(kickForce);
        }

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

        private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == ball.gameObject)
        {
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
            Vector3 targetPosition = transform.position + transform.forward * ballOffset;
            targetPosition.y = ball.transform.position.y;
            Vector3 direction = (targetPosition - ball.transform.position).normalized;
            float distanceToTarget = Vector3.Distance(ball.transform.position, targetPosition);
            float dynamicDribbleForce = dribbleForce * Mathf.Clamp(distanceToTarget, 1f, 5f);
            ballRb.AddForce(direction * dynamicDribbleForce, ForceMode.Force);
            Vector3 correctedVelocity = (targetPosition - ball.transform.position).normalized * dribbleForce;
            ballRb.velocity = Vector3.Lerp(ballRb.velocity, correctedVelocity, Time.deltaTime * 10f);
            if (ballRb.velocity.magnitude > dribbleForce)
            {
                ballRb.velocity = ballRb.velocity.normalized * dribbleForce;
            }
        }
    }

    void KickBall(float force)
    {
        isDribbling = false;
        BallControlTracker tracker = FindObjectOfType<BallControlTracker>();
        tracker.RegisterKick("Player");
        Vector3 direction = transform.forward;
        ballRb.AddForce(direction * force, ForceMode.Impulse);
    }
}