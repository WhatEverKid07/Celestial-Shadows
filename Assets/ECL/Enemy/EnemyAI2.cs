using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    public Transform target;
    public Transform player;
    public float rotationSpeed;
    public float speed;
    public float acceleration;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 120f;
    public LayerMask obstructionLayer;

    public Transform currentTarget;
    private NavMeshAgent agent;
    private bool hasReachedTarget = false;
    private bool hasPerformedAction = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        ////
        currentTarget = target;
        // update the target to go to
    }
    void Update()
    {
        if (player == null || target == null)
            return;

    }
}
