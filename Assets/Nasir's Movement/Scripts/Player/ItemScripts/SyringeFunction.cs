using UnityEngine;

public class SyringeFunction : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStats playerStatsScript;

    [Header("Croc")]
    private GameObject syringe;

    private float rotationSpeed = 30f;
    private float bobbingAmplitude = 0.1f;
    private float bobbingFrequency = 1f;
    private float originalYPosition;

    internal bool canUpdateSyringeStat;

    private void Start()
    {
        playerStatsScript = FindAnyObjectByType<PlayerStats>();

        syringe = GameObject.Find("Syringe");

        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float newY = originalYPosition + (Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("PlayerPhys"))
        {
            playerStatsScript.syringe.Add(gameObject);
            canUpdateSyringeStat = true;

            syringe?.SetActive(false);

            Invoke(nameof(NoMoreUpdates), .0001f);
        }
    }

    private void NoMoreUpdates()
    {
        canUpdateSyringeStat = false;
        Destroy(this);
    }
}
