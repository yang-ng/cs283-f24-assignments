using UnityEngine;

public class PlayerMotionController : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 5f;
    public float turnSpeed = 200f;
    private float speed;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        speed = 0f;
        Vector3 move = Vector3.zero;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            move += transform.forward * moveSpeed;
            speed = moveSpeed;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move -= transform.forward * moveSpeed;
            speed = moveSpeed;
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            isMoving = true;
        }

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("speed", speed);

        // use character controller
        if (controller != null)
        {
            controller.Move(move * Time.deltaTime);
        }

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            speed = 0f;
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }
}