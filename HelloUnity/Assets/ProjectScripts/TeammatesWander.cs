using UnityEngine;

public class TeammatesWander : MonoBehaviour
{
    public float moveRange = 5f; // the range around the ball where ghosts can move
    public float speed = 2f; // speed of random movement
    private Vector3 targetPosition;

    void Start()
    {
        SetNewTargetPosition();
    }

    void Update()
    {
        MoveTowardsTarget();

        // set a new random target when close to the current one
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetNewTargetPosition();
        }
    }

    void SetNewTargetPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-moveRange, moveRange),
            0,
            Random.Range(-moveRange, moveRange)
        );
        targetPosition = GameObject.Find("Ball").transform.position + randomOffset;
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // rotate the ghost to face the direction of movement
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed); // Smooth rotation
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}