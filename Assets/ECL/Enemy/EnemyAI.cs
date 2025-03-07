using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyDetection detection;
    public EnemyCombat combat;
    public EnemyDodge dodge;
    public EnemyAmmo ammo;

    private Transform target;

    private enum AIState { Idle, Chasing, Attacking, Dodging }
    private AIState state = AIState.Idle;

    void Update()
    {
        bool playerSpotted = detection.PlayerDetected();
        if (playerSpotted) target = detection.player;

        if (target == null)
        {
            state = AIState.Idle;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > combat.attackRange)
                state = AIState.Chasing;
            else if (ammo.CanShoot())
                state = AIState.Attacking;
        }

        HandleState();
    }

    void HandleState()
    {
        switch (state)
        {
            case AIState.Idle:
                movement.SetTarget(null);
                break;
            case AIState.Chasing:
                movement.SetTarget(target);
                break;
            case AIState.Attacking:
                combat.Attack(target);
                ammo.Shoot();
                break;
            case AIState.Dodging:
                dodge.TryDodge();
                break;
        }
    }
}