using UnityEngine;
using System.Linq; // For working with collections

public class OpponentTeamBehavior : MonoBehaviour
{
    public enum Role { Defender, Striker }
    public Role opponentRole; // Assign role (Defender or Striker) in the Inspector

    public Transform defensivePosition; // Position to hold when defending
    public Transform opponentGoal; // Reference to the opponent's goal
    public Transform playerGoal; // Reference to the player's goal
    public float speed = 0.5f; // Movement speed
    public float passRange = 3f; // Range within which the ball can be passed
    public float kickForce = 3f; // Force applied to the ball on kick

    private BallControlTracker ballControlTracker; // Reference to the ball control tracker
    private FieldManager fieldManager; // Reference to the field manager
    private Transform ball; // Reference to the ball
    private Rigidbody ballRb; // Rigidbody of the ball

    private Vector3 currentTarget; // Current target position
    private string lastPossessor = "None"; // Tracks the last known possessor of the ball
    private bool hasTarget = false; // Indicates if the player is moving toward a target
    private bool isDribbling = false; // Indicates whether the striker is currently dribbling the ball


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
        string ballArea = fieldManager.GetBallArea();

        if (currentPossessor != lastPossessor)
        {
            lastPossessor = currentPossessor;
            hasTarget = false; // Reset target if possession changes
        }

        if (currentPossessor == "Opponent")
        {
            if (ballArea == "OpponentDefensiveZone")
            {
                if (opponentRole == Role.Defender)
                {
                   HandleDefenderInDefensiveZone();
                }
                else if (opponentRole == Role.Striker)
                {
                    HoldDefensivePosition(); // DONE
                }
            }
            else
            {
                if (opponentRole == Role.Defender)
                {
                    HoldDefensivePosition(); // DONE
                }
                else if (opponentRole == Role.Striker)
                {
                   HandleStrikerOutsideDefensiveZone();
                }
            }
        }
        else if (currentPossessor == "Player")
        {
            if (ballArea == "OpponentDefensiveZone")
            {
                if (opponentRole == Role.Defender)
                {
                    GoToBallAndKick(); // DONE
                }
                else if (opponentRole == Role.Striker)
                {
                    HoldDefensivePosition(); // DONE
                }
            }
            else
            {
                if (opponentRole == Role.Defender)
                {
                    HoldDefensivePosition(); // DONE
                }
                else if (opponentRole == Role.Striker)
                {
                    GoToBallAndKick(); // DONE
                }
            }
        }
        else
        {
            if (opponentRole == Role.Defender)
            {
                HoldDefensivePosition(); // DONE
            }
            else if (opponentRole == Role.Striker)
            {
                HandleStrikerOutsideDefensiveZone();
            }
        }

        MoveTowards(currentTarget);
    }

    private void HandleDefenderInDefensiveZone()
    {
        // Determine the closer defender to the ball
        var allDefenders = FindObjectsOfType<OpponentTeamBehavior>().Where(x => x.opponentRole == Role.Defender);
        var closestDefender = allDefenders.OrderBy(x => Vector3.Distance(x.transform.position, ball.position)).First();

        if (closestDefender == this)
        {
            currentTarget = ball.position;
        }
        else
        {
            HoldDefensivePosition();
        }

        // If the ball is in possession, pass it to a striker
        if (isDribbling)
        {
            PassToClosestTeammate(Role.Striker);
        }
    }

    private void HandleStrikerOutsideDefensiveZone()
    {
        var allStrikers = FindObjectsOfType<OpponentTeamBehavior>().Where(x => x.opponentRole == Role.Striker);
        var closestStriker = allStrikers.OrderBy(x => Vector3.Distance(x.transform.position, ball.position)).First();

        if (closestStriker == this)
        {
            if (isDribbling)
            {
                // If within shooting range, shoot at the goal
                if (Vector3.Distance(transform.position, playerGoal.position) <= passRange)
                {
                    Shoot();
                }
                else
                {
                    // Move toward the player's goal and dribble
                    currentTarget = playerGoal.position;

                    // Prevent moving backward with strict forward validation
                    if (Vector3.Dot((currentTarget - ball.position).normalized, (playerGoal.position - ball.position).normalized) < 0)
                    {
                        currentTarget = ball.position + (playerGoal.position - ball.position).normalized * 1.5f;
                    }

                    DribbleBall();
                }
            }
            else
            {
                // Move toward the ball to start dribbling
                currentTarget = ball.position;
            }
        }

        else // If not the closest to the ball
        {
            Vector3 ballDirection = (playerGoal.position - ball.position).normalized;

            // Add a random angle to ensure strikers don't overlap
            float randomAngle = Random.Range(30f, 90f);
            randomAngle *= Random.Range(0, 2) == 0 ? 1 : -1;
            Quaternion angleOffset = Quaternion.Euler(0, randomAngle, 0);
            Vector3 adjustedDirection = angleOffset * ballDirection;

            float randomDistance = Random.Range(1.5f, 2.5f);
            currentTarget = ball.position + adjustedDirection * randomDistance;

            // Ensure the target remains forward
            if (Vector3.Dot((currentTarget - ball.position).normalized, (playerGoal.position - ball.position).normalized) < 0)
            {
                currentTarget = ball.position + ballDirection * randomDistance;
            }

            currentTarget.y = transform.position.y;
        }
    }


    private void Shoot()
    {
        if (ball != null)
        {
            Vector3 shootDirection = (playerGoal.position - ball.position).normalized;
            ballRb.AddForce(shootDirection * kickForce, ForceMode.Impulse);
            BallControlTracker tracker = FindObjectOfType<BallControlTracker>();
            tracker.RegisterKick("Opponent");
            isDribbling = false;
        }
    }

    void DribbleBall()
    {
        if (ball != null)
        {
            Vector3 targetPosition = transform.position + transform.forward * 0.1f;
            targetPosition.y = ball.transform.position.y;

            Vector3 directionToGoal = (playerGoal.position - ball.transform.position).normalized;

            // Add slight correction toward the goal to prevent backward dribbling
            Vector3 correctedDirection = Vector3.Lerp((targetPosition - ball.transform.position).normalized, directionToGoal, 0.3f).normalized;

            float dynamicDribbleForce = 1f * Mathf.Clamp(Vector3.Distance(ball.transform.position, targetPosition), 1f, 5f);
            ballRb.AddForce(correctedDirection * dynamicDribbleForce, ForceMode.Force);

            // Limit ball velocity to prevent drifting
            if (ballRb.velocity.magnitude > 1f)
            {
                ballRb.velocity = ballRb.velocity.normalized * 1f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the striker collides with the ball, start dribbling
        if (collision.gameObject == ball.gameObject)
        {
            isDribbling = true;
        }
    }

    // private void OnCollisionExit(Collision collision)
    // {
    //     // Stop dribbling if the ball is no longer in contact
    //     if (collision.gameObject == ball.gameObject)
    //     {
    //         isDribbling = false;
    //     }
    // }

    private void HoldDefensivePosition()
    {
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.6f, 0.6f),
                0,
                Random.Range(-0.6f, 0.6f)
            );
            currentTarget = defensivePosition.position + randomOffset;
            currentTarget.y = transform.position.y;
        }

        Vector3 directionToBall = (ball.position - transform.position).normalized;
        if (directionToBall != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToBall);
        }
    }

    private void GoToBallAndKick()
    {
        // Find all opponent strikers
        var teammates = FindObjectsOfType<OpponentTeamBehavior>().Where(x => x.opponentRole == opponentRole);

        // Determine the closest striker to the ball
        var closestTeammate = teammates.OrderBy(x => Vector3.Distance(x.transform.position, ball.position)).First();

        if (closestTeammate == this)
        {
            // If this is the closest striker, go directly to the ball
            currentTarget = ball.position;

            if (Vector3.Distance(transform.position, ball.position) < 0.1f)
            {
                KickBall();
            }
        }
        else
        {
            // If this is not the closest striker, go to a support position near the ball
            Vector3 ballDirection = (playerGoal.position - ball.position).normalized;

            // Offset by a random angle to spread out the strikers
            float randomAngle = Random.Range(-20f, 20f); // Small angle offset for support
            Quaternion angleOffset = Quaternion.Euler(0, randomAngle, 0);
            Vector3 offsetDirection = angleOffset * ballDirection;

            // Set the target position near the ball but offset
            currentTarget = ball.position + offsetDirection * 1f; // Stay 1.5 units away from the ball
            currentTarget.y = transform.position.y; // Ensure y-coordinate is correct
        }
    }

    private void PassToClosestTeammate(Role role)
    {
        var teammates = FindObjectsOfType<OpponentTeamBehavior>().Where(x => x.opponentRole == role);
        var closestTeammate = teammates.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();

        if (Vector3.Distance(transform.position, closestTeammate.transform.position) <= passRange)
        {
            Vector3 passDirection = (closestTeammate.transform.position - ball.position).normalized;
            ballRb.AddForce(passDirection * kickForce * 0.1f, ForceMode.Impulse);
        }
    }


    private void KickBall()
    {
        Vector3 kickDirection = (playerGoal.position - ball.position).normalized;
        ballRb.AddForce(kickDirection * kickForce * 0.1f, ForceMode.Impulse);

        BallControlTracker tracker = FindObjectOfType<BallControlTracker>();
        tracker.RegisterKick("Opponent");
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

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