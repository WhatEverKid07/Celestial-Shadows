using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private float recoilSpeed = 0.1f; // Speed of recoil effect
    [SerializeField] private Transform gunRoot;
    [SerializeField] private float recoilAmount = 10f; //control kickback
    private Quaternion originalRotation; // To store the original rotation of the gun

    private void Start()
    {
        originalRotation = gunRoot.transform.localRotation;
    }
    public void CallRecoilShake()
    {
        StartCoroutine(RecoilShake());
    }
    private IEnumerator RecoilShake()
    {
        float time = 0f;

        while (time < recoilSpeed)
        {
            // Apply recoil
            gunRoot.gameObject.transform.localRotation = Quaternion.Lerp(gunRoot.gameObject.transform.localRotation, originalRotation * Quaternion.Euler(-recoilAmount, 0, 0), time * recoilSpeed);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < recoilSpeed)
        {
            // Return to original position
            gunRoot.gameObject.transform.localRotation = Quaternion.Lerp(gunRoot.gameObject.transform.localRotation, originalRotation, time * recoilSpeed);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
