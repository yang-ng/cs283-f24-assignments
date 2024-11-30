using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    public GameObject ball;
    public float ballOffset = 1.5f;
    public float kickForce = 10f;
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

        if (isDribbling && Input.GetKeyDown(KeyCode.Space))
        {
            KickBall();
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
        Debug.Log("starts dribbling!");
        isDribbling = true;

        ballRb.isKinematic = true; // freeze the ball's physics while dribbling
    }

    void DribbleBall()
    {
        if (ball != null)
        {
            Vector3 ballPosition = transform.position + transform.forward * ballOffset;
            ballPosition.y = transform.position.y + 0.05f; // adjust height
            ball.transform.position = ballPosition;
        }
    }

    void KickBall()
    {
        isDribbling = false;
        Debug.Log("kick the ball!");

        ballRb.isKinematic = false; // enable physics on the ball

        // apply force
        Vector3 kickDirection = transform.forward;
        ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
    }
}