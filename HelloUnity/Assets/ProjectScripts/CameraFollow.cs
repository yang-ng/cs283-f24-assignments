using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform ball;
    public float followSpeed = 5f;
    public float leftBoundary = -15f;
    public float rightBoundary = 15f;

    void LateUpdate()
    {
        if (ball != null)
        {
            Vector3 targetPosition = new Vector3(ball.position.x, transform.position.y, transform.position.z); // follow the ball along the X-axis
            targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}