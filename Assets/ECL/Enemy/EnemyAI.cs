using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // Default target
    public Transform player; // Player reference
    public float rotationSpeed = 5f;
    public float speed = 3.5f;
    public float acceleration = 8f;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 120f;
    public LayerMask obstructionMask; // Set to block walls, etc.

    private NavMeshAgent agent;
    public Transform currentTarget;
    private bool playerVisible = false;

    private bool hasReachedTarget = false;
    private bool hasPerformedAction = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        currentTarget = target;
        InvokeRepeating("MoveToTarget", 0, 1);
        //MoveToTarget(target);

        // Start checking visibility every 1 second
        if (!hasReachedTarget)
        {
            StartCoroutine(CheckPlayerVisibility());
        }
    }

    IEnumerator CheckPlayerVisibility()
    {
        while (true)
        {
            bool wasVisible = playerVisible;
            playerVisible = IsPlayerVisible();

            if (playerVisible)
            {
                Debug.Log("Going to player");
                currentTarget = player;
                StartCoroutine(ResumeMoving());
                agent.isStopped = true;
            }
            else if (!playerVisible && wasVisible)
            {
                Debug.Log("Lost sight of player");
                StopMoving();
                yield return new WaitForSeconds(2f);
                agent.isStopped = false;
                currentTarget = target;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    private void Update()
    {
        //agent.SetDestination(currentTarget.position);

        if (!hasReachedTarget && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.velocity.magnitude == 0)
        {
            hasReachedTarget = true;
            OnReachedTarget();
        }
        if(agent.velocity.magnitude > 0 && agent.remainingDistance >= agent.stoppingDistance)
        {
            //StartCoroutine(ResumeMoving());
        }
    }

    bool IsPlayerVisible()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > fieldOfViewAngle * 0.5f)
        {
            return false;
        }

        // Raycast to check visibility
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, directionToPlayer);
        Debug.DrawRay(ray.origin, ray.direction * detectionRange, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
        {
            if (hit.transform == player)
            {
                agent.isStopped = false;
                //StartCoroutine(ResumeMoving());
                return true;
            }
        }

        return false;
    }

    private void MoveToTarget()
    {
        agent.SetDestination(currentTarget.position);
        /*
        if (newTarget == null) return;

        currentTarget = newTarget;
        */
        Debug.Log("Moving to: " + currentTarget.name);
    }

    private void StopMoving()
    {
        agent.isStopped = true; // Stop AI movement
    }

    IEnumerator ResumeMoving()
    {
        yield return new WaitForSeconds(2f);
        agent.isStopped = false;
        hasReachedTarget = false;
        hasPerformedAction = false;
        Debug.Log("Moving");
    }

    private void OnReachedTarget()
    {
        if (hasPerformedAction) return;
        hasPerformedAction = true;
        agent.isStopped = true;
        Debug.Log("Target Reached!");

        Attack();
    }

    private void Attack()
    {
        Debug.Log("Performing arrival action...");
    }
}
