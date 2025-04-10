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
        lastPos = player.position;
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

        // Target the center of the player's body (adjust Y if needed)
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 targetPosition = player.position + Vector3.up * 1.0f;
        Vector3 directionToPlayer = (targetPosition - eyePosition).normalized;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Debug angle and direction
        //Debug.Log($"[AI] Angle to Player: {angleToPlayer}");

        // Add a small buffer to avoid edge flakiness
        if (angleToPlayer > fieldOfViewAngle * 0.5f + 2f)
        {
            //Debug.Log("[AI] Player out of FOV");
            return false;
        }

        // Visualize raycast
        Debug.DrawRay(eyePosition, directionToPlayer * detectionRange, Color.red);

        // Check if the ray hits the player
        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, detectionRange, ~obstructionLayer))
        {
            //Debug.Log($"[AI] Raycast hit: {hit.transform.name}");

            if (hit.transform == player)
            {
                agent.isStopped = false;
                return true;
            }
            else
            {
                //Debug.Log("[AI] Player blocked by: " + hit.transform.name);
            }
        }
        else
        {
            //Debug.Log("[AI] Raycast didn't hit anything");
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

        if (!hasReachedTarget && !isAtTarget && remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.hasPath)
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