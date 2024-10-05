using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // parameters for linear speed and turning speed
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // move backward
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}
