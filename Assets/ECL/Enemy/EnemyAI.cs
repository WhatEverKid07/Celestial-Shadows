using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed;
    public float speed;
    public float acceleration;

    private NavMeshAgent agent;
    private bool hasReachedTarget = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;

        if (target != null)
        {
            MoveToTarget();
        }
    }

    void Update()
    {
        if (target == null) return;

        // Keep updating the target position dynamically
        if (!hasReachedTarget)
        {
            agent.SetDestination(target.position);
        }

        // Check if the agent reached the target
        if (!hasReachedTarget && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            OnReachedTarget();
        }

        MovementUpdate();
    }

    private void MoveToTarget()
    {
        if (target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            hasReachedTarget = false;
        }
    }

    private void MovementUpdate()
    {
        if (agent.hasPath)
        {
            Vector3 lookRotation = agent.steeringTarget - transform.position;
            if (lookRotation != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnReachedTarget()
    {
        hasReachedTarget = true;
        agent.isStopped = true;
        Debug.Log("Target Reached!");

        // Call a custom method ONCE when the target is reached
        PerformActionOnArrival();
    }

    private void PerformActionOnArrival()
    {
        Debug.Log("Performing arrival action...");
    }

    private void LateUpdate()
    {
        // If moved manually or by force, resume movement
        if (hasReachedTarget && Vector3.Distance(transform.position, target.position) > agent.stoppingDistance)
        {
            MoveToTarget();
        }
    }
}
