using System.Collections;
using UnityEngine;

public class EnemyAmmo : MonoBehaviour
{
    public int maxAmmo = 5;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public bool CanShoot()
    {
        return currentAmmo > 0 && !isReloading;
    }

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            if (currentAmmo == 0)
                StartCoroutine(Reload());
        }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}