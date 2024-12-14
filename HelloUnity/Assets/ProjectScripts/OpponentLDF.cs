using UnityEngine;
using System.Collections.Generic;

public class OpponentLeftDefender : MonoBehaviour
{
    public Transform fixedSpot; // The defender's wandering spot
    public float wanderRange = 2f; // Range of wandering around the fixed spot
    public float moveSpeed = 3f; // Movement speed
    public float passForce = 10f; // Force for passing the ball
    public Transform midfieldTarget; // Target for dribbling towards midfield

    private BallControlTracker ballControlTracker; // Reference to BallControlTracker
    private FieldManager fieldManager; // Reference to FieldManager
    private Rigidbody ballRb; // Rigidbody of the ball
    private bool isDribbling = false; // Is the defender currently dribbling?
    private GameObject closestTeammate; // Closest teammate for passing

    void Start()
    {
        ballControlTracker = FindObjectOfType<BallControlTracker>();
        fieldManager = FindObjectOfType<FieldManager>();
        ballRb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
    }

    void Update()
    {
        string ballArea = fieldManager.GetBallArea();
        string ballPossessor = ballControlTracker.GetCurrentPossessor();

        if (ballPossessor == "Player") // Player's team is possessing
        {
            if (ballArea != "OpponentDefensiveZone")
            {
                WanderAroundFixedSpot();
            }
            else
            {
                GoTowardsBall();
            }
        }
        else if (ballPossessor == "Opponent") // Opponent's team is possessing
        {
            if (IsBallInPossession())
            {
                DribbleToMidfield();
            }
            else
            {
                WanderAroundFixedSpot();
            }
        }
    }

    private void WanderAroundFixedSpot()
    {
        Vector3 randomSpot = fixedSpot.position + new Vector3(
            Random.Range(-wanderRange, wanderRange),
            0,
            Random.Range(-wanderRange, wanderRange)
        );

        MoveTo(randomSpot);
    }

    private void GoTowardsBall()
    {
        MoveTo(ballRb.transform.position);
    }

    private void DribbleToMidfield()
    {
        if (!isDribbling)
        {
            isDribbling = true;
            Debug.Log("Defender is dribbling to midfield.");
        }

        MoveTo(midfieldTarget.position);

        // Check if the ball reached the midfield
        if (fieldManager.GetBallArea() == "Midfield")
        {
            PassToClosestTeammate();
        }
    }

    private void PassToClosestTeammate()
    {

        // Find the closest teammate
        closestTeammate = FindClosestTeammateInFront();
        if (closestTeammate != null)
        {
            Vector3 directionToTeammate = (closestTeammate.transform.position - ballRb.transform.position).normalized;
            ballRb.AddForce(directionToTeammate * passForce, ForceMode.Impulse);
        }

        isDribbling = false; // Reset dribbling
    }

    private GameObject FindClosestTeammateInFront()
    {
        GameObject[] teammates = GameObject.FindGameObjectsWithTag("Opponent");
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject teammate in teammates)
        {
            if (teammate == gameObject) continue;

            float distance = Vector3.Distance(transform.position, teammate.transform.position);
            if (distance < closestDistance && IsInFront(teammate))
            {
                closest = teammate;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private bool IsInFront(GameObject teammate)
    {
        Vector3 toTeammate = teammate.transform.position - transform.position;
        return Vector3.Dot(transform.right, toTeammate.normalized) > 0.5f;
    }

    private void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private bool IsBallInPossession()
    {
        return ballControlTracker.CurrentDribbler == "Opponent" && ballRb.transform.position == transform.position;
    }
}