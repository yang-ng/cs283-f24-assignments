using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
    public float wanderRange = 10f;
    private NavMeshAgent agent;
    private Vector3 lastPosition;
    private float stuckCheckTimer = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
        lastPosition = transform.position;
    }

    void Update()
    {
        // check if arrives target
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewDestination();
        }

        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckCheckTimer -= Time.deltaTime;
            if (stuckCheckTimer <= 0)
            {
                SetNewDestination();
                stuckCheckTimer = 2f;
            }
        }
        else
        {
            stuckCheckTimer = 2f;
        }

        lastPosition = transform.position;
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRange, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}