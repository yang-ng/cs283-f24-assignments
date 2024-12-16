using UnityEngine;
using System.Linq;

public class OpponentTeamBehavior : MonoBehaviour
{
    public enum Role { Defender, Striker }
    public Role opponentRole; // Defender or Striker

    public Transform defensivePosition;
    public Transform opponentGoal;
    public Transform playerGoal;
    public float speed = 0.5f;
    public float passRange = 3f;
    public float kickForce = 3f;

    private BallControlTracker ballControlTracker;
    private FieldManager fieldManager;
    private Transform ball;
    private Rigidbody ballRb;

    private Vector3 currentTarget;
    private string lastPossessor = "None";
    private bool hasTarget = false; 
    private bool isDribbling = false;

    private void Start()
    {
        ballControlTracker = FindObjectOfType<BallControlTracker>();
        fieldManager = FindObjectOfType<FieldManager>();
        ball = fieldManager.ball；
        ballRb = ball.GetComponent<Rigidbody>();
        currentTarget = transform.position;
    }

    private void Update()
    {
        string currentPossessor = ballControlTracker.GetCurrentPossessor();
        string ballArea = fieldManager.GetBallArea();

        if (currentPossessor != lastPossessor)
        {
            lastPossessor = currentPossessor;
            hasTarget = false;
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
                    HoldDefensivePosition();
                }
            }
            else
            {
                if (opponentRole == Role.Defender)
                {
                    HoldDefensivePosition();
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
                    GoToBallAndKick();
                }
                else if (opponentRole == Role.Striker)
                {
                    HoldDefensivePosition(); 
                }
            }
            else
            {
                if (opponentRole == Role.Defender)
                {
                    HoldDefensivePosition();
                }
                else if (opponentRole == Role.Striker)
                {
                    GoToBallAndKick();
                }
            }
        }
        else
        {
            if (opponentRole == Role.Defender)
            {
                HoldDefensivePosition();
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
        // determine the closer defender to the ball
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
                if (Vector3.Distance(transform.position, playerGoal.position) <= passRange)
                {
                    Shoot();
                }
                else
                {
                    currentTarget = playerGoal.position;
                    if (Vector3.Dot((currentTarget - ball.position).normalized, (playerGoal.position - ball.position).normalized) < 0)
                    {
                        currentTarget = ball.position + (playerGoal.position - ball.position).normalized * 1.5f;
                    }
                    DribbleBall();
                }
            }
            else
            {
                currentTarget = ball.position;
            }
        }

        else // if not the closest to the ball
        {
            Vector3 ballDirection = (playerGoal.position - ball.position).normalized;

            float randomAngle = Random.Range(30f, 90f);
            randomAngle *= Random.Range(0, 2) == 0 ? 1 : -1;
            Quaternion angleOffset = Quaternion.Euler(0, randomAngle, 0);
            Vector3 adjustedDirection = angleOffset * ballDirection;

            float randomDistance = Random.Range(1.5f, 2.5f);
            currentTarget = ball.position + adjustedDirection * randomDistance;

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

            Vector3 correctedDirection = Vector3.Lerp((targetPosition - ball.transform.position).normalized, directionToGoal, 0.3f).normalized;

            float dynamicDribbleForce = 1f * Mathf.Clamp(Vector3.Distance(ball.transform.position, targetPosition), 1f, 5f);
            ballRb.AddForce(correctedDirection * dynamicDribbleForce, ForceMode.Force);

            if (ballRb.velocity.magnitude > 1f)
            {
                ballRb.velocity = ballRb.velocity.normalized * 1f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == ball.gameObject)
        {
            isDribbling = true;
        }
    }

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
        var teammates = FindObjectsOfType<OpponentTeamBehavior>().Where(x => x.opponentRole == opponentRole);

        var closestTeammate = teammates.OrderBy(x => Vector3.Distance(x.transform.position, ball.position)).First();

        if (closestTeammate == this)
        {
            currentTarget = ball.position;

            if (Vector3.Distance(transform.position, ball.position) < 0.1f)
            {
                KickBall();
            }
        }
        else
        {
            Vector3 ballDirection = (playerGoal.position - ball.position).normalized;

            float randomAngle = Random.Range(-20f, 20f);
            Quaternion angleOffset = Quaternion.Euler(0, randomAngle, 0);
            Vector3 offsetDirection = angleOffset * ballDirection;

            currentTarget = ball.position + offsetDirection * 1f;
            currentTarget.y = transform.position.y;
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