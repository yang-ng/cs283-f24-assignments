using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI;

public class WanderBehavior : MonoBehaviour
{
    public Transform wanderRange;  // Set to a sphere
    private Root m_btRoot = BT.Root(); 

    void Start()
    {
        BTNode moveTo = BT.RunCoroutine(MoveToRandom);

        Sequence sequence = BT.Sequence();
        sequence.OpenBranch(moveTo);

        m_btRoot.OpenBranch(sequence);
    }

    void Update()
    {
        m_btRoot.Tick();
    }

    IEnumerator<BTState> MoveToRandom()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Generate a random point within the wander range
        Vector3 randomDirection = Random.insideUnitSphere * wanderRange.localScale.x;
        randomDirection += wanderRange.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRange.localScale.x, 1))
        {
            // Set the valid NavMesh position as the target destination
            agent.SetDestination(hit.position);

            // Wait for the agent to reach the destination
            while (agent.remainingDistance > 0.1f)
            {
                yield return BTState.Continue;
            }

            yield return BTState.Success;
        }
        else
        {
            // If no valid point is found, return failure
            yield return BTState.Failure;
        }
    }
}