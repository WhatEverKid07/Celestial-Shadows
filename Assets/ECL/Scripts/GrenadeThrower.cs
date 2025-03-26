using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Space(20)]
    [Header("Grenade Attributes")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwDelay = 2f;
    [SerializeField] private int grenadeAmount = 2;
    [Space(10)]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;

    [Space(20)]
    [SerializeField] private GameObject grenadeThrower;
    [SerializeField] private InputActionAsset controls;
    
    [Space(20)]
    [Header("UI")]
    [SerializeField] private Text ammoText;

    private InputAction throwGrenade;
    private float currentThrowDelay;
    private Grenade grenadeScript;
    private float grenadeAnimPause = 1.2f;
    private bool thrown = false;

    private void Awake()
    {
        throwGrenade = controls.FindActionMap("Gun Controls").FindAction("Shoot");
    }
    private void Start()
    {
        throwGrenade.Enable();

        currentThrowDelay = throwDelay;
    }

    void OnEnable()
    {
        ammoText.gameObject.SetActive(true);
        UpdateAmmoText();
        throwGrenade.Enable();
    }
    private void OnDisable()
    {
        if (ammoText != null)
            ammoText.gameObject.SetActive(false);
        throwGrenade.Disable();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy)
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

            if (currentThrowDelay <= 0 && gameObject.activeInHierarchy)
            {
                Debug.Log("2");
                if (!thrown)
                {
                    thrown = true;
                    animator.Play("WholeGrenadeAnimation");
                    Invoke("ThrowGrenade", grenadeAnimPause);
                    currentThrowDelay = throwDelay;
                }
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
        UpdateAmmoText();
        thrown = false;
    }
    void UpdateAmmoText()
    {
        ammoText.text = grenadeAmount.ToString() + " Grenades Remaining";
    }
}