using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private EnemyAIAttack enemyAttack;

    [SerializeField] private Transform target;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfViewAngle = 120f;
    [SerializeField] private LayerMask obstructionLayer;

    public Transform currentTarget;
    private NavMeshAgent agent;
    private bool playerVisible = false;
    private bool hasReachedTarget = false;
    private Vector3 lastPos;
    private float threshold = 1f;
    private bool isAtTarget = false;
    private Rigidbody rb;
    void Start()
    {
        enemyAttack = GetComponentInChildren<EnemyAIAttack>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        InvokeRepeating("MoveToTarget", 0, 0.3f);
        StartCoroutine(CheckPlayerVisibility());
        currentTarget = target;
        rb = GetComponent<Rigidbody>();
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
                //rb.isKinematic = false;
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
        //rb.isKinematic = true;
        currentTarget = changeToo;
        hasReachedTarget = false;
        enemyAttack.player = currentTarget;
        yield return new WaitForSeconds(1f);
        if (!hasReachedTarget)
        {
            agent.isStopped = false;
            //rb.isKinematic = false;
            //Debug.Log("Attack False 1");
            enemyAttack.isAttacking = false;
        }
    }

    private void ExtraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), rotationSpeed * Time.deltaTime);
    }
    private void UpdateIfHasReachedTarget()
    {
        float remainingDistance = agent.remainingDistance;

        if (!hasReachedTarget && !isAtTarget && remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.velocity.magnitude == 0)
        {
            //Debug.Log("UpdateIfHasReachedTarget: Target Reached");
            agent.isStopped = true;
            //rb.isKinematic = true;
            OnReachedTarget();
        }
        else if (hasReachedTarget && isAtTarget && remainingDistance >= agent.stoppingDistance * 1.05f)
        {
            //Debug.Log("UpdateIfHasReachedTarget: Moving Away from Target");
            isAtTarget = false;
            hasReachedTarget = false;
            agent.isStopped = false;
            //rb.isKinematic = false;
        }
    }

    private void UpdateObjectPosition(Transform obj)
    {
        Vector3 offset = obj.position - lastPos;
        if (offset.x > threshold)
        {
            lastPos = obj.position;
            hasReachedTarget = false;
            //Debug.Log("Attack False 2");
            enemyAttack.isAttacking = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.position);
        }
        else if (offset.x < -threshold)
        {
            lastPos = obj.position;
            hasReachedTarget = false;
            //Debug.Log("Attack False 3");
            enemyAttack.isAttacking = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.position);
        }
    }

    private void MoveToTarget()
    {
        if (hasReachedTarget)
            return;
        isAtTarget = false;
        agent.isStopped = false;
        //rb.isKinematic = false;
        agent.SetDestination(currentTarget.position);
        //Debug.Log("Moving to: " + currentTarget.name);
    }
    private void OnReachedTarget()
    {
        if (hasReachedTarget || isAtTarget) return;
        hasReachedTarget = true;
        isAtTarget = true;
        agent.isStopped = true;
        //rb.isKinematic = true;
        //Debug.Log("Target Reached!");
        //Debug.Log("Attack True");
        enemyAttack.isAttacking = true;
    }
}