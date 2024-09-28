using UnityEngine;

public class Tour : MonoBehaviour
{
    public Transform[] pointsOfInterest;
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 2.0f;

    private int currentPOIIndex = -1;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float transitionProgress = 0f;

    void Start()
    {
        if (pointsOfInterest == null || pointsOfInterest.Length == 0)
        {
            Debug.LogError("No POIs set.");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && !isMoving && pointsOfInterest.Length > 0)
        {
            SetNextPOI();
        }

        if (isMoving)
        {
            PerformTransition();
        }
    }

    void SetNextPOI()
    {
        currentPOIIndex = (currentPOIIndex + 1) % pointsOfInterest.Length;
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetPosition = pointsOfInterest[currentPOIIndex].position;
        targetRotation = pointsOfInterest[currentPOIIndex].rotation;
        transitionProgress = 0f;
        isMoving = true;
        Debug.Log("Moving to POI " + currentPOIIndex);
    }

    void PerformTransition()
    {
        transitionProgress += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPosition, targetPosition, transitionProgress);
        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, transitionProgress);

        if (transitionProgress >= 1.0f)
        {
            isMoving = false;
            Debug.Log("Arrived at POI " + currentPOIIndex);
        }
    }
}
