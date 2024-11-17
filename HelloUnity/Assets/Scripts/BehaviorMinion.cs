using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTAI;

public class BehaviorMinion : MonoBehaviour
{
    public Transform player;
    public Transform playerHome; // player's safe zone
    public Transform npcBase; // NPC's base position
    public GameObject bullet; // bullet to shoot
    public Transform firePoint; // point where the bullet is fired
    public float attackRange = 10f; // distance within the minion will attack
    public float playerSafeRadius = 10f; // radius of the player's safe zone
    public float followDistance = 20f; // distance where the minion will follow the player
    public float attackInterval = 1f; // interval between attacks
    public float bulletSpeed = 10f; // speed of the bullet
    public float npcSpeed = 3.5f; // speed of the NPC
    private Root m_btRoot = BT.Root();
    private float nextAttackTime = 0f;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isRotating = false; // tracks if the NPC is currently rotating
    private bool isChasing = false; // tracks if the NPC is actively chasing the player

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = npcSpeed;

        animator = GetComponent<Animator>();

        m_btRoot.OpenBranch(
            BT.Selector().OpenBranch(
                BT.Sequence().OpenBranch(
                    BT.Condition(() => PlayerInSafeZone()),
                    BT.Call(() => RetreatToBase())
                ),
                BT.Sequence().OpenBranch(
                    BT.Condition(() => PlayerInAttackRange()),
                    BT.Call(() => RotateToFacePlayer()),
                    BT.Call(() => Attack())
                ),
                BT.Sequence().OpenBranch(
                    BT.Condition(() => ShouldChasePlayer()),
                    BT.Call(() => StartChase())
                ),
                BT.Call(() => StopChase()),
                BT.Call(() => IdleAtBase())
            )
        );
    }

    void Update()
    {
        m_btRoot.Tick();
        UpdateAnimationState();
    }

    bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    bool PlayerInSafeZone()
    {
        return Vector3.Distance(player.position, playerHome.position) <= playerSafeRadius;
    }

    // check if the player is far enough to follow
    bool ShouldChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer > attackRange
               && distanceToPlayer <= followDistance + 1f // add a small buffer
               && !PlayerInSafeZone();
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Shoot();
            nextAttackTime = Time.time + attackInterval;
        }
    }

    void Shoot()
    {
        if (bullet != null && firePoint != null)
        {
            GameObject projectile = Instantiate(bullet, firePoint.position, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (player.position - firePoint.position);
                direction.y = 0;
                direction = direction.normalized;

                rb.velocity = direction * bulletSpeed;
            }

            Destroy(projectile, 3f);
        }
    }

    void RetreatToBase()
    {
        isChasing = false;
        agent.SetDestination(npcBase.position);
        agent.isStopped = false;
    }

    void StartChase()
    {
        isChasing = true;
        agent.SetDestination(player.position);
        agent.isStopped = false;
    }

    void StopChase()
    {
        if (!isChasing) return;
        isChasing = false;
        agent.isStopped = true;
    }

    void IdleAtBase()
    {
        if (isChasing) return;
        agent.isStopped = true;
        isRotating = false;
    }

    void RotateToFacePlayer()
    {
        if (player == null || isRotating) return;

        isRotating = true;
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Ignore Y-axis
        StartCoroutine(SmoothRotation(lookRotation));
    }

    IEnumerator SmoothRotation(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null;
        }
        isRotating = false;
    }

    // update the animation state based on NPC's movement and rotation
    void UpdateAnimationState()
    {
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;

            // if not moving and not rotating, set idle
            if (!isMoving && !isRotating && !isChasing)
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                animator.SetBool("isMoving", true);
            }
        }
    }
}