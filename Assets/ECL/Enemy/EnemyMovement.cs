using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}