using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public Transform player;
    public float sightRange = 15f;
    public float fieldOfView = 90f;
    public LayerMask obstacles;

    public bool PlayerDetected()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
            {
                if (hit.transform == player)
                    return true;
            }
        }
        return false;
    }
}