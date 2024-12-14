using UnityEngine;

public class OpponentTeamBehavior : MonoBehaviour
{
    public enum Role { Defender, Striker }
    public Role playerRole; // Assign role (Defender or Striker) in the Inspector

    public Transform defensivePosition; // Position to hold when defending
    public Transform playerGoal; // Player's goal position
    public float speed = 3f; // Movement speed
    public float passRange = 5f; // Range within which passes are made
    public float shootRange = 3f; // Range within which shots are taken

    private BallControlTracker ballControlTracker; // Reference to the ball control tracker
    private FieldManager fieldManager; // Reference to the field manager
    private Transform ball; // Reference to the ball
    private Rigidbody ballRb; // Rigidbody of the ball
    private Vector3 currentTarget; // Current target position
    private string lastPossessor = "None"; // Tracks the last known possessor of the ball
    private bool hasTarget = false; // Indicates if the player is moving toward a target

    private void Start()
    {
        ballControlTracker = FindObjectOfType<BallControlTracker>();
        fieldManager = FindObjectOfType<FieldManager>();
        ball = fieldManager.ball; // Get the ball transform
        ballRb = ball.GetComponent<Rigidbody>();
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
                HoldDefensivePosition(); // Defenders mimic player's defenders when possessing
            }
            else if (playerRole == Role.Striker)
            {
                AttackOrPass(); // Strikers dribble, pass, or shoot
            }
        }
        else if (currentPossessor == "Player")
        {
            if (playerRole == Role.Defender)
            {
                DefenderDefend(); // Defend against player's attack
            }
            else if (playerRole == Role.Striker)
            {
                StrikerDefend(); // Strikers fall back to defensive positions
            }
        }
        else
        {
            if (playerRole == Role.Defender)
            {
                HoldDefensivePosition(); // Neutral behavior for defenders
            }
            else if (playerRole == Role.Striker)
            {
                StrikerDefend();
            }
        }

        MoveTowards(currentTarget);
    }

    private void DefenderDefend()
    {
        string ballArea = fieldManager.GetBallArea();

        if (ballArea == "OpponentDefensiveZone")
        {
            // Move toward the ball to defend if it is in the opponent's defensive zone
            currentTarget = ball.position;
        }
        else
        {
            // Hold defensive position if the ball is not in the opponent's defensive zone
            HoldDefensivePosition();
        }
    }

    private void StrikerDefend()
    {
        string ballArea = fieldManager.GetBallArea();

        if (ballArea == "Midfield")
        {
            // Move toward the ball to defend if it is in the midfield
            currentTarget = ball.position;
        }
        else
        {
            // Hold defensive position if the ball is not in the midfield
            HoldDefensivePosition();
        }
    }

    private void HoldDefensivePosition()
    {
        // If the player doesn't have a target or has reached the previous target, calculate a new random position
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;

            // Generate a random offset within a small radius around the defensive position
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            );
            currentTarget = defensivePosition.position + randomOffset;

            // Ensure the y-coordinate matches the transform's current height
            currentTarget.y = transform.position.y;
        }

        // Always face the ball
        Vector3 directionToBall = (ball.position - transform.position).normalized;
        if (directionToBall != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToBall);
        }
    }

    private void AttackOrPass()
    {
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;

            if (Vector3.Distance(transform.position, playerGoal.position) <= shootRange)
            {
                Shoot(); // If close to the goal, shoot
            }
            else
            {
                // Calculate a random target slightly forward toward the player's goal
                Vector3 goalDirection = (playerGoal.position - ball.position).normalized;
                currentTarget = ball.position + goalDirection * Random.Range(2f, 4f);
                currentTarget.y = transform.position.y; // Match height

                // If a teammate is within pass range, pass the ball
                PassToTeammate();
            }
        }

        // Always face the ball while moving
        Vector3 directionToBall = (ball.position - transform.position).normalized;
        if (directionToBall != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToBall);
        }

        // Start dribbling if close to the ball
        if (Vector3.Distance(transform.position, ball.position) < 1f)
        {
            DribbleBall();
        }
    }

    private void DribbleBall()
    {
        if (ball != null)
        {
            // Calculate the target position slightly in front of the striker
            Vector3 targetPosition = transform.position + transform.forward * 1.5f; // Adjust offset as needed

            // Keep the ball on the ground
            targetPosition.y = ball.transform.position.y;

            // Calculate the direction to the target position
            Vector3 direction = (targetPosition - ball.transform.position).normalized;

            // Apply force to move the ball toward the target position
            float distanceToTarget = Vector3.Distance(ball.transform.position, targetPosition);
            float dynamicDribbleForce = 5f * Mathf.Clamp(distanceToTarget, 1f, 5f); // Adjust force as needed

            ballRb.AddForce(direction * dynamicDribbleForce, ForceMode.Force);

            // Actively correct the ball's velocity to prevent outward drift during rotation
            Vector3 correctedVelocity = (targetPosition - ball.transform.position).normalized * 5f;
            ballRb.velocity = Vector3.Lerp(ballRb.velocity, correctedVelocity, Time.deltaTime * 10f);

            // Limit the ball's overall velocity
            if (ballRb.velocity.magnitude > 5f)
            {
                ballRb.velocity = ballRb.velocity.normalized * 5f;
            }
        }
    }

    private void PassToTeammate()
    {
        OpponentTeamBehavior[] teammates = FindObjectsOfType<OpponentTeamBehavior>();

        foreach (var teammate in teammates)
        {
            if (teammate != this && teammate.playerRole == Role.Striker &&
                Vector3.Distance(transform.position, teammate.transform.position) <= passRange)
            {
                // Pass the ball to the teammate
                Vector3 passDirection = (teammate.transform.position - ball.position).normalized;
                ballRb.AddForce(passDirection * 5f, ForceMode.Impulse); // Adjust force as needed
                Debug.Log("Pass to teammate!");
                break;
            }
        }
    }

    private void Shoot()
    {
        Vector3 shootDirection = (playerGoal.position - ball.position).normalized;
        ballRb.AddForce(shootDirection * 10f, ForceMode.Impulse); // Adjust force as needed
        Debug.Log("Shoot at the goal!");
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
