using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfViewAngle = 120f;
    [SerializeField] private LayerMask obstructionLayer;

    private Transform currentTarget;
    private NavMeshAgent agent;
    private bool playerVisible = false;
    private bool hasReachedTarget = false;
    private Vector3 lastPos;
    private float threshold = 1f;
    private bool isAtTarget = false;
    //private bool hasPerformedAction = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        InvokeRepeating("MoveToTarget", 0, 0.3f);
        StartCoroutine(CheckPlayerVisibility());
        currentTarget = target;
        // update the target to go to
        // update target location
    }
    void Update()
    {
        ExtraRotation();
        UpdateIfHasReachedTarget();
        if (currentTarget == player)
            UpdateObjectPosition(player);
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
                return true;
            }
        }
        return false;
    }
    IEnumerator CheckPlayerVisibility()
    {
        while (true)
        {
            bool wasVisible = playerVisible;
            playerVisible = IsPlayerVisible();

            if (playerVisible && !wasVisible)
            {
                Debug.Log("Found player");
                StartCoroutine(ChangeCurrentTarget(player));
            }
            else if (!playerVisible && wasVisible)
            {
                Debug.Log("Lost sight of player");
                StartCoroutine(ChangeCurrentTarget(target));
            }
            yield return new WaitForSeconds(0.8f);
        }
    }
    IEnumerator ChangeCurrentTarget(Transform changeToo)
    {
        agent.isStopped = true;
        currentTarget = changeToo;
        hasReachedTarget = false;
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    private void ExtraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), rotationSpeed * Time.deltaTime);
    }
    private void UpdateIfHasReachedTarget()
    {
        if (!hasReachedTarget && !isAtTarget && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.velocity.magnitude == 0)
        {
            Debug.Log("UpdateIfHasReachedTarget");
            OnReachedTarget();
        }
        else if (hasReachedTarget && isAtTarget && agent.remainingDistance >= agent.stoppingDistance)//agent.velocity.magnitude > 0 && agent.remainingDistance >= agent.stoppingDistance && isAtTarget)
        {
            Debug.Log("UpdateIfHasReachedTarget 2");
            isAtTarget = false;
            hasReachedTarget = false;
            agent.isStopped = false;
        }
    }
    private void UpdateObjectPosition(Transform obj)
    {
        Vector3 offset = obj.position - lastPos;
        if (offset.x > threshold)
        {
            lastPos = obj.position; // update lastPos
            hasReachedTarget = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.position);// code to execute when X is getting bigger
        }
        else if (offset.x < -threshold)
        {
            lastPos = obj.position; // update lastPos
            hasReachedTarget = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.position);// code to execute when X is getting smaller 
        }
    }
    private void MoveToTarget()
    {
        if (hasReachedTarget) return;
        isAtTarget = false;
        agent.isStopped = false;
        agent.SetDestination(currentTarget.position);
        Debug.Log("Moving to: " + currentTarget.name);
    }
    private void OnReachedTarget()
    {
        if (hasReachedTarget || isAtTarget) return;
        hasReachedTarget = true;
        isAtTarget = true;
        agent.isStopped = true;
        Debug.Log("Target Reached!");
        // Attack and whatever
    }
}