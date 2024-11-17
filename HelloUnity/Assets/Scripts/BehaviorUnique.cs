using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTAI;

public class BehaviorUnique : MonoBehaviour
{
    public Transform player; // player
    public float followDistance = 2f; // minimum distance to maintain from the player
    public float moveSpeed = 3.5f; // speed of helper
    private NavMeshAgent agent;
    private Root behaviorTree = BT.Root();
    private bool shouldFollowPlayer = false; // whether helper should follow
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = followDistance;

        animator = GetComponent<Animator>();

        // ensure helper starts idle
        agent.isStopped = true;
        SetIdleAnimation();

        behaviorTree.OpenBranch(
            BT.Call(() => FollowPlayer())
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            shouldFollowPlayer = !shouldFollowPlayer;

            if (!shouldFollowPlayer)
            {
                agent.isStopped = true;
                SetIdleAnimation();
            }
        }

        behaviorTree.Tick();

        if (shouldFollowPlayer)
        {
            PreventOverlap();
        }
    }

    void FollowPlayer()
    {
        if (player != null && shouldFollowPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > followDistance)
            {
                agent.SetDestination(player.position);
                agent.isStopped = false;
                SetRunningAnimation();
            }
            else
            {
                agent.isStopped = true;
                SetIdleAnimation();
            }
        }
    }

    void PreventOverlap()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // if helper is closer than the follow distance, move it back
            if (distanceToPlayer < followDistance)
            {
                Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
                transform.position += directionAwayFromPlayer * (followDistance - distanceToPlayer);
            }
        }
    }

    void SetRunningAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
    }

    void SetIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }
}