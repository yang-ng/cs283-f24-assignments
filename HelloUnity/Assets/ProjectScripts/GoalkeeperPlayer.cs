using UnityEngine;

public class PlayerGK : MonoBehaviour
{
    public Transform pathStart; // Start position of the sliding path
    public Transform pathEnd;   // End position of the sliding path
    public float slideSpeed = 2f; // Speed of sliding along the path

    private bool isControlledByPlayer = false; // Whether the GK is currently controlled by the player
    private Vector3 targetPosition; // The next target position on the sliding path
    private bool movingToEnd = true; // Sliding direction along the path

    void Start()
    {
        // Initialize the first target position to the path's start
        targetPosition = pathStart.position;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning())
        {
            return; // Skip update logic if the game is over
        }
        
        if (!isControlledByPlayer)
        {
            SlideAlongPath();
        }
    }

    public void OnPlayerControl()
    {
        // Stop automatic movement when controlled by the player
        isControlledByPlayer = true;
    }

    public void OnPlayerRelease()
    {
        // Resume automatic movement when player releases control
        isControlledByPlayer = false;

        // Find the nearest point on the path to return to
        targetPosition = FindNearestPointOnPath();
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

    private Vector3 FindNearestPointOnPath()
    {
        // Determine the closest point to the GK's current position on the sliding path
        float distanceToStart = Vector3.Distance(transform.position, pathStart.position);
        float distanceToEnd = Vector3.Distance(transform.position, pathEnd.position);

        return (distanceToStart < distanceToEnd) ? pathStart.position : pathEnd.position;
    }
}
