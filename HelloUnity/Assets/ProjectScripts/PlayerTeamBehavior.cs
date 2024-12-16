using UnityEngine;

public class PlayerTeamBehavior : MonoBehaviour
{
    public enum Role { Defender, Striker }
    public Role playerRole; // Defender or Striker

    public Transform defensivePosition;
    public Transform opponentGoal;
    public float speed = 0.5f;

    private BallControlTracker ballControlTracker;
    private FieldManager fieldManager;
    private Transform ball;

    private Vector3 currentTarget;
    private string lastPossessor = "None";
    private bool hasTarget = false;

    private void Start()
    {
        ballControlTracker = FindObjectOfType<BallControlTracker>();
        fieldManager = FindObjectOfType<FieldManager>();
        ball = fieldManager.ball;
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
            if (ballArea == "PlayerDefensiveZone")
            {
                if (playerRole == Role.Defender)
                {
                    Defend();
                }
                else if (playerRole == Role.Striker)
                {
                    HoldDefensivePosition();
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
                    Intercept();
                }
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

        // update rotation to always face the ball
        Vector3 directionToBall = (ball.position - transform.position).normalized;
        if (directionToBall != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToBall);
        }

        transform.position = Vector3.Lerp(transform.position, currentTarget, Time.deltaTime * speed);
    }

    private void Attack()
    {
        if (!hasTarget || Vector3.Distance(transform.position, currentTarget) < 0.5f)
        {
            hasTarget = true;
            Vector3 goalDirection = (opponentGoal.position - ball.position).normalized;

            float randomAngle;
            float randomDistance;
            if (fieldManager.GetBallArea() == "OpponentDefensiveZone")
            {
                randomAngle = Random.Range(60f, 90f);
                randomDistance = Random.Range(1f, 2f);
            }
            else
            {
                randomAngle = Random.Range(30f, 90f);
                randomDistance = Random.Range(1.5f, 2.5f);
            }

            randomAngle *= Random.Range(0, 2) == 0 ? 1 : -1; // left or right
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
            Vector3 angledDirection = rotation * goalDirection;
            currentTarget = ball.position + angledDirection * randomDistance;
            currentTarget.y = transform.position.y;
        }
    }

    private void Intercept()
    {
        currentTarget = ball.position;
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