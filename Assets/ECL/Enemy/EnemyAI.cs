using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private EnemyAIAttack enemyAttack;

    public GameObject target;
    public GameObject player;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfViewAngle = 120f;
    [SerializeField] private LayerMask obstructionLayer;

    public GameObject currentTarget;
    private NavMeshAgent agent;
    private bool playerVisible = false;
    private bool hasReachedTarget = false;
    private Vector3 lastPos;
    private float threshold = 1f;
    private bool isAtTarget = false;
    private Rigidbody rb;
    private bool the = true;
    private void Awake()
    {
        enemyAttack = GetComponentInChildren<EnemyAIAttack>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Target");
        player = GameObject.Find("PlayerPhys");
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        agent.speed = speed;
        agent.acceleration = acceleration;
        InvokeRepeating("MoveToTarget", 0, 0.3f);
        StartCoroutine(CheckPlayerVisibility());
        currentTarget = target;
        lastPos = player.transform.position;
    }
    void Update()
    {
        ExtraRotation();
        UpdateIfHasReachedTarget();
        if (currentTarget == player)
            UpdateObjectPosition(player);
        //Debug.DrawRay(transform.position + Vector3.up, (player.transform.position + Vector3.up) - (transform.position + Vector3.up), Color.green);

    }
    bool IsPlayerVisible()
    {
        if (player == null) return false;

        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Collider playerCollider = player.GetComponentInChildren<Collider>();
        Vector3 targetPosition = playerCollider.bounds.center;
        Vector3 directionToPlayer = (targetPosition - eyePosition).normalized;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle * 0.5f + 2f)
            return false;

        Debug.DrawRay(eyePosition, directionToPlayer * detectionRange, Color.green);

        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                // Ignore self
                return false;
            }

            if (hit.transform == player || hit.transform.IsChildOf(player.transform))
            {
                return true;
            }

            Debug.Log("[AI] View blocked by: " + hit.transform.name);
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
    IEnumerator ChangeCurrentTarget(GameObject changeToo)
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

    private void UpdateObjectPosition(GameObject obj)
    {
        Vector3 offset = obj.transform.position - lastPos;
        if (offset.x > threshold)
        {
            lastPos = obj.transform.position;
            hasReachedTarget = false;
            //Debug.Log("Attack False 2");
            enemyAttack.isAttacking = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.transform.position);
        }
        else if (offset.x < -threshold)
        {
            lastPos = obj.transform.position;
            hasReachedTarget = false;
            //Debug.Log("Attack False 3");
            enemyAttack.isAttacking = false;
            MoveToTarget();
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    private void MoveToTarget()
    {
        if (hasReachedTarget)
            return;
        isAtTarget = false;
        agent.isStopped = false;
        enemyAttack.walking = true;
        enemyAttack.player = currentTarget;
        if (enemyAttack.melee && the)
        { 
            the = false;
            enemyAttack.Walk();
        }
        //rb.isKinematic = false;
        agent.SetDestination(currentTarget.transform.position);
        //Debug.Log("Moving to: " + currentTarget.name);
    }
    private void OnReachedTarget()
    {
        if (hasReachedTarget || isAtTarget) return;
        hasReachedTarget = true;
        enemyAttack.walking = false;
        the = true;
        isAtTarget = true;
        agent.isStopped = true;
        //rb.isKinematic = true;
        //Debug.Log("Target Reached!");
        //Debug.Log("Attack True");
        enemyAttack.isAttacking = true;
    }
}