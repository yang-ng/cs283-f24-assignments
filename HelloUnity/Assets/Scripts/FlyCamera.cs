using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float movementSpeed = 10f;  // Speed of the camera movement
    public float lookSpeed = 3f;       // Speed of camera rotation

    private float yaw = 0f;            // Horizontal rotation (left/right)
    private float pitch = 0f;          // Vertical rotation (up/down)

    void Update()
    {
        // Mouse Look: Get mouse input to rotate the camera
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        // Apply the rotation
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Camera Movement: Get WASD input to move the camera
        float moveForward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        // Move the camera relative to its current rotation
        transform.Translate(moveRight, 0, moveForward);
    }
}
