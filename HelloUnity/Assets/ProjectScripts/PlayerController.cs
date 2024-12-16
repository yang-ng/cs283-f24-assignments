using UnityEngine;

public class GhostMotionController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;

    void Update()
    {
        if (!GameManager.Instance.IsGameRunning())
        {
            return;
        }
        
        // forward movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        // rotate left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        // rotate right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}