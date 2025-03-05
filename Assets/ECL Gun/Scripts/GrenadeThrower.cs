using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private Grenade grenadeScript;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwDelay = 2f;
    [SerializeField] private int grenadeAmount = 2;
    [SerializeField] private GameObject grenadeThrower;
    [SerializeField] private InputActionAsset controls;

    private InputAction throwGrenade;
    public float currentThrowDelay;

    private void Start()
    {
        throwGrenade = controls.FindActionMap("Gun Controls").FindAction("Shoot");
        throwGrenade.Enable();

        currentThrowDelay = throwDelay;
    }

    void Update()
    {
        if (!enabled)
        {
            throwGrenade.Disable();
            return;
        }
        if (currentThrowDelay >= 0 && grenadeAmount >= 1)
        {
            currentThrowDelay -= Time.deltaTime;
        }
        if (grenadeAmount <= 0)
        {
            Debug.Log("1");
            grenadeThrower.SetActive(false);
            return;
        }
        else if (grenadeAmount >= 1) { grenadeThrower.SetActive(true);}

        throwGrenade.performed += ctx => {

            if (currentThrowDelay <= 0)
            {
                Debug.Log("2");
                ThrowGrenade();
                currentThrowDelay = throwDelay;
            }
        };

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
        grenadeAmount--;
    }
}
