using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform player;
    public float rotationSpeed;
    public float speed;
    public float acceleration;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 120f;
    public LayerMask obstructionMask;

    private NavMeshAgent agent;
    private bool hasReachedTarget = false;
    private bool hasPerformedAction = false;
    public Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        currentTarget = target;
        MoveToTarget(target);
    }

    void Update()
    {
        if (player == null || target == null)
            return;

        if (IsPlayerVisible())
        {
            currentTarget = player;
            MoveToTarget(player);
        }
        else
        {
            currentTarget = target;
            //MoveToTarget(target);
        }

        //agent.SetDestination(currentTarget.position);

        if (!hasReachedTarget && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.velocity.magnitude == 0)
        {
            hasReachedTarget = true;
            OnReachedTarget();
        }
    }

    bool IsPlayerVisible()
    {
        if (player == null) return false;  // Ensure player reference is valid

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Visualize the raycast (for debugging)
        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);

        if (detectionRange > 0 && Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange, obstructionMask))
        {
            return hit.transform == player;
        }

        return false;
    }

    private void MoveToTarget(Transform newTarget)
    {
        agent.isStopped = false;
        agent.SetDestination(newTarget.position);
        hasReachedTarget = false;
        hasPerformedAction = false;
    }

    private void OnReachedTarget()
    {
        if (hasPerformedAction) return;
        hasPerformedAction = true;
        agent.isStopped = true;
        Debug.Log("Target Reached!");

        PerformActionOnArrival();
    }

    private void PerformActionOnArrival()
    {
        Debug.Log("Performing arrival action...");
    }
}