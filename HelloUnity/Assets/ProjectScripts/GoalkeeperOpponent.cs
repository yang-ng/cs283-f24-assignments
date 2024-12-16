using UnityEngine;

public class OpponentGK : MonoBehaviour
{
    public Transform pathStart;
    public Transform pathEnd;
    public float slideSpeed = 2f;
    public float kickForce = 15f;

    private Vector3 targetPosition;
    private bool movingToEnd = true;

    void Start()
    {
        targetPosition = pathStart.position;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning())
        {
            return;
        }
        
        SlideAlongPath();
    }

    private void SlideAlongPath()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
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
        if (collision.gameObject.CompareTag("Ball"))
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
            Vector3 kickDirection = (ball.transform.position - transform.position).normalized;
            ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
        }
    }
}