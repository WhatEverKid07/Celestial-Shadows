using UnityEngine;

public class EnemyDodge : MonoBehaviour
{
    public float dodgeCooldown = 3f;
    private float nextDodgeTime;

    public void TryDodge()
    {
        if (Time.time >= nextDodgeTime)
        {
            Vector3 dodgeDirection = Random.insideUnitSphere * 3f;
            dodgeDirection.y = 0;
            transform.position += dodgeDirection;
            nextDodgeTime = Time.time + dodgeCooldown;
            Debug.Log("Enemy Dodged!");
        }
    }
}