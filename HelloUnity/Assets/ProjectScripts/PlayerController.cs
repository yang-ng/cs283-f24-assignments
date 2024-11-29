using UnityEngine;

public class GhostMotionController : MonoBehaviour
{
    public float moveSpeed = 5f; // forward movement speed
    public float turnSpeed = 200f; // rotation speed

    void Update()
    {
        // forward movement (W key)
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        // rotate left (A key)
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        // rotate right (D key)
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}