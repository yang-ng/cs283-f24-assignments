using UnityEngine;

public class PlayerGK : MonoBehaviour
{
    public Transform pathStart;
    public Transform pathEnd;
    public float slideSpeed = 2f; 

    private bool isControlledByPlayer = false;
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
        
        if (!isControlledByPlayer)
        {
            SlideAlongPath();
        }
    }

    public void OnPlayerControl()
    {
        isControlledByPlayer = true;
    }

    public void OnPlayerRelease()
    {
        // resume automatic movement when player releases control
        isControlledByPlayer = false;
        targetPosition = FindNearestPointOnPath();
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

    private Vector3 FindNearestPointOnPath()
    {
        float distanceToStart = Vector3.Distance(transform.position, pathStart.position);
        float distanceToEnd = Vector3.Distance(transform.position, pathEnd.position);

        return (distanceToStart < distanceToEnd) ? pathStart.position : pathEnd.position;
    }
}