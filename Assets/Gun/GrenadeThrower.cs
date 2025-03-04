using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private Grenade grenadeScript;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwDelay = 2f;
    public float currentThrowDelay;

    private void Start()
    {
        currentThrowDelay = throwDelay;
    }

    void Update()
    {
        while (currentThrowDelay >= 0)
        {
            currentThrowDelay -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.G) && currentThrowDelay <= 0)
        {
            ThrowGrenade();
            currentThrowDelay = throwDelay;
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        grenadeScript = grenade.GetComponent<Grenade>();
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            grenadeScript.StartFuse();
            Vector3 throwDirection = (throwPoint.forward * throwForce) + (Vector3.up * (throwForce / 2));
            rb.AddForce((throwPoint.forward * throwForce) + (Vector3.up * (throwForce / 2)), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            grenadeScript = null;
        }
    }
}
