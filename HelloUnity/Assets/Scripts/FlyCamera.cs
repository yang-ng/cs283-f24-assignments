using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float lookSpeed = 3f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        // get mouse input to rotate the camera
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        // apply the rotation
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // get WASD input to move the camera
        float moveForward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        // move the camera relative to its current rotation
        transform.Translate(moveRight, 0, moveForward);
    }
}
