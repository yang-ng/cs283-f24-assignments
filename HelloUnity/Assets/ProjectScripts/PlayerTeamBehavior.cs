using UnityEngine;

public class PlayerTeamBehavior : MonoBehaviour
{
    public enum Role { Defender, Striker }
    public Role playerRole; // Assign role (Defender or Striker) in the Inspector

    public Transform defensivePosition; // Position to hold when defending
    public Transform opponentGoal; // Opponent's goal position
    public float speed = 0.5f; // Movement speed

    private BallControlTracker ballControlTracker; // Reference to the ball control tracker
    private FieldManager fieldManager; // Reference to the field manager
    private Transform ball; // Reference to the ball

    private Vector3 currentTarget; // Current target position
    private string lastPossessor = "None"; // Tracks the last known possessor of the ball
    private bool hasTarget = false; // Indicates if the player is moving toward a target

    private void Start()
    {
        ballControlTracker = FindObjectOfType<BallControlTracker>();
        fieldManager = FindObjectOfType<FieldManager>();
        ball = fieldManager.ball; // Get the ball transform
        currentTarget = transform.position; // Start with the current position
    }

    private void Update()
    {
        string currentPossessor = ballControlTracker.GetCurrentPossessor();

        if (currentPossessor != lastPossessor)
        {
            lastPossessor = currentPossessor;
            hasTarget = false; // Reset target if possession changes
        }

        if (currentPossessor == "Opponent")
        {
            if (playerRole == Role.Defender)
            {
                Defend();
            }
            else if (playerRole == Role.Striker)
            {
                Intercept();
            }
        }
        else if (currentPossessor == "Player")
        {
            if (playerRole == Role.Defender)
            {
                HoldDefensivePosition();
            }
            else if (playerRole == Role.Striker)
            {
                Attack();
            }
        }
        else
        {
            if (playerRole == Role.Defender)
            {
                HoldDefensivePosition();
            }
            else if (playerRole == Role.Striker)
            {
                Attack();
            }
        }

        MoveTowards(currentTarget);
    }

    private void Defend()
    {
        currentTarget = ball.position;
    }

    private void HoldDefensivePosition()
    {
        // If the player doesn't have a target or has reached the previous target, calculate a new random position
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;

            // Generate a random offset within a small radius around the defensive position
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.6f, 0.6f),
                0,
                Random.Range(-0.6f, 0.6f)
            );
            currentTarget = defensivePosition.position + randomOffset;

            // Ensure the y-coordinate matches the transform's current height
            currentTarget.y = transform.position.y;
        }

        // Update rotation to always face the ball
        Vector3 directionToBall = (ball.position - transform.position).normalized;
        if (directionToBall != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToBall);
        }

        // Move only toward the target position without affecting rotation
        transform.position = Vector3.Lerp(transform.position, currentTarget, Time.deltaTime * speed);
    }


    private void Attack()
    {
        // If the player doesn't have a target or has reached the previous target, calculate a new one
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;

            // Generate a random distance between 2 and 4
            float randomDistance = Random.Range(0.5f, 1.0f);

            // Direction towards the opponent's goal from the ball's position
            Vector3 goalDirection = (opponentGoal.position - ball.position).normalized;

            // Generate a random offset angle for a spread effect
            float randomAngle = Random.Range(-30f, 30f);
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
            Vector3 randomDirection = rotation * goalDirection;

            // Calculate the target position
            currentTarget = ball.position + randomDirection * randomDistance;

            // Ensure the y-coordinate matches the transform's current height
            currentTarget.y = transform.position.y;
        }
    }

    private void Intercept()
    {
        currentTarget = ball.position;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // Move toward the target position smoothly
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate to face the target
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
        }
    }
}